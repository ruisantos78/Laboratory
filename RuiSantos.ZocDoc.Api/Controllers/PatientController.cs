using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers
{
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
        /// Get a patient by security social numeber
        /// </summary>
        /// <param name="socialNumber">Security Social Number</param>
        /// <response code="200">Patient information</response>
        /// <response code="404">No records found</response>
        [HttpGet("{socialNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientContract>> GetAsync(string socialNumber)
        {
            var result = await management.GetPatientBySocialNumberAsync(socialNumber);

            return (result is not null) ? Ok(new PatientContract(result)) : NotFound();
        }

        /// <summary>
        /// Get patient's appointments
        /// </summary>
        /// <param name="socialNumber">Security Social Number</param>
        /// <response code="200">Patient appointments</response>
        /// <response code="400">No records found</response>
        [HttpGet("{socialNumber}/Appointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PatientAppointmentsContract>>> GetAppointmentsAsync(string socialNumber)
        {
            var result = new List<PatientAppointmentsContract>();
            await foreach (var (doctor, date) in management.GetAppointmentsAsync(socialNumber))
                result.Add(new PatientAppointmentsContract(doctor, date));

            return result.Any() ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Patient sucessufly create</response>
        /// <response code="400">Invalid arguments</response>
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
}
