using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class PatientController : Controller
{
    private readonly IPatientManagement management;

    public PatientController(IPatientManagement management)
    {
        this.management = management;
    }

    /// <summary>
    /// Get a patient's information using their social security number
    /// </summary>
    /// <param name="socialNumber">Social security number of the patient</param>
    /// <response code="200">Patient information retrieved successfully</response>
    /// <response code="204">No patient record found for the given social security number</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet("{socialNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientContract))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(string socialNumber)
    {
        try
        {
            var model = await management.GetPatientBySocialNumberAsync(socialNumber);
            return this.OkOrNoContent(model, typeof(PatientContract));
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
    /// <summary>
    /// Get a patient's appointments.
    /// </summary>
    /// <param name="socialNumber">The patient's social security number.</param>
    /// <response code="200">The patient's appointments.</response>
    /// <response code="204">No records were found.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet("{socialNumber}/Appointments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientAppointmentsContract[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAppointmentsAsync(string socialNumber)
    {
        try
        {
            var models = management.GetAppointmentsAsync(socialNumber);
            return await this.OkOrNoCotentAsync(models, typeof(PatientAppointmentsContract));
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Create a new patient.
    /// </summary>
    /// <param name="request">The information for creating the patient.</param>
    /// <response code="200">The patient was successfully created.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync(PatientContract request)
    {
        try
        {
            await management.CreatePatientAsync(
                request.SocialSecurityNumber, 
                request.Email,
                request.FirstName, 
                request.LastName, 
                request.ContactNumbers);

            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
