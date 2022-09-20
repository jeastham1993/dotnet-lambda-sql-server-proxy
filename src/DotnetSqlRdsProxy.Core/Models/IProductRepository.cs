namespace DotnetSqlRdsProxy.Core.Models;

public interface IProductRepository
{
    Task Add(Product product);

    Task<Product> Get(string productId);
}