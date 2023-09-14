

using System.Net;
using BlogManager.Core.Logger;
using Newtonsoft.Json;

namespace BlogManager.Adapter.Api.Utilities
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate    _next;
        private readonly IBlogManagerLogger _logger;


        public ErrorHandlerMiddleware(RequestDelegate next, IBlogManagerLogger logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError($"Error occurred on {context.Request.Method}:{context.Request.Path.Value}", error);

                var response = context.Response;
                response.ContentType = "application/json";
        
                string         message;
                HttpStatusCode statusCode;

                switch (error)
                {
                    case UnauthorizedAccessException _:
                        statusCode = HttpStatusCode.Unauthorized;
                        message    = "Unauthorized";
                        break;

                    // ... other custom exception types

                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        message    = UnwrapException(error);
                        break;
                }

                response.StatusCode = (int)statusCode;

                var result = JsonConvert.SerializeObject(new { message });
                await response.WriteAsync(result);
            }
        }
        
        private string UnwrapException(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return UnwrapException(ex.InnerException);
            }
            return ex.Message;
        }
    }
}