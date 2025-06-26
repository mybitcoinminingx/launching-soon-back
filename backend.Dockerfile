# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем весь код. Внутри контейнера создастся папка /src/CoinGeckoAPI/
COPY ./ ./

# !! ВАЖНОЕ ИЗМЕНЕНИЕ !!
# Переходим в папку, где лежит файл проекта (.csproj), перед запуском команд dotnet
WORKDIR /src/CoinGeckoAPI

# Теперь команды найдут файл проекта
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Этап 2: Создание образа для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CoinGeckoAPI.dll"]