using CoinGeckoAPI.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// *** НАЧАЛО ИЗМЕНЕНИЙ: ИСПРАВЛЕНИЕ CORS v2 ***test
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // Читаем origins из конфигурации как единую строку
            var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Value;

            // Проверяем, что строка не пустая, и делим ее по запятой
            if (!string.IsNullOrEmpty(origins))
            {
                var allowedOrigins = origins.Split(',', StringSplitOptions.RemoveEmptyEntries);
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
});
// *** КОНЕЦ ИЗМЕНЕНИЙ ***


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRefitClient<ICoinGeckoApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["CoinGecko:BaseUrl"]!);
        c.DefaultRequestHeaders.Add("User-Agent", "MyBitcoinMiningApp/1.0");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Применяем политику CORS
app.UseCors(MyAllowSpecificOrigins);

// Определяем наш API эндпоинт
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
        return Results.Problem($"Error fetching data from CoinGecko: {ex.Message}", statusCode: (int?)ex.StatusCode);
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}", statusCode: 500);
    }
})
.WithName("GetBitcoinMarketData")
.WithOpenApi();

app.Run();