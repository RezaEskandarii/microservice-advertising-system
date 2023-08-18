using AdvertisingSystem.Identity.Api.Middlewares;
using AdvertisingSystem.Identity.Application;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddApplicationServices(builder.Configuration);

services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });


services.AddMvc(opt => { opt.Filters.Add<ValidationActionFilter>(); });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseErrorHandlingMiddleware();
app.Run();