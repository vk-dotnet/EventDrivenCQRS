using EventDrivenCQRS.Infrastructure;
using EventDrivenCQRS.Infrastructure.Messaging;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 🌐 Add services to the container
builder.Services.AddControllers();

// 🔍 OpenAPI (Swagger) Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Driven CQRS API", Version = "v1" });
});

// 📌 MediatR and CQRS Configuration
builder.Services.AddMediatR(typeof(EventDrivenCQRS.Application.AssemblyReference).Assembly);

// 📌 FluentValidation Service Registration
builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(typeof(EventDrivenCQRS.Application.AssemblyReference).Assembly);

// 📌 Register Infrastructure Services (Database, Messaging, Cache)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 🌐 CORS Policy (Optional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🌐 OpenAPI (Swagger) Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔒 HTTPS Redirection Middleware
app.UseHttpsRedirection();

// 🚀 CORS Middleware
app.UseCors("AllowAll");

// 📌 Register API Controllers
app.MapControllers();

// RabbitMQ Initialization and Testing
using (var scope = app.Services.CreateScope())
{
    var rabbitMqService = scope.ServiceProvider.GetRequiredService<RabbitMqService>();
    var initializer = scope.ServiceProvider.GetRequiredService<RabbitMqInitializer>();

    // Initialize RabbitMQ (queues, exchanges)
    initializer.Initialize();

    // Configure Dead Letter Queue (DLQ)
    rabbitMqService.ConfigureQueueWithDLQ("main_queue", "dlq_queue");

    // Test Message Publishing
    rabbitMqService.PublishMessage("test_exchange", "test_queue", "Normal Message 1");
    rabbitMqService.PublishMessage("test_exchange", "test_queue", "Error Message 1");
}

// RabbitMQ Consumers and Retry Processor
using (var scope = app.Services.CreateScope())
{
    var rabbitMqService = scope.ServiceProvider.GetRequiredService<RabbitMqService>();
    var retryProcessor = scope.ServiceProvider.GetRequiredService<RetryProcessor>();

    // Main Queue Listener
    rabbitMqService.StartListening("main_queue", consumerCount: 3);

    // Dead Letter Queue Listener
    rabbitMqService.StartListening("dlq_queue");

    // Retry Processor for Dead Letter Queue
    Task.Run(() => retryProcessor.ProcessRetries("dlq_queue", "main_queue", TimeSpan.FromMinutes(1)));
}

app.Run();
