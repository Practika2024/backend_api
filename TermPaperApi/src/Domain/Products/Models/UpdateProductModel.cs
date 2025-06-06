﻿namespace Domain.Products.Models;

public class UpdateProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid ModifiedBy { get; set; }
    public Guid TypeId { get; set; }
    public List<ProductImage> Images { get; set; } = [];
}