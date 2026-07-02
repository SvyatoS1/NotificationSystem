using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TicketNotificationApi.Api.Middleware;
using TicketNotificationApi.Application;
using TicketNotificationApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ticket Notification API",
        Description = "An ASP.NET Core Web API for managing ticket notifications."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
