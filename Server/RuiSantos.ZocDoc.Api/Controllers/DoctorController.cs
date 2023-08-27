using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Services;
using static RuiSantos.ZocDoc.Api.Core.ControllerUtils;

namespace RuiSantos.ZocDoc.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class DoctorController : Controller
{
    private readonly IDoctorService service;

    public DoctorController(IDoctorService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Gets information about a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <response code="200">Returns the doctor's information.</response>
    /// <response code="204">If no records are found for the given license.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet("{license}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorContract))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(string license)
    {
        try
        {
            var model = await service.GetDoctorByLicenseAsync(license);
            return this.OkOrNoContent(model, typeof(DoctorContract));
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Gets the appointments of a doctor.
    /// </summary>
    /// <param name="license">The doctor's medical license.</param>
    /// <param name="dateTime">The date of the appointments (optional).</param>
    /// <response code="200">Returns a list of the doctor's appointments.</response>
    /// <response code="204">If no records are found for the given doctor and date.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet("{license}/Appointments/{dateTime?}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorAppointmentsContract[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        try
        {
            var models = await service.GetAppointmentsAsync(
                license, 
                dateTime);

            return this.OkOrNoContent(models, typeof(DoctorAppointmentsContract));
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Creates a new doctor.
    /// </summary>
    /// <param name="request">The request object containing the doctor's details.</param>
    /// <response code="200">Returns a success message if the doctor is successfully created.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(DoctorContract request)
    {
        try
        {
            await service.CreateDoctorAsync(
                request.License,
                request.Email,
                request.FirstName,
                request.LastName,
                request.ContactNumbers,
                request.Specialties);

            return StatusCode(201);
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Sets the office hours of a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <param name="dayOfweek">The day of the week to set the office hours (Sunday, Monday, ...).</param>
    /// <param name="hours">An array of strings representing the office hours in HH:mm format.</param>
    /// <response code="200">Returns a success message if the office hours are successfully set.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPut("{license}/OfficeHours/{dayOfweek}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutOfficeHoursAsync(string license, string dayOfweek, [FromBody] string[] hours)
    {
        try
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfweek, out var week))
                return BadRequest($"Invalid value form dayOfWeek: {dayOfweek}");

            if (!TryParseTimeSpanArray(hours, out var timeSpans))
                return BadRequest("Invalid timespan format for hours");
            
            await service.SetOfficeHoursAsync(
                license,
                week,
                timeSpans);

            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
