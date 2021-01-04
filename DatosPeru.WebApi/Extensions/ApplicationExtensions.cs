using DatosPeru.WebApi.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DatosPeru.WebApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var error = context.Features.Get<IExceptionHandlerPathFeature>();

                if (error.Error is ApiException)
                {
                    var ex = error.Error as ApiException;
                    await context.Response.WriteAsync(ex.ToString());
                }
                else
                {
                    await context.Response.WriteAsync(new ApiException(error.Error.Message).ToString());
                }
            }));
        }
    }
}
