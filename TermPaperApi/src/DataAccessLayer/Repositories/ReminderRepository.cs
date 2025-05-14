using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Extensions;
using Domain.Reminders;
using Domain.Reminders.Models;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ReminderRepository(ApplicationDbContext context, IMapper mapper) : IReminderRepository, IReminderQueries
{
    public async Task<Reminder> Create(CreateReminderModel model, CancellationToken cancellationToken)
    {
        var reminderEntity = mapper.Map<ReminderEntity>(model);

        await context.Reminders.AddAuditableAsync(reminderEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Reminder>(reminderEntity);
    }

    public async Task<Reminder> Update(UpdateReminderModel model, CancellationToken cancellationToken)
    {
        var reminderEntity = await GetReminderAsync(x => x.Id == model.Id, cancellationToken);

       mapper.Map(model, reminderEntity);

        context.Reminders.UpdateAuditable(reminderEntity!);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Reminder>(reminderEntity);
    }

    public async Task<Reminder> Delete(DeleteReminderModel model, CancellationToken cancellationToken)
    {
        var reminderEntity = await GetReminderAsync(x => x.Id == model.Id, cancellationToken);

        if (reminderEntity == null)
        {
            throw new InvalidOperationException("Reminder not found.");
        }

        context.Reminders.Remove(reminderEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Reminder>(reminderEntity);
    }

    public async Task<IReadOnlyList<Reminder>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await context.Reminders
            .AsNoTracking()
            .Include(r => r.Container)
            .ToListAsync(cancellationToken);
        
        return mapper.Map<IReadOnlyList<Reminder>>(entities);
    }

    public async Task<Option<Reminder>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetReminderAsync(x => x.Id == id, cancellationToken);

        var reminder = mapper.Map<Reminder>(entity);

        return reminder == null ? Option.None<Reminder>() : Option.Some(reminder);
    }

    public async Task<Option<IReadOnlyList<Reminder>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var entity = await context.Reminders.Where(r => r.CreatedBy == userId)
            .AsNoTracking()
            .Include(r => r.Container)
            .ToListAsync(cancellationToken);

        var reminder = mapper.Map<IReadOnlyList<Reminder>>(entity);

        return reminder == null ? Option.None<IReadOnlyList<Reminder>>() : Option.Some(reminder);
    }

    public async Task<IReadOnlyList<Reminder>> GetByContainerId(Guid containerId, CancellationToken cancellationToken)
    {
        var entities = await context.Reminders
            .Where(r => r.ContainerId == containerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return mapper.Map<IReadOnlyList<Reminder>>(entities);
    }

    private async Task<ReminderEntity?> GetReminderAsync(Expression<Func<ReminderEntity, bool>> predicate,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.Reminders
                .AsNoTracking()
                .Include(r => r.Container)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Reminders
            .Include(r => r.Container)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}