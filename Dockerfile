#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Telegram_V2.sln", "."]
COPY ["Telegram_V2.Application/Telegram_V2.Application.csproj", "Telegram_V2.Application/"]
COPY ["Telegram_V2.Core/Telegram_V2.Core.csproj", "Telegram_V2.Core/"]
COPY ["Telegram_V2.Infrastructure/Telegram_V2.Infrastructure.csproj", "Telegram_V2.Infrastructure/"]
COPY ["Telegram_V2.Web/Telegram_V2.Web.csproj", "Telegram_V2.Web/"]
RUN dotnet restore "./Telegram_V2.sln"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Telegram_V2.Web/Telegram_V2.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Telegram_V2.Web/Telegram_V2.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telegram_V2.Web.dll"]