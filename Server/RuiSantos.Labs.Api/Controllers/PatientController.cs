using Microsoft.AspNetCore.Mvc;
using RuiSantos.Labs.Api.Contracts;
using RuiSantos.Labs.Api.Core;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class PatientController : Controller
{
    private readonly IPatientService _service;

    public PatientController(IPatientService service)
    {
        _service = service;
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
            var model = await _service.GetPatientBySocialNumberAsync(socialNumber);
            return this.Success(model, typeof(PatientContract));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
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
            var models = _service.GetAppointmentsAsync(socialNumber);
            return await this.SuccessAsync(models, typeof(PatientAppointmentsContract));
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
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
            await _service.CreatePatientAsync(
                request.SocialSecurityNumber, 
                request.Email,
                request.FirstName, 
                request.LastName, 
                request.ContactNumbers);

            return this.Success();
        }
        catch (Exception ex)
        {
            return this.Failure(ex);
        }
    }
}
