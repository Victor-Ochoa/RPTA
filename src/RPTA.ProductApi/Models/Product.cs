﻿using System.Text.Json.Serialization;

namespace RPTA.ProductApi.Models;

public record Product
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
