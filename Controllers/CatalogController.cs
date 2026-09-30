using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories;

namespace CatalogService.Controllers;

[ApiController]
[Route("product")]
public class CatalogController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{productId}", Name = "GetProductById")]
    public async Task<ActionResult<Product>> Get(Guid productId)
    {
        var product = await _repository.GetByIdAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
    [HttpGet("version")]
            public async Task<Dictionary<string,string>> GetVersion()
            {
                var properties = new Dictionary<string, string>();
                var assembly = typeof(Program).Assembly;
                properties.Add("service", "HaaV Catalog Service"); // eller "HaaV Catalog Service
                var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
                    .Assembly.Location).ProductVersion;
                properties.Add("version", ver!);
                try {
                    var hostName = System.Net.Dns.GetHostName();
                    var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
                    var ipa = ips.First().MapToIPv4().ToString();
                    properties.Add("hosted-at-address", ipa);
                } catch (Exception ex) {
                    _logger.LogError(ex.Message);
                    properties.Add("hosted-at-address", "Could not resolve IP-address");
                }
                return properties;
            }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        if (product.Id == Guid.Empty)
        {
            product.Id = Guid.NewGuid();
        }

        await _repository.CreateAsync(product);
        return CreatedAtRoute("GetProductById", new { productId = product.Id }, product);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> Update(Guid productId, Product product)
    {
        var updated = await _repository.UpdateAsync(productId, product);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var deleted = await _repository.DeleteAsync(productId);
        return deleted ? NoContent() : NotFound();
    }
}