using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models;

public class Product
{
    [BsonId]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    public string? Brand { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? ImageUrl { get; set; }
    public string? ProductUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}