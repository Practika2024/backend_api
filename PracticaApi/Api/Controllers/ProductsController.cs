using System.Security.Claims;
using Api.Dtos.Products;
using Application.Commands.Products.Commands;
using Application.Common.Interfaces.Queries;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductsController(ISender sender, IProductQueries productQueries, IMapper mapper) : ControllerBase
{
    //[Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ProductDto>).ToList());
    }

    //[Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-by-id/{productId:guid}")]
    public async Task<ActionResult<ProductDto>> GetById([FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var entity = await productQueries.GetById(productId, cancellationToken);
        return entity.Match<ActionResult<ProductDto>>(
            p => Ok(mapper.Map<ProductDto>(p)),
            () => NotFound());
    }
    
    //[Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPost("add")]
    public async Task<ActionResult<ProductDto>> AddProduct(
        [FromBody] CreateProductDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddProductCommand
        {
            Name = model.Name,
            Description = model.Description,
            ManufactureDate = model.ManufactureDate,
            TypeId = model.TypeId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            dto => Ok(mapper.Map<ProductDto>(dto)),
            e => Problem(e.Message));
    }

    //[Authorize(Roles = "Operator")]
    // [HttpPut("update/{productId:guid}")]
    // public async Task<ActionResult<ProductDto>> UpdateProduct(
    //     [FromRoute] Guid productId,
    //     [FromBody] UpdateProductDto model,
    //     CancellationToken cancellationToken)
    // {
    //     var command = new UpdateProductCommand
    //     {
    //         Id = productId,
    //         Name = model.Name,
    //         Notes = model?.Notes,
    //         Volume = model.Volume,
    //         TypeId = model.TypeId,
    //         ModifiedBy = model.ModifiedBy,
    //     };
    //
    //     var result = await sender.Send(command, cancellationToken);
    //
    //     return result.Match<ActionResult<ProductDto>>(
    //         dto => Ok(dto),
    //         e => Problem(e.Message));
    // }

    //[Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpDelete("delete/{productId:guid}")]
    public async Task<ActionResult<ProductDto>> DeleteProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand
        {
            Id = productId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            dto => Ok(mapper.Map<ProductDto>(dto)),
            e => Problem(e.Message));
    }
}