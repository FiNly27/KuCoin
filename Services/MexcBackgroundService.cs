using System.Globalization;
using System.Text.Json;
using KuCoinApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KuCoinApp.Services;

public class MexcBackgroundService : BackgroundService
{
    private static readonly HttpClient client = new();
    private readonly ILogger<TelegramService> _logger;

    public MexcBackgroundService(ILogger<TelegramService> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Hosted Service running.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (CoinHelper.OrderFlag)
            {
                await GetPriceToken();
                _logger.LogInformation("Everything goes well");
            }
        }
    }
    
    public async Task GetPriceToken()
    {
        string apiUrl = $"https://api.mexc.com/api/v3/ticker/price?symbol={CoinHelper.Coin}";

        while (true)
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var ticker = JsonSerializer.Deserialize<TickerPrice>(responseBody);
                CoinHelper.CurrentPrice = Decimal.Parse(ticker.price,
                    NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                
                Console.WriteLine($"Symbol: {ticker.symbol}, \nPrice: {ticker.price}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
            }
            catch (JsonException e)
            {
                Console.WriteLine("JSON parse error: " + e.Message);
            }
        }
    }
    
    public class TickerPrice
    {
        public string symbol { get; set; }
        public string price { get; set; }
    }
}

