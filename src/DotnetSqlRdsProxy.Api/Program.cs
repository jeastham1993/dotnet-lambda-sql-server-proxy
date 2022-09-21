using DotnetSqlRdsProxy.Core.Models;
using DotnetSqlRdsProxy.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Making test get request");

var client = new HttpClient();
var testGet = client.GetAsync("https://google.com").Result;

Console.WriteLine(testGet.StatusCode);

builder.Services.AddSecretsManagerSecrets();
builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
});

builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

var dbContext = app.Services.GetRequiredService<ProductDbContext>();

dbContext.Database.Migrate();

app.MapGet("/product/{productId}", async (IProductRepository repo, string productId) =>
{
    var product = await repo.Get(productId);

    return product;
});

app.Run();