using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;
using static RuiSantos.ZocDoc.Api.Core.ControllerUtils;

namespace RuiSantos.ZocDoc.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class DoctorController : Controller
{
    private readonly DoctorManagement management;

    public DoctorController(DoctorManagement management)
    {
        this.management = management;
    }

    /// <summary>
    /// Gets information about a doctor.
    /// </summary>
    /// <param name="license">The doctor's license number.</param>
    /// <response code="200">Returns the doctor's information.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">If no records are found for the given license.</response>
    [HttpGet("{license}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorContract))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(string license)
    {
        try
        {
            var result = await management.GetDoctorByLicenseAsync(license);

            return this.OkOrNotFound<DoctorContract>(result);
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
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">If no records are found for the given doctor and date.</response>
    [HttpGet("{license}/Appointments/{dateTime?}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorAppointmentsContract[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsAsync(string license, DateTime? dateTime)
    {
        try
        {
            var result = new List<DoctorAppointmentsContract>();
            await foreach (var (patient, date) in management.GetAppointmentsAsync(license, dateTime))
                result.Add(new DoctorAppointmentsContract(patient, date));

            return this.OkOrNotFound(result);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(DoctorContract request)
    {
        try
        {
            await management.CreateDoctorAsync(
                request.License,
                request.Email,
                request.FirstName,
                request.LastName,
                request.ContactNumbers,
                request.Specialties);

            return Ok();
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
    /// <param name="week">The day of the week to set the office hours for (0 = Sunday, 6 = Saturday).</param>
    /// <param name="hours">An array of strings representing the office hours in HH:mm format.</param>
    /// <response code="200">Returns a success message if the office hours are successfully set.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPut("{license}/OfficeHours/{week}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutOfficeHoursAsync(string license, DayOfWeek week, string[] hours)
    {
        try
        {
            await management.SetOfficeHoursAsync(
                license,
                week,
                StringToTimeSpanArray(hours));

            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
