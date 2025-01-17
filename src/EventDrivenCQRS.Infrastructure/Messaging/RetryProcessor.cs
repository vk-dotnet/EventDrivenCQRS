using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventDrivenCQRS.Infrastructure.Messaging
{
    public class RetryProcessor
    {
        private readonly IConnectionFactory _connectionFactory;

        public RetryProcessor(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void ProcessRetries(string dlQueue, string mainQueue, TimeSpan retryInterval)
        {
            while (true) // Sonsuz döngü
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"[Retry] Processing message: {message}");

                    try
                    {
                        // Mesajı ana kuyruğa tekrar gönder
                        channel.BasicPublish(exchange: "", routingKey: mainQueue, basicProperties: null, body: body);
                        Console.WriteLine($"[Retry] Message sent back to main queue: {message}");

                        // Mesaj işlenince DLQ'dan sil
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Retry] Failed to process message: {message}. Error: {ex.Message}");
                        // Başarısız olan mesajı silmeyerek yeniden işlenmesini sağlar
                    }
                };

                try
                {
                    channel.BasicConsume(queue: dlQueue, autoAck: false, consumer: consumer);
                    Console.WriteLine($"[Retry] Listening on DLQ: {dlQueue}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Retry] Failed to consume messages from DLQ: {dlQueue}. Error: {ex.Message}");
                }

                // Mesajları tekrar kontrol etmek için bekleme süresi
                Task.Delay(retryInterval).Wait();
            }
        }
    }
}
