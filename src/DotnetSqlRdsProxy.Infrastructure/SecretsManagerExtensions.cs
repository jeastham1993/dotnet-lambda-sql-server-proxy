using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSqlRdsProxy.Infrastructure;

public static class SecretsManagerExtensions
{
    public static IServiceCollection AddSecretsManagerSecrets(this IServiceCollection services)
    {
        Console.WriteLine("Attempting to retrieve secrets");
        
        var secretName = Environment.GetEnvironmentVariable("SECRET_NAME") ?? "test/proxy-db";
        var region = "us-east-2";

        var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        GetSecretValueRequest request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.
        
        Console.WriteLine("Making request");
        
        var connString = client.GetSecretValueAsync(request).Result;
        
        Console.WriteLine("Request complete");

        var parsedConnString = JsonSerializer.Deserialize<SecretManagerConnectionString>(connString.SecretString);

        Environment.SetEnvironmentVariable("SQL_CONNECTION_STRING", parsedConnString.ToString());

        return services;
    }
}

record SecretManagerConnectionString
{
    public string username { get; set; }
    
    public string password { get; set; }
    
    public string host { get; set; }
    
    public string dbname { get; set; }

    public override string ToString()
    {
        return $"Server={host};Database={host};User Id={username};Password={password};";
    }
}