
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkyCarsWebAPI.Extensions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        //public async Task Invoke(HttpContext context, ILogService _logService)
        //{
        //    try
        //    {
        //        context.Request.EnableBuffering();
        //        await _next(context);

        //    }
        //    catch (Exception ex)
        //    {
        //        var response = context.Response;
        //        response.ContentType = "application/json";
        //        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        string param = "";
        //        if (context.Request.Method.ToLower() == "get")
        //        {
        //            param = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
        //        }
        //        else
        //        {
        //            context.Request.Body.Seek(0, SeekOrigin.Begin);
        //            using (StreamReader stream = new(context.Request.Body))
        //            {
        //                param = stream.ReadToEnd();
        //            }
        //            context.Request.Body.Dispose();
        //        }

        //        var msg = "{Params:" + param + ",DetailError:" + ex.InnerException + "}";
        //        await _logService.ErrorAsync($"{context.Request.Method}-{context.Request.Path.Value}", msg, Core.Domain.Logging.LogSource.API, ex, 0).ConfigureAwait(false);
        //        var errorJson = JsonSerializer.Serialize(ApiResponseHelper.GenerateResponse(ApiStatusCode.Status500InternalServerError, "Error. Please try again after some time.", new { ApiUrl = context.Request.Path.Value }));

        //        await response.WriteAsync(errorJson);
        //    }
        //}
    }
}
