var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adds controllers to the dependency injection container.
builder.Services.AddControllers();

// Adds API explorer and Swagger UI support to the application.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Start the application.
app.Run();