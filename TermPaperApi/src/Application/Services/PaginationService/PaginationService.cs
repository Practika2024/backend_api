using Domain.Common.Abstractions;
using Domain.Common.Models;

namespace Application.Services.PaginationService;

public static class PaginationService
{
    public static ServiceResponse GetEntitiesWithPagination<TEntity>(int? page, int? pageSize, List<TEntity> entitiesList)
        where TEntity : AuditableEntity
    {
        if (page is null)
        {
            return ServiceResponse.BadRequestResponse($"Incorrect page");
        }
        
        if (pageSize is null || pageSize <= 0)
        {
            return ServiceResponse.BadRequestResponse($"Incorrect page size");
        }
        
        var entities = entitiesList.OrderBy(x=> x.CreatedAt).ToList();
        int totalCount = entities.Count();

        int pageCount = (int)Math.Ceiling(totalCount / (decimal)pageSize);

        pageCount = pageCount == 0 ? 1 : pageCount;

        if(page < 1 || page > pageCount)
        {
            return ServiceResponse.NotFoundResponse($"Page {page} not found");
        }

        var list = entities.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();

        var payload = new EntitiesListModel<TEntity>
        {
            PageCount = pageCount,
            PageSize = pageSize.Value,
            TotalCount = totalCount,
            Page = page.Value,
            Entities = list
        };
        
        return ServiceResponse.OkResponse($"{typeof(TEntity).Name}s list", payload);
    }
}