using Microsoft.AspNetCore.Mvc;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Api.Core;
using RuiSantos.Labs.Core.Services;

using static RuiSantos.Labs.Api.Core.ControllerUtils;

namespace RuiSantos.Labs.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class DoctorController : Controller
{
    const int PageSize = 25;

    private readonly IDoctorService _service;

    public DoctorController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorContract[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllDoctorsAsync([FromQuery]int? take, [FromQuery]string? from)
    {
        try
        {
            var model = await _service.GetAllDoctorsAsync(take.GetValueOrDefault(PageSize), from);
            return this.Success(model, typeof(DoctorContract[]));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
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
            var model = await _service.GetDoctorByLicenseAsync(license);
            return this.Success(model, typeof(DoctorContract));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }

    /// <summary>
    /// Gets the appointments of a doctor.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dateTime">The date of the appointments (optional).</param>
    /// <response code="200">Returns a list of the doctor's appointments.</response>
    /// <response code="204">If no records are found for the given doctor and date.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet("{doctorId}/Appointments/{dateTime?}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorAppointmentsContract[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAppointmentsAsync(Guid doctorId, DateTime? dateTime)
    {
        try
        {
            var models = await _service.GetAppointmentsAsync(
                doctorId, 
                dateTime);

            return this.Success(models, typeof(DoctorAppointmentsContract));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
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
            await _service.CreateDoctorAsync(
                request.License,
                request.Email,
                request.FirstName,
                request.LastName,
                request.ContactNumbers,
                request.Specialties);

            return this.Success(true);
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }

    /// <summary>
    /// Sets the office hours of a doctor.
    /// </summary>
    /// <param name="doctorId">The doctor's identification.</param>
    /// <param name="dayOfweek">The day of the week to set the office hours (Sunday, Monday, ...).</param>
    /// <param name="hours">An array of strings representing the office hours in HH:mm format.</param>
    /// <response code="200">Returns a success message if the office hours are successfully set.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPut("{doctorId}/OfficeHours/{dayOfweek}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutOfficeHoursAsync(Guid doctorId, string dayOfweek, [FromBody] string[] hours)
    {
        try
        {
            if (!Enum.TryParse<DayOfWeek>(dayOfweek, out var week))
                return BadRequest($"Invalid value form dayOfWeek: {dayOfweek}");

            if (!TryParseTimeSpanArray(hours, out var timeSpans))
                return BadRequest("Invalid timespan format for hours");
            
            await _service.SetOfficeHoursAsync(
                doctorId,
                week,
                timeSpans);

            return this.Success();
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }
}
