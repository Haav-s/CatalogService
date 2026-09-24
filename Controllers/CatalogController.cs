using Microsoft.AspNetCore.Mvc;
using Models;

namespace CatalogService.Controllers;

[ApiController]
[Route("product")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;

    private static readonly Product[] _products =
    {
        new Product
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Name = "Laptop",
            Description = "A laptop computer",
            Price = 7999,
            Brand = "Lenovo",
            Manufacturer = "Lenovo",
            Model = "ThinkPad",
            ReleaseDate = DateTime.Now
        },

        new Product
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Name = "Phone",
            Description = "A smartphone",
            Price = 4999,
            Brand = "Samsung",
            Manufacturer = "Samsung",
            Model = "Galaxy",
            ReleaseDate = DateTime.Now
        },

        new Product
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            Name = "Headphones",
            Description = "Wireless headphones",
            Price = 999,
            Brand = "Sony",
            Manufacturer = "Sony",
            Model = "WH",
            ReleaseDate = DateTime.Now
        }
    };

    public CatalogController(ILogger<CatalogController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    [HttpGet("{productId}", Name = "GetProductById")]
    public ActionResult<Product> Get(Guid productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}