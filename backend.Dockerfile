# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./ ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Этап 2: Создание образа для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Внутри контейнера приложение будет работать на стандартном порту 8080
ENTRYPOINT ["dotnet", "CoinGeckoAPI.dll"]