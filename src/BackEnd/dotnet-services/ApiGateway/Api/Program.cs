using System.Text;
using Api.Interfaces;
using Api.Middlewares;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var elasticUrl = builder.Configuration["Elasticsearch:Url"];

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .Enrich.WithProcessName()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(
            new Uri(elasticUrl ??
                    throw new InvalidOperationException("Elasticsearch:Url config is null")))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "api-gateway-app-logs-{0:yyyy.MM.dd}",
            NumberOfShards = 1,
            NumberOfReplicas = 1
        })
    .CreateLogger();


builder.Host.UseSerilog();

#if DEBUG
builder.Configuration.AddJsonFile("ocelot.dev.json", optional: false, reloadOnChange: true);
#else
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
#endif

builder.Services.AddOcelot(builder.Configuration);

//var secretManager = builder.Services.BuildServiceProvider().GetRequiredService<ISecretManager>();
//var jwtSecretKey = await secretManager.ReadAsync("jwt-secret-key");

var jwtSecretKey = builder.Configuration["JWTSecretKey"];

builder. // Add authentication services
    Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ValidIssuer = "http://127.0.0.1:5004",
            /// ValidAudience = Configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };
    });

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMetricServer();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

///app.UseMiddleware<RequestIdMiddleware>();

app.UseAuthorization();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(c =>
{
    c.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
});

app.UseHttpMetrics();
await app.UseOcelot();
app.Run();