using DotnetSqlRdsProxy.Core.Models;
using DotnetSqlRdsProxy.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConnection();

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