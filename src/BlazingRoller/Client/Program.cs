using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazingRoller.Client.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingRoller.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredModal();
            builder.Services.AddTransient(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddLogging();
            builder.Services.AddSingleton<SessionStorageService>();
            builder.Services.AddTransient<JsInteropService>();
            builder.Services.AddTransient<RoomHubConnection>();

            await builder.Build().RunAsync();
        }
    }
}
