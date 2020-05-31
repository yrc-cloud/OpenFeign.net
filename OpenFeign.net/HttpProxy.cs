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
using System.Collections.Concurrent;
using OpenFeign.net.Reflection;

namespace OpenFeign.net
{
    public class HttpProxy : DispatchProxy
    {
        private HttpClient HttpClient => new HttpClient();

        protected string ServerAddress { get; set; }

        protected ConcurrentDictionary<string, MethodProperty> Methods { get; } = new ConcurrentDictionary<string, MethodProperty>();

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

            var methodInfo = Methods.GetOrAdd(targetMethod.Name, name =>
            {
                return GetMethodProperty(targetMethod);
            });

            for (var index = 0; index < methodInfo.Parameters.Count; index++)
            {
                var parameterInfo = methodInfo.Parameters[index];
                var arg = args.GetValue(index);

                switch (parameterInfo.Type)
                {
                    case ParameterType.Path:
                        //TODO: this should not convert to string
                        pathVariables.Add(parameterInfo.Name, arg?.ToString());
                        break;
                    case ParameterType.Query:
                        //TODO: this should not convert to string
                        requestParameters.Add(parameterInfo.Name, arg?.ToString());
                        break;
                    case ParameterType.Body:
                        bodyParameters.Add(parameterInfo.Name, arg);
                        break;
                    case ParameterType.Header:
                        headerParameters.Add(parameterInfo.Name, arg?.ToString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var endPoint = methodInfo.Path;
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

            var message = new HttpRequestMessage(methodInfo.HttpMethod, endPoint);
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

        protected MethodProperty GetMethodProperty(MethodInfo method)
        {
            var methodProperty = new MethodProperty();
            var requestMethod = method.GetCustomAttribute<HttpRequestMethodAttribute>();
            methodProperty.Path = requestMethod.Path;
            methodProperty.HttpMethod = requestMethod.Method;
            methodProperty.Parameters = new List<Reflection.ParameterInfo>();
            foreach(var param in method.GetParameters())
            {
                var parameterType = param.GetCustomAttribute<ParameterAttribute>();
                if (parameterType == null)
                {
                    throw new ArgumentException($"{method.Name}:{param.Name} don't have an attribute");
                }
                var key = string.IsNullOrEmpty(parameterType.Name) ? param.Name : parameterType.Name;
                methodProperty.Parameters.Add(new Reflection.ParameterInfo(
                    key,
                    parameterType.ParameterType,
                    Reflection.ParameterInfo.IsSimpleType(param.GetType())
                    ));
            }

            return methodProperty;
        }
    }
}
