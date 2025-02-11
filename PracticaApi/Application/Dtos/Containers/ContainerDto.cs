using Application.Dtos.Products;
using Domain.Authentications.Users;
using Domain.Containers;
using Domain.ContainerHistories;
using Domain.Products;
using System.Collections.Generic;

namespace Application.Dtos.Containers
{
    public record ContainerDto
    {
        public Guid? Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Volume { get; init; }
        public string? Notes { get; init; }
        public string UniqueCode { get; init; }
        public Guid? TypeId { get; init; } = null!;
        public Guid? ContentId { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime? ModifiedAt { get; init; }
        public Guid? ModifiedBy { get; init; }
        public static ContainerDto FromDomainModel(ContainerEntity containerEntity)
        {
            return new ContainerDto
            {
                Id = containerEntity.Id.Value,
                Name = containerEntity.Name,
                Volume = containerEntity.Volume,
                Notes = containerEntity.Notes,
                UniqueCode = containerEntity.UniqueCode,
                TypeId = containerEntity.TypeId?.Value,
                ContentId = containerEntity.ContentId?.Value,
                CreatedAt = containerEntity.CreatedAt,
                CreatedBy = containerEntity.CreatedBy.Value,
                ModifiedAt = containerEntity.ModifiedAt,
                ModifiedBy = containerEntity.ModifiedBy?.Value,
            };
        }
    }
}