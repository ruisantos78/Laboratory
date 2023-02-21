using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class SchedulesController : Controller
{
    private readonly AppointmentManagement management;

    public SchedulesController(AppointmentManagement management)
    {
        this.management = management;
    }

    /// <summary>
    /// Get doctor's schedules
    /// </summary>
    /// <param name="date">The expected date for the appointment</param>
    /// <param name="specialty">The medical specialty</param>
    /// <response code="200">The doctors' schedules</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">No records were found</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorAvailabilityContract[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromQuery] DateTime date, [FromQuery] string specialty)
    {
        try
        {
            var result = new List<DoctorAvailabilityContract>();
            await foreach (var (doctor, schedule) in management.GetAvailabilityAsync(specialty, date))
                result.Add(new DoctorAvailabilityContract(doctor, schedule));

            return this.OkOrNotFound(result);

        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }    
    }

    /// <summary>
    /// Sets a new appointment.
    /// </summary>
    /// <param name="request">The appointment request.</param>
    /// <response code="200">The appointment was successfully created.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(AppointmentContract request)
    {
        try
        {
            await management.CreateAppointmentAsync(request.PatientSecuritySocialNumber, request.MedicalLicense, request.Date);
            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Delete an appointment
    /// </summary>
    /// <param name="request">Appointment request</param>
    /// <response code="200">The appointment was successfully deleted.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">The appointment could not be found for deletion.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(AppointmentContract request)
    {
        try
        {
            await management.DeleteAppointmentAsync(request.PatientSecuritySocialNumber, request.MedicalLicense, request.Date);
            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
