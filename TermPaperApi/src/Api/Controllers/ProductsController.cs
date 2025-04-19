using System.Security.Claims;
using Api.Dtos.Products;
using Api.Modules.Errors;
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
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class ProductsController(ISender sender, IProductQueries productQueries, IMapper mapper) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ProductDto>).ToList());
    }

    [HttpGet("get-by-id/{productId:guid}")]
    public async Task<ActionResult<ProductDto>> GetById([FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var entity = await productQueries.GetById(productId, cancellationToken);
        return entity.Match<ActionResult<ProductDto>>(
            p => Ok(mapper.Map<ProductDto>(p)),
            () => NotFound());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
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
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{productId:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        [FromRoute] Guid productId,
        [FromBody] UpdateProductDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand
        {
            Id = productId,
            Name = model.Name,
            TypeId = model.TypeId,
            Description = model.Description,
            ManufactureDate = model.ManufactureDate,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            dto => Ok(mapper.Map<ProductDto>(dto)),
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
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
            e => e.ToObjectResult());
    }
    
    [HttpPut("upload-images/{productId:guid}")]
    public async Task<ActionResult<ProductDto>> Upload([FromRoute] Guid productId, IFormFileCollection imagesFiles,
        CancellationToken cancellationToken)
    {
        var input = new UploadProductImagesCommand()
        {
            ProductId = productId,
            ImagesFiles = imagesFiles
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            r => Ok(mapper.Map<ProductDto>(r)),
            e => e.ToObjectResult());
    }
}