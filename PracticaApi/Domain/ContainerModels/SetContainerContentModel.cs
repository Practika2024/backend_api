﻿namespace Domain.ContainerModels;

public class SetContainerContentModel
{
    public Guid ContainerId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid ModifiedBy { get; set; } 
}