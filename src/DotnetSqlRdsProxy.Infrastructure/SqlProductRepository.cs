using DotnetSqlRdsProxy.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSqlRdsProxy.Infrastructure;

public class SqlProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public SqlProductRepository(ProductDbContext context)
    {
        _context = context;
    }
    
    public async Task Add(Product product)
    {
        await this._context.Product.AddAsync(product);

        await this._context.SaveChangesAsync();
    }

    public async Task<Product> Get(string productId)
    {
        var product = await this._context.Product.FirstOrDefaultAsync(p => p.ProductId == productId);

        return product;
    }
}