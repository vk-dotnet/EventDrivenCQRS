using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace EventDrivenCQRS.Infrastructure.Messaging
{
    public class RabbitMqService
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMqService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void PublishMessage(string exchange, string routingKey, string message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            try
            {
                // Exchange'in var olup olmadığını kontrol et
                channel.ExchangeDeclare(exchange: exchange, type: "direct", durable: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PublishMessage] Failed to declare exchange '{exchange}': {ex.Message}");
                throw;
            }

            try
            {
                // Kuyruğun var olup olmadığını kontrol et
                channel.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: routingKey, exchange: exchange, routingKey: routingKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PublishMessage] Failed to declare or bind queue '{routingKey}': {ex.Message}");
                throw;
            }

            // Mesajı gönder
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);

            Console.WriteLine($"Message sent to exchange '{exchange}' with routing key '{routingKey}': {message}");
        }

        public void StartListening(string queueName, int consumerCount = 1)
        {
            for (int i = 0; i < consumerCount; i++)
            {
                Task.Run(() =>
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();

                    try
                    {
                        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[StartListening] Failed to declare queue '{queueName}': {ex.Message}");
                        return;
                    }

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"[Consumer-{Task.CurrentId}] Received: {message}");

                        try
                        {
                            if (message.Contains("Error"))
                            {
                                Console.WriteLine($"[Consumer-{Task.CurrentId}] Error occurred while processing: {message}");
                                throw new Exception("Simulated processing error.");
                            }

                            channel.BasicAck(ea.DeliveryTag, false);
                            Console.WriteLine($"[Consumer-{Task.CurrentId}] Processed successfully: {message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Consumer-{Task.CurrentId}] Message rejected: {message}, Error: {ex.Message}");
                            channel.BasicReject(ea.DeliveryTag, false);
                        }
                    };

                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.WriteLine($"[Consumer-{Task.CurrentId}] Listening on queue: {queueName}");
                });
            }
        }

        public void ConfigureQueueWithDLQ(string mainQueue, string dlQueue)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            try
            {
                var args = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" }, // Default exchange
                    { "x-dead-letter-routing-key", dlQueue }
                };

                channel.QueueDeclare(queue: mainQueue, durable: true, exclusive: false, autoDelete: false, arguments: args);
                channel.QueueDeclare(queue: dlQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine($"[ConfigureQueueWithDLQ] Configured {mainQueue} with DLQ: {dlQueue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConfigureQueueWithDLQ] Failed to configure queues: {ex.Message}");
                throw;
            }
        }
    }
}
