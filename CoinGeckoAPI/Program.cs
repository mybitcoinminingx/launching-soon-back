using CoinGeckoAPI.Models;
using CoinGeckoAPI.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// *** НАЧАЛО ИЗМЕНЕНИЙ: ДОБАВЛЕНИЕ CORS ***
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000") // URL вашего фронтенда
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// *** КОНЕЦ ИЗМЕНЕНИЙ ***


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Добавляем Refit клиент в контейнер зависимостей
builder.Services.AddRefitClient<ICoinGeckoApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["CoinGecko:BaseUrl"]!);
        c.DefaultRequestHeaders.Add("User-Agent", "MyBitcoinMiningApp/1.0"); // Добавляем User-Agent
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// *** НАЧАЛО ИЗМЕНЕНИЙ: ПРИМЕНЕНИЕ CORS ***
app.UseCors(MyAllowSpecificOrigins);
// *** КОНЕЦ ИЗМЕНЕНИЙ ***

// 2. Определяем наш API эндпоинт
app.MapGet("/api/bitcoin-market-data", async (ICoinGeckoApi coinGeckoApi) =>
{
    try
    {
        var result = await coinGeckoApi.GetBitcoinMarketData();
        if (result == null || !result.Any())
        {
            return Results.NotFound("Bitcoin market data not found.");
        }
        return Results.Ok(result.First());
    }
    catch (ApiException ex)
    {
        // Обрабатываем ошибки от API CoinGecko
        return Results.Problem($"Error fetching data from CoinGecko: {ex.Message}", statusCode: (int?)ex.StatusCode);
    }
    catch (Exception ex)
    {
        // Обрабатываем другие возможные ошибки
        return Results.Problem($"An unexpected error occurred: {ex.Message}", statusCode: 500);
    }
})
.WithName("GetBitcoinMarketData")
.WithOpenApi();

app.Run();