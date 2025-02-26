using Api.Dtos.ProductsType;
using Application.Commands.ProductsType.Commands;
using Application.Common.Interfaces.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
[ApiController]
public class ProductsTypeController(ISender sender, IProductTypeQueries productTypeQueries, IMapper mapper) : BaseController
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ProductTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productTypeQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ProductTypeDto>).ToList());
    }
    
    [HttpPost("add")]
    public async Task<ActionResult<ProductTypeDto>> AddProductType(
        [FromBody] CreateUpdateProductTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddProductTypeCommand()
        {
            Name = model.Name,
            CreatedBy = GetUserId()!.Value
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductTypeDto>>(
            dto => mapper.Map<ProductTypeDto>(dto),
            e => Problem(e.Message));
    }
    
    [HttpPut("update/{productId:guid}")]
    public async Task<ActionResult<ProductTypeDto>> UpdateProductType(
        [FromRoute] Guid productId,
        [FromBody] CreateUpdateProductTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductTypeCommand
        {
            Id = productId,
            Name = model.Name,
            ModifiedBy = GetUserId()!.Value,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductTypeDto>>(
            dto => Ok(mapper.Map<ProductTypeDto>(dto)),
            e => Problem(e.Message));
    }
    
    [HttpDelete("delete/{productId:guid}")]
    public async Task<ActionResult<ProductTypeDto>> DeleteProductType(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductTypeCommand
        {
            Id = productId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductTypeDto>>(
            dto => Ok(mapper.Map<ProductTypeDto>(dto)),
            e => Problem(e.Message));
    }
}