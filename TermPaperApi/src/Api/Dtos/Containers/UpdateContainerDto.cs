﻿namespace Api.Dtos.Containers;

public class UpdateContainerDto
{
    public string Name { get; set; } = null!; 
    public decimal Volume { get; set; } 
    public string? Notes { get; set; }
}