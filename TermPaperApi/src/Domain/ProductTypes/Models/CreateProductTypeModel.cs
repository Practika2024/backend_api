﻿namespace Domain.ProductTypes.Models;

public class CreateProductTypeModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
}