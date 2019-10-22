using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoggerTest.Handlers
{
    public abstract class ExceptionHandler
    {
        protected bool HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string data = null;
            if (exception.Data != null)
            {
                data = JsonConvert.SerializeObject(exception.Data, new JsonSerializerSettings()
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                });
            }

            logger.LogError(exception, exception.Message);

            context.Response.WriteAsync(new DefaultError()
            {
                Code = GetStatusCode(exception),
                Message = exception.Message,
                Data = data,
            }.ToString());

            return true;
        }

        private static int GetStatusCode(Exception exception)
        {
            switch (exception)
            {
                case NullReferenceException _:
                    return (int)HttpStatusCode.NotFound;
                case AccessViolationException _:
                    return (int)HttpStatusCode.Forbidden;
                case ArgumentException _:
                    return (int)HttpStatusCode.BadRequest;
                default:
                    return (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
