using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DotnetSqlRdsProxy.Infrastructure;

public class EntityFrameworkInterceptor : IDbConnectionInterceptor
{
    public InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");

        Console.WriteLine($"Generating auth token {host}::{dbName}::{user}");

        var authToken =
            Amazon.RDS.Util.RDSAuthTokenGenerator.GenerateAuthToken(
                host, 1433, user);
        
        ((SqlConnection)connection).AccessToken = authToken;

        return result;
    }

    public async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return result;
    }

    public void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
    }

    public async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
    }

    public InterceptionResult ConnectionClosing(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        return result;
    }

    public async ValueTask<InterceptionResult> ConnectionClosingAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        return result;
    }

    public void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
    {
    }

    public async Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
    {
    }

    public void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
    {
    }

    public async Task ConnectionFailedAsync(DbConnection connection, ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = new CancellationToken())
    {
    }
}