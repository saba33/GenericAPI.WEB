using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace GenericAPI.WEB.Infrastructure.Helper
{
    public static class ExceptionValidator
    {
        public static void ValidateException(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;
            var sw = Stopwatch.StartNew();
            var ReasonPhrase = context.Features.Get<IHttpResponseFeature>().ReasonPhrase;
            var method = context.Request.Method;
            var path = context.Request.Path;


            if (statusCode >= 500)
            {
                Serilog.Log.Error($"{statusCode} {ReasonPhrase} for request {method} {path} ({sw.ElapsedMilliseconds}ms)");
            }
            else if (statusCode >= 400)
            {

                Serilog.Log.Warning($"{statusCode} {ReasonPhrase} for request {context.Request.Method} {context.Request.Path} ({sw.ElapsedMilliseconds}ms)");
            }
            else
            {
                Serilog.Log.Information($"{method} {path} responded {statusCode} ({sw.ElapsedMilliseconds}ms)");
            }
        }
    }
}
