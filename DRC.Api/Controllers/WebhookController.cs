using DRC.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Webhook;

namespace DRC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public WebhookController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("whatsapp")]
        public async Task<IActionResult> GetWhatsApp([FromQuery(Name = "hub.mode")] string hubMode, [FromQuery(Name = "hub.challenge")] int hubChallenge, [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            var key = _configuration["Apps:Meta:Key"];
            if (!hubVerifyToken.Equals(key))
            {
                return Forbid();
            }

            return Ok(hubChallenge);
        }

        [AllowAnonymous]
        [HttpPost("whatsapp")]
        public async Task<IActionResult> PostWhatsApp(IWhatsAppBusinessClient whatsAppBusinessClient, IWhatAppService whatAppService, [FromBody] dynamic messageReceived)
        {
            if (messageReceived is null)
            {
                return BadRequest(new
                {
                    Message = "Message not received"
                });
            }

            var msg = messageReceived.ToString();
            JsonDocument doc = JsonDocument.Parse(msg);

            if (doc.RootElement.TryGetProperty("entry", out var entries) && entries.EnumerateArray().Any())
            {
                var firstEntry = entries.EnumerateArray().First();

                if (firstEntry.TryGetProperty("changes", out var changes) && changes.EnumerateArray().Any())
                {
                    var firstChange = changes.EnumerateArray().First();

                    if (firstChange.TryGetProperty("value", out var value))
                    {
                        bool isStatusesNull = !value.TryGetProperty("statuses", out var statuses) || statuses.ValueKind == JsonValueKind.Null || (statuses.ValueKind == JsonValueKind.Array && !statuses.EnumerateArray().Any());

                        if (isStatusesNull)
                        {
                            if (value.TryGetProperty("messages", out var messages) && messages.EnumerateArray().Any())
                            {
                                var firstMessage = messages.EnumerateArray().First();

                                if (firstMessage.TryGetProperty("type", out var type))
                                {
                                    string messageType = type.GetString();
                                    if (messageType.Equals("text"))
                                    {
                                        var textMessageReceived = JsonConvert.DeserializeObject<TextMessageReceived>(Convert.ToString(messageReceived)) as TextMessageReceived;
                                        var textMessage = new List<TextMessage>(textMessageReceived.Entry.SelectMany(x => x.Changes).SelectMany(x => x.Value.Messages));

                                        MarkMessageRequest markMessageRequest = new MarkMessageRequest();
                                        var metadata = textMessage.SingleOrDefault();
                                        markMessageRequest.MessageId = metadata.Id;
                                        markMessageRequest.Status = "read";

                                        await whatsAppBusinessClient.MarkMessageAsReadAsync(markMessageRequest);
                                        await whatAppService.ReceiveMessage(metadata.From, metadata.Text.Body);

                                        return Ok(new
                                        {
                                            Message = "Text Message received"
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Ok();
        }
    }
}
