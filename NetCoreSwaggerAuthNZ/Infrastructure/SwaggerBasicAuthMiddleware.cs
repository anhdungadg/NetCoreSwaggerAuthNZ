using Microsoft.VisualBasic;
using System.Net;
using System.Security.Cryptography;
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
            if (referer.Any(s => s.Contains("swagger")))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null)
                {
                    if (authHeader.StartsWith("Basic"))
                    {
                        var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                        var username = decodedUsernamePassword.Split(":", 2)[0];
                        var password = decodedUsernamePassword.Split(":", 2)[1];

                        if (IsAuthorized(username, password))
                        {
                            await _next.Invoke(context);
                            return;
                        }
                    }
                    else if (authHeader.StartsWith("Bearer"))
                    {
                        // Check token

                        // Lấy ngày tháng năm hiện tại
                        DateTime now = DateTime.Now;
                        string dateString = now.ToString("yyyyMMdd");
                        await Console.Out.WriteLineAsync("dateString " + dateString);

                        // Tạo đối tượng MD5
                        MD5 md5 = MD5.Create();

                        // Chuyển đổi chuỗi thành mảng byte
                        byte[] inputBytes = Encoding.ASCII.GetBytes(dateString);

                        // Tạo chuỗi MD5
                        byte[] hashBytes = md5.ComputeHash(inputBytes);
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < hashBytes.Length; i++)
                        {
                            sb.Append(hashBytes[i].ToString("X2"));
                        }
                        string md5String = sb.ToString();
                        await Console.Out.WriteLineAsync("md5String " + md5String);

                        var token = authHeader.Substring("Bearer ".Length).Trim();
                        if (token == md5String)
                        {
                            await _next.Invoke(context);
                        }
                        else
                        {
                            context.Response.Headers["WWW-Authenticate"] = "Bearer";
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                    }
                    else
                    {
                        //context.Response.Headers["WWW-Authenticate"] = "Basic";
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                } else
                {
                    //context.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
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
