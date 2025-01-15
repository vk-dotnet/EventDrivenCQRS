using EventDrivenCQRS.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore; // Doğru namespace

var builder = WebApplication.CreateBuilder(args);

// 🌐 Add services to the container
builder.Services.AddControllers();

// 🔍 OpenAPI (Swagger) konfigürasyonu
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Driven CQRS API", Version = "v1" });
});

// 📌 MediatR ve CQRS yapılandırması
builder.Services.AddMediatR(typeof(EventDrivenCQRS.Application.AssemblyReference).Assembly);


// 📌 FluentValidation servisini ekleme
// builder.Services.AddValidatorsFromAssemblyContaining<EventDrivenCQRS.Application.AssemblyReference>();

// 📌 FluentValidation servisini ekleme
builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(typeof(Program).Assembly);





// // 📌 AutoMapper Konfigürasyonu
// builder.Services.AddAutoMapper(typeof(EventDrivenCQRS.Application.AssemblyReference));

// 📌 Infrastructure Servislerini Ekleyelim (Database, Messaging, Cache)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 🌐 CORS Politikası (İsteğe Bağlı)
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

// 🌐 OpenAPI (Swagger) Middleware'i
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔒 HTTPS Yönlendirmesi
app.UseHttpsRedirection();

// 🚀 CORS Middleware
app.UseCors("AllowAll");

// 📌 API Controller'larını ekleyelim
app.MapControllers();

app.Run();