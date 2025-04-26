using System.Security.Claims;
using Api.Dtos.Products;
using Api.Modules.Errors;
using Application.Commands.Products.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
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
public class ProductsController(ISender sender, IProductQueries productQueries, IMapper mapper) : BaseController
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Products list", entities.Select(mapper.Map<ProductDto>).ToList()));
    }

    [HttpGet("get-by-id/{productId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var entity = await productQueries.GetById(productId, cancellationToken);
        
        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Product", mapper.Map<ProductDto>(p))),
            () =>GetResult(ServiceResponse.NotFoundResponse("Product not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<IActionResult> AddProduct(
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

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(
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

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand
        {
            Id = productId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
    
    [HttpPut("upload-images/{productId:guid}")]
    public async Task<IActionResult> Upload([FromRoute] Guid productId, IFormFileCollection imagesFiles,
        CancellationToken cancellationToken)
    {
        var input = new UploadProductImagesCommand()
        {
            ProductId = productId,
            ImagesFiles = imagesFiles
        };

        var result = await sender.Send(input, cancellationToken);

        return GetResult(result);
    }
}