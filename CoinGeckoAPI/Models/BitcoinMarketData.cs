using System.Text.Json.Serialization;

namespace CoinGeckoAPI.Models;

public class BitcoinMarketData
{
    [JsonPropertyName("current_price")]
    public decimal CurrentPrice { get; set; }

    [JsonPropertyName("price_change_percentage_24h")]
    public decimal PriceChangePercentage24h { get; set; }
}
