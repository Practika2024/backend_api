using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Domain.Reminders;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;
public class ReminderRepository(ApplicationDbContext _context) : IReminderRepository, IReminderQueries
{
    public async Task<Reminder> Create(Reminder reminder, CancellationToken cancellationToken)
    {
        await _context.Reminders.AddAsync(reminder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return reminder;
    }

    public async Task<Reminder> Update(Reminder reminder, CancellationToken cancellationToken)
    {
        _context.Reminders.Update(reminder);
        await _context.SaveChangesAsync(cancellationToken);
        return reminder;
    }

    public async Task<Reminder> Delete(Reminder reminder, CancellationToken cancellationToken)
    {
        _context.Reminders.Remove(reminder);
        await _context.SaveChangesAsync(cancellationToken);
        return reminder;
    }

    public async Task<IReadOnlyList<Reminder>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Reminders
            .AsNoTracking()
            .Include(r => r.Container)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Reminder>> GetById(ReminderId id, CancellationToken cancellationToken)
    {
        var entity = await GetReminderAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<Reminder>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<Reminder>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken)
    {
        return await _context.Reminders
            .Where(r => r.ContainerId == containerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private async Task<Reminder?> GetReminderAsync(Expression<Func<Reminder, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.Reminders
                .AsNoTracking()
                .Include(r => r.Container)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        return await _context.Reminders
            .Include(r => r.Container)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}