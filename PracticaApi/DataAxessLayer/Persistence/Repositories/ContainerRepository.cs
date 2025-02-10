using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models;
using Application.Models.ContainerModels;
using Domain.Containers;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ContainerRepository(ApplicationDbContext _context) : IContainerRepository, IContainerQueries
{
    public async Task<ContainerEntity> Create(CreateContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = ContainerEntity.New(
            id: model.Id,
            name: model.Name,
            volume: model.Volume,
            notes: model.Notes,
            createdBy: model.CreatedBy,
            typeId: model.TypeId,
            uniqueCode: model.UniqueCode
        );

        await _context.Containers.AddAsync(containerEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return containerEntity;
    }

    public async Task<ContainerEntity> Update(UpdateContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.Id, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        containerEntity.Update(
            name: model.Name,
            volume: model.Volume,
            notes: model.Notes,
            modifiedBy: model.ModifiedBy,
            typeId: model.TypeId,
            uniqueCode: model.UniqueCode
        );

        await _context.SaveChangesAsync(cancellationToken);

        return containerEntity;
    }

    public async Task<ContainerEntity> Delete(DeleteContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.Id, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        _context.Containers.Remove(containerEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return containerEntity;
    }

    public async Task<IReadOnlyList<ContainerEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Containers
            .AsNoTracking()
            .Include(c => c.Content)
            .Include(c => c.CreatedByNavigation)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerEntity>> GetById(ContainerId id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<ContainerEntity>() : Option.Some(entity);
    }

    public async Task<Option<ContainerEntity>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Name == name, cancellationToken);
        return entity == null ? Option.None<ContainerEntity>() : Option.Some(entity);
    }

    public async Task<Option<ContainerEntity>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken)
    {
        var container = await _context.Containers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UniqueCode == uniqueCode, cancellationToken);

        return container == null ? Option.None<ContainerEntity>() : Option.Some(container);
    }

    private async Task<ContainerEntity?> GetContainerAsync(Expression<Func<ContainerEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.Containers
                .AsNoTracking()
                .Include(c => c.Content)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await _context.Containers
            .Include(c => c.Content)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}