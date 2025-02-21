using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerTypeModels;
using DataAccessLayer.Data;
using Domain.Containers;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerTypeRepository(ApplicationDbContext _context) : IContainerTypeQueries, IContainerTypeRepository
{
    public async Task<ContainerTypeEntity> Create(CreateContainerTypeModel model, CancellationToken cancellationToken)
    {
        var containerTypeEntity = ContainerTypeEntity.New(
            name: model.Name,
            createdBy: model.CreatedBy
        );

        await _context.ContainerTypes.AddAsync(containerTypeEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return containerTypeEntity;
    }

    public async Task<ContainerTypeEntity> Update(UpdateContainerTypeModel model, CancellationToken cancellationToken)
    {
        var containerTypeEntity = await GetContainerTypeAsync(x => x.Id == model.Id, cancellationToken);

        if (containerTypeEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        containerTypeEntity.Update(
            modifiedBy: model.ModifiedBy,
            name: model.Name
        );

        await _context.SaveChangesAsync(cancellationToken);

        return containerTypeEntity;
    }

    public async Task<ContainerTypeEntity> Delete(Guid id, CancellationToken cancellationToken)
    {
        var containerTypeEntity = await GetContainerTypeAsync(x => x.Id == id, cancellationToken);

        if (containerTypeEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        _context.ContainerTypes.Remove(containerTypeEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return containerTypeEntity;
    }

    public async Task<IReadOnlyList<ContainerTypeEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.ContainerTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerTypeEntity>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerTypeAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<ContainerTypeEntity>() : Option.Some(entity);
    }

    public async Task<Option<ContainerTypeEntity>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerTypeAsync(x => x.Name == name, cancellationToken);
        return entity == null ? Option.None<ContainerTypeEntity>() : Option.Some(entity);
    }

    private async Task<ContainerTypeEntity?> GetContainerTypeAsync(Expression<Func<ContainerTypeEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.ContainerTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await _context.ContainerTypes
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }



}