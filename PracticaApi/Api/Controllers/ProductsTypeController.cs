using Api.Dtos.ProductsType;
using Api.Modules.Errors;
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
public class ProductsTypeController(ISender sender, IProductTypeQueries productTypeQueries, IMapper mapper) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ProductTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productTypeQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ProductTypeDto>).ToList());
    }
    
    [HttpGet("get-by-id/{productTypeId:guid}")]
    public async Task<ActionResult<ProductTypeDto>> GetById([FromRoute] Guid productTypeId,
        CancellationToken cancellationToken)
    {
        var entity = await productTypeQueries.GetById(productTypeId, cancellationToken);
        
        return entity.Match<ActionResult<ProductTypeDto>>(
            p => Ok(mapper.Map<ProductTypeDto>(p)),
            () => NotFound());
    }
    
    [HttpPost("add")]
    public async Task<ActionResult<ProductTypeDto>> AddProductType(
        [FromBody] CreateUpdateProductTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddProductTypeCommand()
        {
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductTypeDto>>(
            dto => mapper.Map<ProductTypeDto>(dto),
            e => e.ToObjectResult()); 
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
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductTypeDto>>(
            dto => Ok(mapper.Map<ProductTypeDto>(dto)),
            e => e.ToObjectResult()); 
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
            e => e.ToObjectResult()); 
    }
}