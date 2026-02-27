using System.Net.Mime;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using PatientDemo.Application.Handlers.Directory.GetGenderList;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить список значений для пола пациента
    /// </summary>
    /// <response code="200">Список получен</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("genders")]
    [ProducesResponseType(typeof(EnumVm[]), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<ActionResult<IEnumerable<EnumVm>>> Get()
    {
        var query = new GetGenderListQuery();

        var patient = await mediator.Send(query, HttpContext.RequestAborted);

        return Ok(patient);
    }
}