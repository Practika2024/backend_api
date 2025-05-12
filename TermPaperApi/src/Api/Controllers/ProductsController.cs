using Api.Dtos.Products;
using Application.Commands.Products.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Settings;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class ProductsController(ISender sender, IProductQueries productQueries, IMapper mapper) : BaseController(mapper)
{
    private readonly IMapper _mapper = mapper;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await productQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Products list", entities.Select(_mapper.Map<ProductDto>).ToList()));
    }
    
    [HttpGet("get-all-with-pagination")]
    public async Task<IActionResult> GetAllWithPagination(int page, int pageSize, CancellationToken cancellationToken)
    {
        var command = new GetProductsWithPaginationCommand
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult<EntitiesListModel<ProductDto>>(result);
    }

    [HttpGet("get-by-id/{productId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var entity = await productQueries.GetById(productId, cancellationToken);
        
        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Product", _mapper.Map<ProductDto>(p))),
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

        return GetResult<ProductDto>(result);
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
    
    // [HttpPut("upload-images/{productId:guid}")]
    // public async Task<IActionResult> Upload([FromRoute] Guid productId, IFormFileCollection imagesFiles,
    //     CancellationToken cancellationToken)
    // {
    //     var input = new UploadProductImagesCommand()
    //     {
    //         ProductId = productId,
    //         ImagesFiles = imagesFiles
    //     };
    //
    //     var result = await sender.Send(input, cancellationToken);
    //
    //     return GetResult<ProductDto>(result);
    // }
    //
    // [HttpPut("delete-image/{productId:guid}")]
    // public async Task<IActionResult> Upload([FromRoute] Guid productId, Guid productImageId,
    //     CancellationToken cancellationToken)
    // {
    //     var input = new DeleteProductImageCommand()
    //     {
    //         ProductId = productId,
    //         ProductImageId = productImageId
    //     };
    //
    //     var result = await sender.Send(input, cancellationToken);
    //
    //     return GetResult<ProductDto>(result);
    // }
    
    [HttpPut("update-images/{productId:guid}")]
    public async Task<IActionResult> UpdateImages([FromRoute] Guid productId, 
        [FromForm] IFormFileCollection newImages, 
        [FromForm] List<Guid> imagesToDelete,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductImagesCommand
        {
            ProductId = productId,
            NewImages = newImages,
            ImagesToDelete = imagesToDelete
        };

        var result = await sender.Send(command, cancellationToken);
        
        return GetResult<ProductDto>(result);
    }
}