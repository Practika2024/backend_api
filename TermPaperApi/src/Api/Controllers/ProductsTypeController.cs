using Api.Dtos.ProductsType;
using Api.Modules.Errors;
using Application.Commands.ProductsType.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
[ApiController]
public class ProductsTypeController(ISender sender, IProductTypeQueries productTypeQueries, IMapper mapper)
    : BaseController
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productTypeQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse
            .OkResponse("Products types list", entities.Select(mapper.Map<ProductTypeDto>).ToList()));
    }

    [HttpGet("get-by-id/{productTypeId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid productTypeId,
        CancellationToken cancellationToken)
    {
        var entity = await productTypeQueries.GetById(productTypeId, cancellationToken);

        return entity.Match(
            p => GetResult(ServiceResponse.OkResponse("Product type", mapper.Map<ProductTypeDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Product type not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<IActionResult> AddProductType(
        [FromBody] CreateUpdateProductTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddProductTypeCommand()
        {
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{productId:guid}")]
    public async Task<IActionResult> UpdateProductType(
        [FromRoute] Guid productId,
        [FromBody] CreateUpdateProductTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductTypeCommand
        {
            Id = productId,
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{productId:guid}")]
    public async Task<IActionResult> DeleteProductType(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductTypeCommand
        {
            Id = productId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}