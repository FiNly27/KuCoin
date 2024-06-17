using CryptoExchange.Net.Authentication;
using KuCoinApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<TelegramService>();
        services.AddHostedService<MexcBackgroundService>();
        services.AddScoped<MexcService>();
        services.AddMexc(options =>
        {
            options.ApiCredentials = new ApiCredentials("mx0vglgQmhRlHcYL7U", "9eb8ba350cba4065b293d0e6dd4f5c71");
        });
    })
    .Build();
await host.RunAsync();