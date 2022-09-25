using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using DotnetSqlRdsProxy.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
serviceCollection.AddDatabaseConnection();
var serviceProvider = serviceCollection.BuildServiceProvider();

var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

var handler = async (string evt, ILambdaContext context) =>
{
    context.Logger.LogLine("Applying database migrations");

    await dbContext.Database.MigrateAsync();

    context.Logger.LogLine("Database migrated");
};

await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync();