using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoggerTest.Filters
{
    public class ArgumentExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is ArgumentException)) return;

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
            context.Result = new JsonResult(new
            {
                Content = context.Exception.Message,
                ReasonPhrase = "Parameter(s) incorrect",
            });
        }
    }
}
