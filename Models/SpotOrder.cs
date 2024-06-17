namespace KuCoinApp.Models;

public class SpotOrder
{
    public string symbol { get; set; }
    public string orderId { get; set; }
    public string orderListId { get; set; }
    public string price { get; set; }
    public string origQty { get; set; }
    public string type { get; set; }
}