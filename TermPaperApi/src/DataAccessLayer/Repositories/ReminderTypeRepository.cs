using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Extensions;
using Domain.ReminderTypes.Models;
using Microsoft.EntityFrameworkCore;
using Optional;
using ReminderType = Domain.ReminderTypes.ReminderType;

namespace DataAccessLayer.Repositories;

public class ReminderTypeRepository(ApplicationDbContext context, IMapper mapper)
    : IReminderTypeRepository, IReminderTypeQueries
{
    public async Task<ReminderType> Create(CreateReminderTypeModel model, CancellationToken cancellationToken)
    {
        var reminderTypeEntity = mapper.Map<ReminderTypeEntity>(model);

        await context.ReminderTypes.AddAuditableAsync(reminderTypeEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReminderType>(reminderTypeEntity);
    }
    public async Task<ReminderType> Update(UpdateReminderTypeModel model, CancellationToken cancellationToken)
    {
        var reminderTypeEntity = await GetReminderTypeAsync(x => x.Id == model.Id, cancellationToken);

        reminderTypeEntity = mapper.Map(model, reminderTypeEntity);
        
        context.ReminderTypes.UpdateAuditable(reminderTypeEntity!);

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReminderType>(reminderTypeEntity);
    }

    public async Task<ReminderType> Delete(int id, CancellationToken cancellationToken)
    {
        var reminderTypeEntity = await GetReminderTypeAsync(x => x.Id == id, cancellationToken);

        if (reminderTypeEntity == null)
        {
            throw new InvalidOperationException("Product type not found.");
        }

        context.ReminderTypes.Remove(reminderTypeEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReminderType>(reminderTypeEntity);
    }

    public async Task<IReadOnlyList<ReminderType>> GetAll(CancellationToken cancellationToken)
    {
        var reminderTypesEntity = await context.ReminderTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<ReminderType>>(reminderTypesEntity);
    }

    public async Task<Option<ReminderType>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await GetReminderTypeAsync(x => x.Id == id, cancellationToken);
        var reminderType = mapper.Map<ReminderType>(entity);
        return reminderType == null ? Option.None<ReminderType>() : Option.Some(reminderType);
    }

    public async Task<Option<ReminderType>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetReminderTypeAsync(x => x.Name == name, cancellationToken);
        var reminderType = mapper.Map<ReminderType>(entity);
        return reminderType == null ? Option.None<ReminderType>() : Option.Some(reminderType);
    }

    private async Task<ReminderTypeEntity?> GetReminderTypeAsync(Expression<Func<ReminderTypeEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.ReminderTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.ReminderTypes
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
