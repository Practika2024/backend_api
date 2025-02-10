﻿using Domain.Authentications.Users;
using Domain.Products;

namespace Application.Models.ProductModels;

public class UpdateProductModel
{
    public ProductId Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public UserId ModifiedBy { get; set; }
    public ProductTypeId TypeId { get; set; }
}