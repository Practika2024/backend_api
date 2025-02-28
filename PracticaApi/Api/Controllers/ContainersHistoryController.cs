using Api.Dtos.ContainerHistories;
using Api.Dtos.Users;
using Application.Common.Interfaces.Queries;
using Application.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers-history")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[Authorize(Roles = AuthSettings.AdminRole)]
[ApiController]
public class ContainersHistoryController(IContainerHistoryQueries containerHistoryQueries, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContainerHistoryDto>>> GetAll(
        [FromQuery] Guid? containerId, 
        Guid? productId, 
        DateTime? starDate,
        DateTime? endDate, 
        CancellationToken cancellationToken)
    {
        var entities = await containerHistoryQueries.GetByQuery(containerId, productId, starDate, endDate, cancellationToken);

        return entities.Select(mapper.Map<ContainerHistoryDto>).ToList();
    }
    
    [HttpGet("get-by-id/{containerHistoryId:guid}")]
    public async Task<ActionResult<ContainerHistoryDto>> GetById([FromRoute] Guid containerHistoryId,
        CancellationToken cancellationToken)
    {
        var entity = await containerHistoryQueries.GetById(containerHistoryId, cancellationToken);
        
        return entity.Match<ActionResult<ContainerHistoryDto>>(
            p => Ok(mapper.Map<ContainerHistoryDto>(p)),
            () => NotFound());
    }
}