using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerHistoryModels;
using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories
{
    public class ContainerHistoryRepository(ApplicationDbContext _context) : IContainerHistoryRepository, IContainerHistoryQueries
    {
        public async Task<ContainerHistoryEntity> Create(CreateContainerHistoryModel model, CancellationToken cancellationToken)
        {
            var historyEntity = ContainerHistoryEntity.New(
                id: model.Id,
                containerId: model.ContainerId,
                productId: model.ProductId,
                startDate: model.StartDate,
                createdBy: model.CreatedBy
            );

            await _context.ContainerHistories.AddAsync(historyEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return historyEntity;
        }

        public async Task<ContainerHistoryEntity> Update(UpdateContainerHistoryModel model, CancellationToken cancellationToken)
        {
            var historyEntity = await GetHistoryAsync(x => x.Id == model.Id, cancellationToken);

            if (historyEntity == null)
            {
                throw new InvalidOperationException("Container history not found.");
            }

            historyEntity.SetEndDate(model.EndDate ?? DateTime.UtcNow);

            await _context.SaveChangesAsync(cancellationToken);

            return historyEntity;
        }

        public async Task<ContainerHistoryEntity> Delete(DeleteContainerHistoryModel model, CancellationToken cancellationToken)
        {
            var historyEntity = await GetHistoryAsync(x => x.Id == model.Id, cancellationToken);

            if (historyEntity == null)
            {
                throw new InvalidOperationException("Container history not found.");
            }

            _context.ContainerHistories.Remove(historyEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return historyEntity;
        }

        public async Task<IReadOnlyList<ContainerHistoryEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.ContainerHistories
                .AsNoTracking()
                .Include(h => h.Container)
                .Include(h => h.Product)
                .ToListAsync(cancellationToken);
        }

        public async Task<Option<ContainerHistoryEntity>> GetById(ContainerHistoryId id, CancellationToken cancellationToken)
        {
            var entity = await GetHistoryAsync(x => x.Id == id, cancellationToken);
            return entity == null ? Option.None<ContainerHistoryEntity>() : Option.Some(entity);
        }

        public async Task<IReadOnlyList<ContainerHistoryEntity>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken)
        {
            return await _context.ContainerHistories
                .Where(h => h.ContainerId == containerId)
                .AsNoTracking()
                .Include(h => h.Product)
                .ToListAsync(cancellationToken);
        }

        private async Task<ContainerHistoryEntity?> GetHistoryAsync(Expression<Func<ContainerHistoryEntity, bool>> predicate, CancellationToken cancellationToken,
            bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _context.ContainerHistories
                    .AsNoTracking()
                    .Include(h => h.Container)
                    .Include(h => h.Product)
                    .FirstOrDefaultAsync(predicate, cancellationToken);
            }

            return await _context.ContainerHistories
                .Include(h => h.Container)
                .Include(h => h.Product)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}