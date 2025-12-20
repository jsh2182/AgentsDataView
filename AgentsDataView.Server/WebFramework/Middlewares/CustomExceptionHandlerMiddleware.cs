using AgentsDataView.Common;
using AgentsDataView.Common.Exceptions;
using AgentsDataView.Common.Utilities;
using AgentsDataView.WebFramework.Api;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AgentsDataView.WebFramework.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware(RequestDelegate next,  IWebHostEnvironment env, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly IWebHostEnvironment _env = env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            string? message = null;
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            ApiResultStatusCode apiStatusCode = ApiResultStatusCode.ServerError;

            try
            {
                await _next(context);
            }
            catch (AppException exception)
            {
                _logger.LogError(exception, exception.Message);
                httpStatusCode = exception.HttpStatusCode;
                apiStatusCode = exception.ApiStatusCode;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace ?? "",
                    };
                    if (exception.InnerException != null)
                    {
                        message = exception.InnerException.Message;
                        dic.Add("InnerException.Exception", message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace ?? "");
                    }
                    if (exception.AdditionalData != null)
                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                    message = JsonConvert.SerializeObject(dic);
                }
                else
                {
                    message = exception.Message;
                }
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                string msg = "";
                Exception tmpEx = exception;
                while (true)
                {
                    if (!tmpEx.Message.Contains("See the inner exception for details"))
                    {
                        msg += tmpEx.Message;
                    }

                    if (tmpEx.InnerException == null)
                    {
                        break;
                    }

                    tmpEx = tmpEx.InnerException;
                }
                if (exception.StackTrace != null)
                {
                    msg += _getProjectCallStack(exception, "AgentsDataView.Server");
                }
                _logger.LogError(exception, msg);

                if (_env.IsDevelopment())
                {
                    //var dic = new Dictionary<string, string>
                    //{
                    //    ["Exception"] = exception.Message,
                    //    ["StackTrace"] = exception.StackTrace ?? "",
                    //};
                    message = JsonConvert.SerializeObject(new {Exception = msg});
                    //message = msg;
                }
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

                var result = new ApiResult(false, apiStatusCode, message);
                var json = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiStatusCode = ApiResultStatusCode.UnAuthorized;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace ?? ""
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires.ToString());

                    message = JsonConvert.SerializeObject(dic);
                }
            }
        }
        private static string _getProjectCallStack(Exception ex, string projectNamespacePrefix)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();
            var stackTrace = new StackTrace(ex, true);
            var frames = stackTrace.GetFrames();
            if (frames == null) return string.Empty;

            // ترتیب از اول تا آخر فراخوانی
            foreach (var frame in frames.Reverse())
            {
                MethodBase method = frame.GetMethod();
                if (method == null || method.DeclaringType == null) continue;

                // فقط متدهای پروژه
                if (!method.DeclaringType.FullName.StartsWith(projectNamespacePrefix, StringComparison.OrdinalIgnoreCase)) continue;

                string className = method.DeclaringType.Name;
                string methodName = method.Name;
                //string fileName = frame.GetFileName() != null ? Path.GetFileName(frame.GetFileName()) : "(unknown file)";
                int lineNumber = frame.GetFileLineNumber();
                string lineStr = lineNumber > 0 ? $"Line: {lineNumber}" : "";
                sb.AppendLine($"{className}.{methodName} {lineStr}");//in {fileName}
            }

            return sb.ToString();
        }
    }
}
