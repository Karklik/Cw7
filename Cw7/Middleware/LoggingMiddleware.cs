using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cw7.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                var path = context.Request.Path;
                var method = context.Request.Method;
                var queryString = context.Request.QueryString;
                var body = "";
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                var lines = new List<string>
                {
                    "====================================================================",
                    "On: " + DateTime.Now.ToString(),
                    "Method: " + method,
                    "Path: " + path,
                    "QueryString: " + queryString,
                    "Body:\n" + body
                };
                await File.AppendAllLinesAsync("requestsLog.txt ", lines, Encoding.UTF8);
            }

            await _next(context);
        }
    }
}
