using EventDrivenCQRS.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EventDrivenCQRS.Infrastructure.Persistence;
using StackExchange.Redis;
using RabbitMQ.Client;
using MongoDB.Driver;

namespace EventDrivenCQRS.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // PostgreSQL
            services.AddDbContext<EventDrivenCQRSDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

            // MongoDB
            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(configuration.GetConnectionString("MongoDB")));

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

            // RabbitMQ
            services.AddSingleton<IConnectionFactory>(sp =>
                new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"],
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]
                });

            services.AddSingleton<RabbitMqService>();
            services.AddSingleton<RetryProcessor>();
            services.AddScoped<RabbitMqInitializer>();
            
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            return services;
        }
    }
}