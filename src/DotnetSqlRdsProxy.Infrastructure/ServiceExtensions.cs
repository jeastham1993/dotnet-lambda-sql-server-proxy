using System.Text.Json;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSqlRdsProxy.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabaseConnection(this IServiceCollection services)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");

        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = host;
        builder.InitialCatalog = dbName;
        builder.ConnectTimeout = 30;

        var connection = new SqlConnection(builder.ConnectionString);

        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseSqlServer(connection)
                .AddInterceptors(new EntityFrameworkInterceptor());
        });

        return services;
    }
    
    public static IServiceCollection AddDatabaseConnectionFromSecrets(this IServiceCollection services)
    {
        var secretName = Environment.GetEnvironmentVariable("SECRET_NAME") ?? "test/proxy-db";
        
        Console.WriteLine($"Attempting to retrieve secret named {secretName}");

        var client = new AmazonSecretsManagerClient(new EnvironmentVariablesAWSCredentials());

        GetSecretValueRequest request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.
        
        Console.WriteLine("Making request");
        
        var connString = client.GetSecretValueAsync(request).Result;
        
        Console.WriteLine("Request complete");

        var parsedConnString = JsonSerializer.Deserialize<SecretManagerConnectionString>(connString.SecretString);
        
        services.AddDbContext<ProductDbContext>(options => { options.UseSqlServer(parsedConnString.ToString()); });

        return services;
    }
    
    record SecretManagerConnectionString
    {
        public string username { get; set; }
    
        public string password { get; set; }
    
        public string host { get; set; }
    
        public string dbname { get; set; }

        public override string ToString()
        {
            return $"Server={host};Database={dbname};User Id={username};Password={password};";
        }
    }
}