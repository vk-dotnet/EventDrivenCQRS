using EventDrivenCQRS.Infrastructure.Messaging;
using System;

public class RabbitMqInitializer
{
    private readonly RabbitMqService _rabbitMqService;

    public RabbitMqInitializer(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public void Initialize()
    {
        try
        {
            Console.WriteLine("[Initializer] Configuring Dead Letter Queue...");
            // RabbitMQ DLQ yapılandırması
            _rabbitMqService.ConfigureQueueWithDLQ("main_queue", "dlq_queue");
            Console.WriteLine("[Initializer] Dead Letter Queue configuration completed.");
        }
        catch (Exception ex)
        {
            // Hata loglama
            Console.WriteLine($"[Initializer] Failed to configure DLQ: {ex.Message}");
            throw;
        }
    }
}