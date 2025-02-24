using AdvertisingSystem.Identity.Api.Middlewares;
using AdvertisingSystem.Identity.Application;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddApplicationServices(builder.Configuration);

services.Configure<ApiBehaviorOptions>(
    options => { options.SuppressModelStateInvalidFilter = true; }
);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new
        ElasticsearchSinkOptions(
            new Uri(builder.Configuration["Elasticsearch:Url"] ?? throw new Exception("elastic search address is empty")
            ))
        {
            AutoRegisterTemplate = true,
            IndexFormat = "user-logs-{0:yyyy.MM.dd}",
            FailureCallback = e => Console.WriteLine("Elasticsearch logging failed: ", e.Exception?.Message),
        })
    .Enrich.FromLogContext()
    .CreateLogger();


builder.Host.UseSerilog();

var kestrelUrl = builder.Configuration.GetValue<string>("Kestrel:Endpoints:Http:Url");

Log.Information("Application started on {KestrelUrl}", kestrelUrl);

services.AddMvc(opt => { opt.Filters.Add<ValidationActionFilter>(); });
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.UseErrorHandlingMiddleware();
app.Run();