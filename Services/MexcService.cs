using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CryptoExchange.Net.Authentication;
using KuCoinApp.Models;
using Mexc.Net.Clients;
using Mexc.Net.Objects.Models.Spot;

namespace KuCoinApp.Services;

public class MexcService
{
    private string apiKey = "mx0vglgQmhRlHcYL7U";
    private string apiSecret = "9eb8ba350cba4065b293d0e6dd4f5c71";
    private string apiUrl = "https://api.mexc.com";

    public MexcService()
    {
        
    }




    public async Task PlaceOrder(MexcRestClient client)
    {
        var result = await client.SpotApi.Trading.PlaceOrderAsync(
            "BTCUSDT",
            Mexc.Net.Enums.OrderSide.Buy,
            Mexc.Net.Enums.OrderType.Market,
            quoteQuantity: 6);
        CoinHelper.OrderId = result.Data.OrderId;
        CoinHelper.CurrentPrice = result.Data.Price;
        CoinHelper.SellPrice = CoinHelper.CurrentPrice * 2;
    }

    public async Task GetActiveOrder(MexcRestClient client)
    {
        var result = await client.SpotApi.Trading.GetOrdersAsync("ADAXUSDT");
        CoinHelper.OrderId = result.Data.FirstOrDefault().OrderId;
        CoinHelper.Quantity = result.Data.FirstOrDefault().Quantity;
    }

    public async Task CancelOrder(MexcRestClient client)
    {
        var result = await client.SpotApi.Trading.PlaceOrderAsync(
            "USDTADAX",
            Mexc.Net.Enums.OrderSide.Sell,
            Mexc.Net.Enums.OrderType.Market,
            CoinHelper.Quantity);
        CoinHelper.OrderId = result.Data.OrderId;
        CoinHelper.CurrentPrice = result.Data.Price;
        CoinHelper.SellPrice = CoinHelper.CurrentPrice * 2;
    }

    public async Task Asd()
    {
        var restClient = new MexcRestClient();
        var tickerResult = await restClient.SpotApi.ExchangeData.GetTickerAsync("ADAUSDT");
        var lastPrice = tickerResult.Data.LastPrice;
        Console.WriteLine(lastPrice);
        var result = await restClient.SpotApi.Account.GetAccountInfoAsync();
        Console.WriteLine(result.Data.Balances.FirstOrDefault(x=>x.Asset == "USDT").Available);
        await PlaceOrder(restClient);
        await GetActiveOrder(restClient);
        await CancelOrder(restClient);
    }
}

