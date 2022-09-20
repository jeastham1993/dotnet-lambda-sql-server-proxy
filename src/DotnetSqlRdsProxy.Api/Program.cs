using DotnetSqlRdsProxy.Core.Models;
using DotnetSqlRdsProxy.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSecretsManagerSecrets();
builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
});

builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.MapGet("/product/{productId}", async (IProductRepository repo, string productId) =>
{
    var product = await repo.Get(productId);

    return product;
});

app.Run();