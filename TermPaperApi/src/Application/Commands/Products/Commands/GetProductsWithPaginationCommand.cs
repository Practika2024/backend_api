using Application.Common.Interfaces.Queries;
using Application.Services;
using Domain.Common.Models;
using Domain.Products;
using MediatR;

namespace Application.Commands.Products.Commands;

public class GetProductsWithPaginationCommand : IRequest<ServiceResponse>
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}

public class GetUserWithPaginationCommandHandler(
    IProductQueries productQueries) : IRequestHandler<GetProductsWithPaginationCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        GetProductsWithPaginationCommand request,
        CancellationToken cancellationToken)
    {
        var page = request.Page;
        var pageSize = request.PageSize;
        
        var products1 = await productQueries.GetAll(cancellationToken);
        var products = products1.OrderBy(x=> x.CreatedAt);
        int totalCount = products.Count();

        if (pageSize < 0)
        {
            return ServiceResponse.BadRequestResponse($"Incorrect page size");
        }

        int pageCount = (int)Math.Ceiling(totalCount / (decimal)pageSize);

        pageCount = pageCount == 0 ? 1 : pageCount;

        if(page < 1 || page > pageCount)
        {
            return ServiceResponse.BadRequestResponse($"Page {page} not found");
        }

        var list = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var payload = new EntitiesListModel<Product>
        {
            PageCount = pageCount,
            PageSize = pageSize,
            TotalCount = totalCount,
            Page = page,
            Entities = list
        };

        return ServiceResponse.OkResponse("Users list", payload);
    }
}