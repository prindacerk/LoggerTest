using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoggerTest.Filters
{
    public class NotImplementedExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is NotImplementedException)) return;

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            context.Result = new JsonResult(new
            {
                Content = "This feature has not yet been implemented to completion.",
                ReasonPhrase = "Implementation not complete",
            });
        }
    }
}
