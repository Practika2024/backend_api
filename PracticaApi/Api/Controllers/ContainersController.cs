using Api.Dtos.Containers;
using Api.Dtos.Reminders;
using Api.Modules.Errors;
using Api.ViewModels.Containers;
using Api.ViewModels.Reminders;
using Application.Common.Interfaces.Queries;
using Application.Services.ContainerServices.AddContainerService;
using Application.Services.ContainerServices.DeleteContainerService;
using Application.Services.ContainerServices.UpdateContainerService;
using Application.Services.ContainerServices.SetCurrentProductService;
using Application.Services.ContainerServices.ClearProductService;
using Application.Services.ContainerServices.AddReminderToContainerService;
using Domain.Authentications;
using Domain.Authentications.Users;
using Domain.Containers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[Route("containers")]
[ApiController]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ContainersController : ControllerBase
{
    private readonly IAddContainerService _addContainerService;
    private readonly IDeleteContainerService _deleteContainerService;
    private readonly IContainerQueries _containerQueries;
    private readonly IUpdateContainerService _updateContainerService;
    private readonly ISetCurrentProductService _setCurrentProductService;
    private readonly IClearProductService _clearProductService;
    private readonly IAddReminderToContainerService _addReminderService;

    public ContainersController(
        IAddContainerService addContainerService,
        IDeleteContainerService deleteContainerService,
        IContainerQueries containerQueries,
        IUpdateContainerService updateContainerService,
        ISetCurrentProductService setCurrentProductService,
        IClearProductService clearProductService,
        IAddReminderToContainerService addReminderService)
    {
        _addContainerService = addContainerService;
        _deleteContainerService = deleteContainerService;
        _containerQueries = containerQueries;
        _updateContainerService = updateContainerService;
        _setCurrentProductService = setCurrentProductService;
        _clearProductService = clearProductService;
        _addReminderService = addReminderService;
    }


    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<ActionResult<ContainerDto>> AddContainer([FromBody] CreateContainerVM containerVm, CancellationToken cancellationToken)
    {
        var result = await _addContainerService.AddContainerAsync(
            containerVm.Name,
            containerVm.Volume,
            new UserId(containerVm.UserId),
            containerVm.Notes,
            containerVm.Type,
            cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            c => Ok(ContainerDto.FromDomainModel(c)),
            e => e.ToObjectResult());
    }


    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> UpdateContainer(
        [FromRoute] Guid containerId,
        [FromBody] UpdateContainerVM containerVm,
        CancellationToken cancellationToken)
    {
        var result = await _updateContainerService.UpdateContainerAsync(
            containerId,
            containerVm.Name,
            containerVm.Volume,
            new UserId(containerVm.UserId),
            containerVm.Notes,
            containerVm.Type,
            cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            c => Ok(ContainerDto.FromDomainModel(c)),
            e => e.ToObjectResult());
    }


    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{containerId:guid}")]
    public async Task<IActionResult> DeleteContainer([FromRoute] Guid containerId, CancellationToken cancellationToken)
    {
        var result = await _deleteContainerService.DeleteContainerAsync(containerId, cancellationToken);

        return result.Match<IActionResult>(
            success => Ok(success), // Успішне видалення
            e => e.ToObjectResult()); // Обробка помилки
    }


    // [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpGet("{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> GetContainerById([FromRoute] Guid containerId, CancellationToken cancellationToken)
    {
        var entity = await _containerQueries.GetById(new ContainerId(containerId), cancellationToken);

        return entity.Match<ActionResult<ContainerDto>>(
            c => Ok(ContainerDto.FromDomainModel(c)), // Контейнер знайдено
            () => NotFound()); // Контейнер не знайдено
    }


    // [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpGet("all")]
    public async Task<ActionResult<List<ContainerDto>>> GetAllContainers(CancellationToken cancellationToken)
    {
        var entities = await _containerQueries.GetAll(cancellationToken);

        if (entities == null || !entities.Any())
        {
            return NotFound();
        }

        return Ok(entities.Select(ContainerDto.FromDomainModel).ToList());
    }


    // [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPut("set-product/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> SetCurrentProduct(
        [FromRoute] Guid containerId,
        [FromBody] ProductIdVM productIdVm,
        CancellationToken cancellationToken)
    {
        var result = await _setCurrentProductService.SetCurrentProductAsync(
            containerId,
            productIdVm.ProductId,
            cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            c => Ok(ContainerDto.FromDomainModel(c)), // Успішне встановлення продукту
            e => e.ToObjectResult()); // Обробка помилки
    }


    [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPut("clear-product/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> ClearProduct(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var result = await _clearProductService.ClearProductAsync(containerId, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            c => Ok(ContainerDto.FromDomainModel(c)), // Успішне очищення
            e => e.ToObjectResult()); // Обробка помилки
    }


    // [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPost("add-reminder/{containerId:guid}")]
    public async Task<ActionResult<ReminderDto>> AddReminder(
        [FromRoute] Guid containerId,
        [FromBody] CreateReminderVM reminderVm,
        CancellationToken cancellationToken)
    {
        var result = await _addReminderService.AddReminderAsync(
            containerId,
            reminderVm.Title,
            reminderVm.DueDate,
            reminderVm.Type,
            cancellationToken);

        return result.Match<ActionResult<ReminderDto>>(
            r => Ok(ReminderDto.FromDomainModel(r)), // Успішне додавання нагадування
            e => e.ToObjectResult()); // Обробка помилки
    }
}