
using ChatAIze.GenerativeCS.Extensions;
using DRC.Api.Interfaces;
using DRC.Api.Services;
using ViaCep;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Extensions;

namespace DRC.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();

            builder.Services.AddGeminiClient(builder.Configuration["Apps:Gemini:Key"]);

            builder.Services.AddWhatsAppBusinessCloudApiService(
                new WhatsAppBusinessCloudApiConfig
                {
                    AccessToken = builder.Configuration["Apps:Meta:AccessToken"],
                    AppName = builder.Configuration["Apps:Meta:AppName"],
                    WhatsAppBusinessAccountId = builder.Configuration["Apps:Meta:WhatsAppBusinessAccountId"],
                    WhatsAppBusinessId = builder.Configuration["Apps:Meta:WhatsAppBusinessId"],
                    WhatsAppBusinessPhoneNumberId = builder.Configuration["Apps:Meta:WhatsAppBusinessPhoneNumberId"]
                });

            builder.Services.AddHttpClient<IViaCepClient, ViaCepClient>(client => 
            { 
                client.BaseAddress = new Uri("https://viacep.com.br/"); 
            });

            builder.Services.AddHttpClient<IS2iDService, S2iDService>(client =>
            {
                client.BaseAddress = new Uri("https://s2id.mi.gov.br");
            });

            builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>(client =>
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json");
            });

            builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(client =>
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/geocode/json");
            });

            builder.AddRedisDistributedCache("redis");

            builder.Services.AddScoped<ICepService, CepService>();
            builder.Services.AddScoped<IChatCacheService, ChatCacheService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IWhatAppService, WhatsAppCloudService>();

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
