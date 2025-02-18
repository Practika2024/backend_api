using Domain.Interfaces;
using System;
using Domain.Abstractions;

namespace Domain.Containers
{
    public class ContainerTypeEntity : AuditableEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private ContainerTypeEntity(Guid id, string name, Guid createdBy)
            : base(createdBy)
        {
            Id = id;
            Name = name;
        }

        public static ContainerTypeEntity New(string name, Guid createdBy)
            => new ContainerTypeEntity(Guid.NewGuid(), name, createdBy);

        public void UpdateName(string newName)
        {
            Name = newName;
        }
    }
}