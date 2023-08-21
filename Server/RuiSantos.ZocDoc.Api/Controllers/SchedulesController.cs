using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Services;

namespace RuiSantos.ZocDoc.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class SchedulesController : Controller
{
    private readonly IAppointmentService service;

    public SchedulesController(IAppointmentService service)
    {
        this.service = service;
    }

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
            return await this.OkOrNoCotentAsync(models, typeof(DoctorAvailabilityContract));
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
            await service.CreateAppointmentAsync(
                request.PatientSecuritySocialNumber, 
                request.MedicalLicense, 
                request.Date);

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
                request.MedicalLicense, 
                request.Date);
                
            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
