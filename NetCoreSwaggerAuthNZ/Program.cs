using NetCoreSwaggerAuthNZ.Infrastructure;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddAuthorization();

// Adds controllers to the dependency injection container.
builder.Services.AddControllers();

// Adds API explorer and Swagger UI support to the application.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        { new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        }, new List<string>() }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI when in development environment.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect all HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable authorization for the application.
app.UseAuthorization();

// Map controllers to the request pipeline.
app.MapControllers();


app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello, {user.Identity?.Name}!").RequireAuthorization();

//app.UseMiddleware<SwaggerBasicAuthMiddleware>();

// Start the application.
app.Run();