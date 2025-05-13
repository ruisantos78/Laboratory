using Microsoft.AspNetCore.Mvc;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Api.Core;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class SchedulesController(IAppointmentService service) : Controller
{

    /// <summary>
    /// Get doctor's schedules
    /// </summary>
    /// <param name="date">The expected date for the appointment</param>
    /// <param name="specialty">The medical specialty</param>
    /// <response code="200">The doctors' schedules</response>
    /// <response code="204">No records were found</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorAvailabilityContract[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync([FromQuery] DateTime date, [FromQuery] string specialty)
    {
        try
        {
            var models = service.GetAvailabilityAsync(specialty, date);
            return await this.SuccessAsync(models, typeof(DoctorAvailabilityContract));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
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
            await service.CreateAppointmentAsync(
                request.PatientSecuritySocialNumber,
                request.DoctorId,
                request.Date);

            return this.Success();
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }

    /// <summary>
    /// Delete an appointment
    /// </summary>
    /// <param name="request">Appointment request</param>
    /// <response code="200">The appointment was successfully deleted.</response>
    /// <response code="204">The appointment could not be found for deletion.</response>        
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(AppointmentContract request)
    {
        try
        {
            await service.DeleteAppointmentAsync(
                request.PatientSecuritySocialNumber,
                request.DoctorId,
                request.Date);

            return Ok();
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }
}
