using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSqlRdsProxy.Infrastructure;

public static class SecretsManagerExtensions
{
    public static IServiceCollection AddSecretsManagerSecrets(this IServiceCollection services)
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

        Console.WriteLine(parsedConnString.ToString());

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
        return $"Server={host};Database={dbname};User Id={username};Password={password};";
    }
}