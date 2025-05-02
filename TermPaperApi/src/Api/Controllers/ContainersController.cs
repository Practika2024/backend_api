using Api.Dtos.Containers;
using Api.Modules.Errors;
using Application.Commands.Containers.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class ContainersController(ISender sender, IContainerQueries containerQueries, IMapper mapper) : BaseController
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await containerQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Containers list", entities.Select(mapper.Map<ContainerDto>)));
    }

    [HttpGet("get-by-id/{containerId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetById(containerId, cancellationToken);

        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Container", mapper.Map<ContainerDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Container not found")));
    }
    
    [HttpGet("get-by-fill-status/{isEmpty:bool}")]
    public async Task<IActionResult> GetContainersByFillStatus(
        [FromRoute] bool isEmpty,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByFillStatus(isEmpty, cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Containers list", containers.Select(mapper.Map<ContainerDto>)));
    }

    [HttpGet("get-by-product-type/{productTypeId:guid}")]
    public async Task<IActionResult> GetContainersByProductType(
        [FromRoute] Guid productTypeId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByProductType(productTypeId, cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Containers list", containers.Select(mapper.Map<ContainerDto>)));
    }

    [HttpGet("get-by-product/{productId:guid}")]
    public async Task<IActionResult> GetContainersByProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByProduct(productId, cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Containers list", containers.Select(mapper.Map<ContainerDto>)));
    }

    [HttpGet("get-empty-by-last-product/{lastProductId:guid}")]
    public async Task<IActionResult> GetEmptyContainersByLastProduct(
        [FromRoute] Guid lastProductId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetEmptyContainersByLastProduct(lastProductId, cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Containers list", containers.Select(mapper.Map<ContainerDto>)));
    }

    [HttpGet("get-unique-code-id/{uniqueCode}")]
    public async Task<IActionResult> GetById([FromRoute] string uniqueCode,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetByUniqueCode(uniqueCode, cancellationToken);

        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Container", mapper.Map<ContainerDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Container not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<IActionResult> AddContainer(
        [FromBody] CreateContainerDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddContainerCommand
        {
            Name = model.Name,
            Volume = model.Volume,
            Notes = model.Notes,
            TypeId = model.TypeId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{containerId:guid}")]
    public async Task<IActionResult> UpdateContainer(
        [FromRoute] Guid containerId,
        [FromBody] UpdateContainerDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContainerCommand
        {
            Id = containerId,
            Name = model.Name,
            Notes = model.Notes,
            Volume = model.Volume,
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{containerId:guid}")]
    public async Task<IActionResult> DeleteContainer(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContainerCommand
        {
            Id = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
    
    [HttpPut("set-content/{containerId:guid}")]
    public async Task<IActionResult> SetContainerContent(
        [FromRoute] Guid containerId,
        [FromBody] SetContainerContentDto model,
        CancellationToken cancellationToken)
    {
        var command = new SetContainerContentCommand
        {
            ContainerId = containerId,
            ProductId = model.ProductId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
    
    [HttpPut("clear-content/{containerId:guid}")]
    public async Task<IActionResult> ClearContainerContent(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new ClearContainerContentCommand
        {
            ContainerId = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}