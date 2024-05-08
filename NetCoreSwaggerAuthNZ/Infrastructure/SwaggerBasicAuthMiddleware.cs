using Microsoft.VisualBasic;
using System.Net;
using System.Text;

namespace NetCoreSwaggerAuthNZ.Infrastructure
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration configuration;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var referer = context.Request.Headers["Referer"];

            //if(context.Request.Path.StartsWithSegments("/swagger"))
            if(referer.Any(s => s.Contains("swagger")))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if(authHeader.StartsWith("Basic"))
                {
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    var username = decodedUsernamePassword.Split(":", 2)[0];
                    var password = decodedUsernamePassword.Split(":", 2)[1];

                    if(IsAuthorized(username, password))
                    {
                        await _next.Invoke(context);
                        return;
                    }
                }

                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            } 
            else
            {
                await _next.Invoke(context);
            }
        }

        public bool IsAuthorized(string username, string password)
        {
            return username == configuration["Swagger:Username"] && password == configuration["Swagger:Password"];
        }
    }
}
