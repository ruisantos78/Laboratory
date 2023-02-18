using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers
{
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
        /// Get the doctor by the license number
        /// </summary>
        /// <param name="license">Medical license number</param>
        /// <response code="200">Doctor information</response>
        /// <response code="400">No records found</response>
        [HttpGet("{license}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<ActionResult<DoctorContract>> GetAsync(string license)
        {
            var result = await management.GetDoctorByLicenseAsync(license);
            if (result is null)
                return NotFound();
                
            return Ok(new DoctorContract(result));
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorAvailabilityContract[]>> GetAsync([FromQuery] DateTime date, [FromQuery] string specialty)
        {
            var result = await management.GetDoctorBySpecialityAsync(specialty, date);
            if (!result.Any())
                return NotFound();

            return Ok(result.Select(s => new DoctorAvailabilityContract(s, date)).ToArray());
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
        public async Task<IActionResult> PostAsync(DoctorContract request)
        {
            try
            {
                await management.CreateDoctorAsync(request.License, request.Specialties, request.Email, 
                    request.FirstName, request.LastName, request.ContactNumbers);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }

        /// <summary>
        /// Set the doctor's office hours
        /// </summary>
        /// <param name="license">Doctor license</param>
        /// <param name="dayOfWeek">Day of the week: 0 - Sunday, 7 - Saturday</param>
        /// <param name="hours">Array with office hours in HH:mm format</param>
        /// <returns></returns>
        [HttpPut("hours/{license}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAvailabilityAsync([FromRoute] string license, [FromQuery] DayOfWeek dayOfWeek, string[] hours)
        {
            try
            {
                await management.SetOfficeHoursAsync(license, dayOfWeek, hours);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}
