using System.Net.Mime;
using System.Text.Json;

using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using PatientDemo.Application.Handlers.Patients.Commands.CreatePatient;
using PatientDemo.Application.Handlers.Patients.Commands.DeletePatient;
using PatientDemo.Application.Handlers.Patients.Commands.UpdatePatient;
using PatientDemo.Application.Handlers.Patients.Queries.GetByDatePatientList;
using PatientDemo.Application.Handlers.Patients.Queries.GetPatient;
using PatientDemo.Application.Interfaces;
using PatientDemo.Shared.DTO.Requests;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(
    IMediator mediator,
    IMapper mapper,
    IFhirDateParser fhirDateParser) : ControllerBase
{
    /// <summary>
    /// Создать пациента
    /// </summary>
    /// <response code="201">Пациент создан</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<ActionResult<string>> Create([FromBody] CreatePatientDto dto)
    {
        var command = mapper.Map<CreatePatientCommand>(dto);

        var patientId = await mediator.Send(command, HttpContext.RequestAborted);

        return CreatedAtAction(nameof(Get), new { id = patientId }, JsonSerializer.Serialize(patientId));
    }

    /// <summary>
    /// Обновить пациента
    /// </summary>
    /// <response code="204">Пациент обновлён</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="404">Пациент не найден</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update([FromBody] UpdatePatientDto dto)
    {
        var command = mapper.Map<UpdatePatientCommand>(dto);

        await mediator.Send(command, HttpContext.RequestAborted);

        return NoContent();
    }

    /// <summary>
    /// Удалить пациента
    /// </summary>
    /// <response code="204">Пациент удалён</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="404">Пациент не найден</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete([FromBody] DeletePatientDto dto)
    {
        var command = mapper.Map<DeletePatientCommand>(dto);

        await mediator.Send(command, HttpContext.RequestAborted);

        return NoContent();
    }

    /// <summary>
    /// Получить пациента
    /// </summary>
    /// <response code="200">Пациент получен</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientVm), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PatientVm>> Get(Guid id)
    {
        var query = new GetPatientQuery
        {
            Id = id
        };

        var patient = await mediator.Send(query, HttpContext.RequestAborted);

        return Ok(patient);
    }

    /// <summary>
    /// Получить пациента
    /// </summary>
    /// <response code="200">Пациенты получены</response>
    /// <response code="400">Ошибка валидации</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpGet("filter")]
    [ProducesResponseType(typeof(PatientVm[]), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<ActionResult<IEnumerable<PatientVm>>> GetPatients([FromQuery] string[]? date)
    {
        var query = new GetByBirthDatePatientListQuery();

        if (date is not null)
        {
            var parsingResult = fhirDateParser.GetPeriod(date);

            query.DateFrom = parsingResult.DateFrom;
            query.DateTo = parsingResult.DateTo;
        }

        var patients = await mediator.Send(query, HttpContext.RequestAborted);

        return Ok(patients);
    }
}