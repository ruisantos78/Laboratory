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
        /// Get the doctor by the license number
        /// </summary>
        /// <param name="socialNumber">Security Social Number</param>
        /// <response code="200">Doctor information</response>
        /// <response code="400">No records found</response>
        [HttpGet("{socialNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientContract>> GetAsync(string socialNumber)
        {
            var result = await management.GetPatientBySocialNumberAsync(socialNumber);
            if (result is null)
                return NotFound();

            return Ok(new PatientContract(result));
        }

        /// <summary>
        /// Create a new Doctor
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Doctor sucessufly create</response>
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
