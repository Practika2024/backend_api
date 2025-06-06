﻿namespace Domain.Products;

public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
}