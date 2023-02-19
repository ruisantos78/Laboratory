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
        /// Get the doctor informations
        /// </summary>
        /// <param name="license">Doctor license number</param>
        /// <response code="200">Doctor informations</response>
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

        /// <summary>
        /// Search for doctor with their schedules
        /// </summary>
        /// <param name="date">Expected date for appointment</param>
        /// <param name="specialty">Medical specialty</param>
        /// <response code="200">List of doctors with a free schedule</response>
        /// <response code="404">No records found</response>
        [HttpGet("schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DoctorWithScheduleContract>>> GetAsync([FromQuery] DateTime date, [FromQuery] string specialty)
        {
            var result = new List<DoctorWithScheduleContract>();
            await foreach (var (doctor, schedule) in management.GetDoctorWithScheduleBySpecialityAsync(specialty, date))
                result.Add(new DoctorWithScheduleContract(doctor, schedule));
            
            if (!result.Any())
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new Doctor
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Doctor sucessufly created</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(DoctorContract request)
        {
            try
            {
                await management.CreateDoctorAsync(request.License, request.Email, request.FirstName,
                    request.LastName, request.ContactNumbers, request.Specialties);

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
        /// <response code="200">Doctor sucessufly created</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPut("hours/{license}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAvailabilityAsync([FromRoute] string license, [FromQuery] DayOfWeek dayOfWeek, string[] hours)
        {
            try
            {
                var timespans = hours.Select(TimeSpan.Parse);
                await management.SetOfficeHoursAsync(license, dayOfWeek, timespans);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}
