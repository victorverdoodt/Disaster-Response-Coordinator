using DRC.App.Components;
using DRC.App.Services;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace DRC.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();
            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration); //Add this

            builder.Services.AddHttpClient<ChatClientService>(client =>
            {
                client.BaseAddress = new Uri("http://api");
            });
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
