using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using DataAccessLayer.Data;
using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

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
            //modifiedBy: model.ModifiedBy,
            typeId: model.TypeId
            // ,uniqueCode: model.UniqueCode
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
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerEntity>> GetById(Guid id, CancellationToken cancellationToken)
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
    
    public async Task<ContainerEntity> SetContainerContent(SetContainerContentModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.ContainerId, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        var contentEntity = ContainerContentEntity.New(
            productId: model.ProductId,
            isEmpty: model.IsEmpty,
            createdBy: model.ModifiedBy
        );

        containerEntity.SetContent(contentEntity, model.ModifiedBy);

        await _context.SaveChangesAsync(cancellationToken);

        return containerEntity;
    }
    
    public async Task<ContainerEntity> ClearContainerContent(ClearContainerContentModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.ContainerId, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        containerEntity.ClearContent(model.ModifiedBy);

        await _context.SaveChangesAsync(cancellationToken);

        return containerEntity;
    }

    public async Task<int> GetLastSequenceForPrefixAsync(string codePrefix, CancellationToken cancellationToken)
    {
        var containers = await _context.Containers
            .Where(c => c.UniqueCode.StartsWith(codePrefix))
            .ToListAsync(cancellationToken);

        if (!containers.Any())
        {
            return 0;
        }
        
        var maxSequence = containers
            .Select(c =>
            {
                var numericPart = c.UniqueCode.Substring(codePrefix.Length);
                return int.TryParse(numericPart, out int number) ? number : 0;
            })
            .Max();

        return maxSequence;
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