FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /advertisementmanagementapp
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AdvertisingSystem.Api/AdvertisingSystem.Api.csproj", "src/AdvertisingSystem.Api/"]
COPY ["src/AdvertisingSystem.Application/AdvertisingSystem.Application.csproj", "src/AdvertisingSystem.Application/"]
COPY ["src/AdvertisingSystem.Domain/AdvertisingSystem.Domain.csproj", "src/AdvertisingSystem.Domain/"]
COPY ["src/AdvertisingSystem.Infrastructure/AdvertisingSystem.Infrastructure.csproj", "src/AdvertisingSystem.Infrastructure/"]
COPY ["src/AdvertisingSystem.Contract/AdvertisingSystem.Contract.csproj", "src/AdvertisingSystem.Contract/"]
RUN dotnet restore "src/AdvertisingSystem.Api/AdvertisingSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/AdvertisingSystem.Api"
RUN dotnet build "AdvertisingSystem.Api.csproj" -c Release -o /advertisementmanagementapp/build

FROM build AS publish
RUN dotnet publish "AdvertisingSystem.Api.csproj" -c Release -o /advertisementmanagementapp/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /advertisementmanagementapp
COPY --from=publish /advertisementmanagementapp/publish .
ENTRYPOINT ["dotnet", "AdvertisingSystem.Api.dll"]
