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
    private readonly PatientManagement management;

    public PatientController(PatientManagement management)
    {
        this.management = management;
    }

    /// <summary>
    /// Get a patient's information using their social security number
    /// </summary>
    /// <param name="socialNumber">Social security number of the patient</param>
    /// <response code="200">Patient information retrieved successfully</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">No patient record found for the given social security number</response>
    [HttpGet("{socialNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientContract))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(string socialNumber)
    {
        try
        {
            var result = await management.GetPatientBySocialNumberAsync(socialNumber);
            return this.OkOrNotFound<PatientContract>(result);
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
    /// <response code="400">If the request object contains invalid arguments.</response>
    /// <response code="404">No records were found.</response>
    [HttpGet("{socialNumber}/Appointments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientAppointmentsContract[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsAsync(string socialNumber)
    {
        try
        {
            var result = new List<PatientAppointmentsContract>();
            await foreach (var (doctor, date) in management.GetAppointmentsAsync(socialNumber))
                result.Add(new PatientAppointmentsContract(doctor, date));

            return this.OkOrNotFound(result);
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
            await management.CreatePatientAsync(request.SocialSecurityNumber, request.Email,
                request.FirstName, request.LastName, request.ContactNumbers);

            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}
