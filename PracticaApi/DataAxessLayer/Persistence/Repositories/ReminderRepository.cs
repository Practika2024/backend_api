using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ReminderModels;
using Domain.Containers;
using Domain.Reminders;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories
{
    public class ReminderRepository(ApplicationDbContext _context) : IReminderRepository, IReminderQueries
    {
        public async Task<ReminderEntity> Create(CreateReminderModel model, CancellationToken cancellationToken)
        {
            var reminderEntity = ReminderEntity.New(
                id: model.Id,
                containerId: model.ContainerId,
                title: model.Title,
                dueDate: model.DueDate,
                type: model.Type,
                createdBy: model.CreatedBy
            );

            await _context.Reminders.AddAsync(reminderEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return reminderEntity;
        }

        public async Task<ReminderEntity> Update(UpdateReminderModel model, CancellationToken cancellationToken)
        {
            var reminderEntity = await GetReminderAsync(x => x.Id == model.Id, cancellationToken);

            if (reminderEntity == null)
            {
                throw new InvalidOperationException("Reminder not found.");
            }

            reminderEntity.Update(
                title: model.Title,
                dueDate: model.DueDate,
                type: model.Type,
                modifiedBy: model.ModifiedBy
            );

            await _context.SaveChangesAsync(cancellationToken);

            return reminderEntity;
        }

        public async Task<ReminderEntity> Delete(DeleteReminderModel model, CancellationToken cancellationToken)
        {
            var reminderEntity = await GetReminderAsync(x => x.Id == model.Id, cancellationToken);

            if (reminderEntity == null)
            {
                throw new InvalidOperationException("Reminder not found.");
            }

            _context.Reminders.Remove(reminderEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return reminderEntity;
        }

        public async Task<IReadOnlyList<ReminderEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Reminders
                .AsNoTracking()
                .Include(r => r.Container)
                .ToListAsync(cancellationToken);
        }

        public async Task<Option<ReminderEntity>> GetById(ReminderId id, CancellationToken cancellationToken)
        {
            var entity = await GetReminderAsync(x => x.Id == id, cancellationToken);
            return entity == null ? Option.None<ReminderEntity>() : Option.Some(entity);
        }

        public async Task<IReadOnlyList<ReminderEntity>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken)
        {
            return await _context.Reminders
                .Where(r => r.ContainerId == containerId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        private async Task<ReminderEntity?> GetReminderAsync(Expression<Func<ReminderEntity, bool>> predicate, CancellationToken cancellationToken,
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
}