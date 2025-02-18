﻿using Domain.Containers;
using Domain.Products;

namespace Application.Models.ContainerModels;

public class SetContainerContentModel
{
    public Guid ContainerId { get; set; }
    public Guid? ProductId { get; set; } 
    public bool IsEmpty { get; set; } 
    public Guid ModifiedBy { get; set; } 
}