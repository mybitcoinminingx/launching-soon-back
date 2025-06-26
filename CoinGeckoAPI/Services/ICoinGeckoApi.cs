using CoinGeckoAPI.Models;
using Refit;

namespace CoinGeckoAPI.Services;

public interface ICoinGeckoApi
{
    [Get("/coins/markets?vs_currency=usd&ids=bitcoin")]
    Task<List<BitcoinMarketData>> GetBitcoinMarketData();
}
