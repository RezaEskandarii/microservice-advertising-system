FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY *.csproj ./

COPY ./AdvertisingSystem.Api.csproj src/AdvertisingSystem.Api/
#COPY ../AdvertisingSystem.Api/AdvertisingSystem.Api.csproj src/AdvertisingSystem.Api/
#COPY ../AdvertisingSystem.Application/AdvertisingSystem.Application.csproj src/AdvertisingSystem.Application/
#COPY ../AdvertisingSystem.Domain/AdvertisingSystem.Domain.csproj src/AdvertisingSystem.Domain/
#COPY ../AdvertisingSystem.Infrastructure/AdvertisingSystem.Infrastructure.csproj src/AdvertisingSystem.Infrastructure/
#COPY ../AdvertisingSystem.Contract/* src/AdvertisingSystem.Contract/
RUN dotnet restore "src/AdvertisingSystem.Api/AdvertisingSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/AdvertisingSystem.Api"
RUN dotnet build "AdvertisingSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdvertisingSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdvertisingSystem.Api.dll"]
