FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/AdvertisingSystem.Identity.Api/AdvertisingSystem.Identity.Api.csproj", "src/AdvertisingSystem.Identity.Api/"]
COPY ["src/AdvertisingSystem.Identity.Application/AdvertisingSystem.Identity.Application.csproj", "src/AdvertisingSystem.Identity.Application/"]
COPY ["src/AdvertisingSystem.Identity.Contract/AdvertisingSystem.Identity.Contract.csproj", "src/AdvertisingSystem.Identity.Contract/"]
COPY ["src/AdvertisingSystem.Identity.Shared/AdvertisingSystem.Identity.Shared.csproj", "src/AdvertisingSystem.Identity.Shared/"]
COPY ["src/AdvertisingSystem.Identity.Domain/AdvertisingSystem.Identity.Domain.csproj", "src/AdvertisingSystem.Identity.Domain/"]
COPY ["src/AdvertisingSystem.Identity.Infrastructure/AdvertisingSystem.Identity.Infrastructure.csproj", "src/AdvertisingSystem.Identity.Infrastructure/"]
RUN dotnet restore "src/AdvertisingSystem.Identity.Api/AdvertisingSystem.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/AdvertisingSystem.Identity.Api"
RUN dotnet build "AdvertisingSystem.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdvertisingSystem.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdvertisingSystem.Identity.Api.dll"]