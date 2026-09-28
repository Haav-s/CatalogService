using Models;
using MongoDB.Driver;

namespace Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MONGODB_CONNECTION_STRING"]
            ?? throw new InvalidOperationException("MONGODB_CONNECTION_STRING mangler");
        var databaseName = configuration["MONGODB_DATABASE"]
            ?? throw new InvalidOperationException("MONGODB_DATABASE mangler");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _products = database.GetCollection<Product>("Products");
    }

    public async Task<List<Product>> GetAllAsync() =>
        await _products.Find(_ => true).ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _products.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product product) =>
        await _products.InsertOneAsync(product);

    public async Task<bool> UpdateAsync(Guid id, Product product)
    {
        product.Id = id;
        var result = await _products.ReplaceOneAsync(p => p.Id == id, product);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _products.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }
}