using Microsoft.AspNetCore.Mvc;
using EventDrivenCQRS.Infrastructure.Messaging;

namespace EventDrivenCQRS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqController : ControllerBase
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly ILogger<RabbitMqController> _logger;
        public RabbitMqController(RabbitMqService rabbitMqService, ILogger<RabbitMqController> logger)
        {
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }
        /// <summary>
        /// RabbitMQ'ya normal ve hata mesajları gönderir.
        /// </summary>
        /// <returns>İşlem sonucu</returns>
        [HttpPost("publish")]
        public IActionResult PublishMessage()
        {
            try
            {
                // Normal bir mesaj gönder
                _rabbitMqService.PublishMessage("test_exchange", "test_queue", "Hello from RabbitMQ Controller!");

                // Hata simülasyonu için bir mesaj gönder
                _rabbitMqService.PublishMessage("test_exchange", "test_queue", "Error Controller!");

                return Ok("Messages published to RabbitMQ.");
            }
            catch (Exception ex)
            {
                // Hata durumunda ayrıntılı bilgi döndür
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// RabbitMQ'ya özel bir mesaj gönderir.
        /// </summary>
        /// <param name="message">Gönderilecek mesaj</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPost("publish-custom")]
        public IActionResult PublishCustomMessage([FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Message cannot be empty.");
            }

            try
            {
                _rabbitMqService.PublishMessage("test_exchange", "test_queue", message);
                return Ok($"Custom message published to RabbitMQ: {message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
