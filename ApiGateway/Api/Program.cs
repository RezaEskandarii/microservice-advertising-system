using System.Text;
using Api.Interfaces;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISecretManager, SecretManager>();

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