FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /apigatewayapp
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /apigatewayapp/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /apigatewayapp/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /apigatewayapp
COPY --from=publish /apigatewayapp/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
