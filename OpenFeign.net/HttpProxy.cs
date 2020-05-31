using OpenFeign.net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenFeign.net
{
    public class HttpProxy : DispatchProxy
    {
        private HttpClient HttpClient => new HttpClient();

        protected string ServerAddress { get; set; }

        private void SetServerAddress(string url)
        {
            if (!string.IsNullOrEmpty(ServerAddress))
            {
                throw new Exception("server address is already set");
            }

            ServerAddress = url;
        }

        public static T Create<T>() where T : class
        {
            var type = typeof(T);

            var serviceAttribute =
                type.GetCustomAttribute<FeignClientAttribute>();
            if (serviceAttribute == null)
            {
                throw new Exception("No FeignClientAttribute found on type:" + type.FullName);
            }
            if (string.IsNullOrEmpty(serviceAttribute.Url))
            {
                throw new Exception("server address is empty");
            }

            var obj = DispatchProxy.Create<T, HttpProxy>();

            if (obj is HttpProxy proxy)
            {
                proxy.SetServerAddress(serviceAttribute.Url);
            }

            return obj;
        }
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var pathVariables = new Dictionary<string, string>();
            var requestParameters = new Dictionary<string, string>();
            var headerParameters = new Dictionary<string, string>();
            var bodyParameters = new Dictionary<string, object>();

            for (var index = 0; index < targetMethod.GetParameters().Length; index++)
            {
                var parameter = targetMethod.GetParameters()[index];
                var arg = args.GetValue(index);

                var parameterType = parameter.GetCustomAttribute<ParameterAttribute>();
                if (parameterType == null)
                {
                    throw new ArgumentException($"{targetMethod.Name}:{parameter.Name} don't have an attribute");
                }
                var key = string.IsNullOrEmpty(parameterType.Name) ? parameter.Name : parameterType.Name;
                switch (parameterType.ParameterType)
                {
                    case ParameterType.Path:
                        //TODO: this should not convert to string
                        pathVariables.Add(key, arg?.ToString());
                        break;
                    case ParameterType.Query:
                        //TODO: this should not convert to string
                        requestParameters.Add(key, arg?.ToString());
                        break;
                    case ParameterType.Body:
                        bodyParameters.Add(key, arg);
                        break;
                    case ParameterType.Header:
                        headerParameters.Add(key, arg?.ToString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var requestMethod = targetMethod.GetCustomAttribute<HttpRequestMethodAttribute>();
            var endPoint = requestMethod.Path;
            endPoint = pathVariables.Aggregate(endPoint, (current, entry) => current.Replace($"{{{entry.Key}}}", entry.Value));

            var query = new StringBuilder();
            foreach (var entry in requestParameters)
            {
                query.Append(query.Length == 0 ? "?" : "&")
                    .Append(WebUtility.UrlEncode(entry.Key))
                    .Append("=")
                    .Append(WebUtility.UrlEncode(entry.Value));
            }
            if (query.Length > 0)
            {
                endPoint += query.ToString();
            }

            endPoint = ServerAddress + endPoint;

            var message = new HttpRequestMessage(requestMethod.Method, endPoint);
            foreach (var headerParameter in headerParameters)
            {
                message.Headers.Add(headerParameter.Key, headerParameter.Value);
            }
            var bodyJson =
                JsonConvert.SerializeObject(bodyParameters.Count == 1 ? bodyParameters.First().Value : bodyParameters);
            message.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            if (targetMethod.ReturnType.GetInterfaces().Any(i => i == typeof(IAsyncResult)))
            {
                if (targetMethod.ReturnType.IsGenericType &&
                    targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var method = GetType().GetMethod(nameof(InvokeAsync), BindingFlags.Instance | BindingFlags.NonPublic);
                    var generic = method.MakeGenericMethod(targetMethod.ReturnType.GenericTypeArguments);
                    return generic.Invoke(this, new []{message});
                }

                if (targetMethod.ReturnType == typeof(Task))
                {
                    var method = GetType().GetMethod(nameof(InvokeWithNoResultAsync), BindingFlags.Instance | BindingFlags.NonPublic);
                    return method.Invoke(this, new[] { message });
                }
            }

            var response = HttpClient.SendAsync(message).Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(json, targetMethod.ReturnType);
            }

            return null;
        }

        protected async Task<T> InvokeAsync<T>(HttpRequestMessage message)
        {
            var response = await HttpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }

        protected async Task InvokeWithNoResultAsync(HttpRequestMessage message)
        {
            await HttpClient.SendAsync(message);
        }
    }
}
