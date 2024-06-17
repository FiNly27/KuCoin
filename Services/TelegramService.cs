using KuCoinApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TL;
using WTelegram;

namespace KuCoinApp.Services;

public class TelegramService : BackgroundService
{
    private int _minute = -1;
    private readonly ILogger<TelegramService> _logger;
    private readonly MexcService _service = new();
    
    public TelegramService(ILogger<TelegramService> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Hosted Service running.");
        var i = 1;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (CheckTime())
            {
                if (i == 1)
                {
                    await GetMessage();
                    i = 2;
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }

    private async Task GetMessage()
    {
        using var client = new Client(YourConfig);
        var user = await client.LoginUserIfNeeded();
        char[] delimiters = [' ', '\r', '\n'];
        _logger.LogInformation($"We are logged in as {user.username ?? user.first_name + " " + user.last_name}");

        // Получение диалогов
        var dialogs = await client.Messages_GetAllDialogs();
        
        var chat = dialogs.chats.Values.OfType<Channel>()
            .FirstOrDefault(x=>x.Title == "Kucoin Signals Bybit Pumps") ??
                    throw new NullReferenceException("Channel cannot be null");
        
        _logger.LogInformation($"Channel: {chat.Title}");

        // Получение последних сообщений из канала
        var messages = await client.Messages_GetHistory(chat, limit: 1);
        var message = messages.Messages.OfType<Message>().FirstOrDefault()
                      ?? throw new NullReferenceException("Message cannot be null");
        
        
        CoinHelper.Coin = $"BTCUSDT";
        CoinHelper.OrderFlag = true;
        CoinHelper.Growth = 1;
        CoinHelper.QuoteOrderQty = 100;
        await _service.Asd();
        

        if (message.message.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length == 1)
        {
            CoinHelper.Coin = $"BTCUSDT";
            CoinHelper.OrderFlag = true;
            CoinHelper.Growth = 1;
            CoinHelper.QuoteOrderQty = 100;
        }

        _logger.LogInformation(message.message.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length == 1
            ? $"Message: {message.message}"
            : $"Message is not correct: {message.message}");
    }

    private static string? YourConfig(string what)
    {
        // Supply the necessary values here or fetch them from a config file or environment variables
        switch (what)
        {
            case "api_id": return "27400841";
            case "api_hash": return "b8ff23446c9f8723a19c84d536e12939";
            case "phone_number": return "+375295836320";
            case "verification_code": Console.Write("Code: "); return Console.ReadLine();
            case "password": return "LfybrAby27vfz2001";
            default: return null;
        }
    }

    private bool CheckTime()
    {
        var minute = DateTimeOffset.Now.Minute;

        if (_minute == minute) return false;
        _minute = minute;
        return true;

    }
}