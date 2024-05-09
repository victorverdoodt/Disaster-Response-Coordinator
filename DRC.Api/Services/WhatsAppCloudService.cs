using DRC.Api.Interfaces;
using System.Security.Cryptography;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi;
using System.Text;

namespace DRC.Api.Services
{
    public class WhatsAppCloudService : IWhatAppService
    {
        private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
        private readonly IChatService _chatService;
        public WhatsAppCloudService(IWhatsAppBusinessClient whatsAppBusinessClient, IChatService chatService)
        {
            _whatsAppBusinessClient = whatsAppBusinessClient;
            _chatService = chatService;
        }
        public async Task<bool> ReceiveMessage(string phone, string message)
        {
            var GuidGen = CreateGuidFromSeed(phone);
            var response = await _chatService.SendMessage(GuidGen, message);
            await SendMessage(phone, response.message);
            return true;
        }

        public async Task<bool> SendTemplateMessage(string phone, string template, List<TextMessageComponent> parameters = null)
        {
            TextTemplateMessageRequest textTemplateMessage = new()
            {
                To = phone,
                Template = new()
                {
                    Name = template,
                    Language = new()
                    {
                        Code = LanguageCode.Portuguese_BR
                    }
                },
            };

            if (parameters is not null)
            {
                textTemplateMessage.Template.Components = parameters;
            }

            var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(textTemplateMessage);
            return true;
        }

        public async Task<bool> SendMessage(string phone, string message)
        {
            TextMessageRequest textMessageRequest = new()
            {
                To = phone,
                Text = new()
                {
                    Body = message,
                    PreviewUrl = false
                }
            };

            var results = await _whatsAppBusinessClient.SendTextMessageAsync(textMessageRequest);
            return true;
        }

        private Guid CreateGuidFromSeed(string seed)
        {
            using (var hash = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(seed);
                byte[] hashBytes = hash.ComputeHash(bytes);

                return new Guid(hashBytes[..16]);
            }
        }
    }
}
