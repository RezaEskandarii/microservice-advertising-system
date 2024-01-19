FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY  src/AdvertisingSystem.Api/AdvertisingSystem.Api.csproj src/AdvertisingSystem.Api/ 
COPY  src/AdvertisingSystem.Application/AdvertisingSystem.Application.csproj src/AdvertisingSystem.Application/
COPY  src/AdvertisingSystem.Domain/AdvertisingSystem.Domain.csproj src/AdvertisingSystem.Domain/
COPY  src/AdvertisingSystem.Infrastructure/AdvertisingSystem.Infrastructure.csproj src/AdvertisingSystem.Infrastructure/
COPY  src/AdvertisingSystem.Contract/AdvertisingSystem.Contract.csproj src/AdvertisingSystem.Contract/
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
