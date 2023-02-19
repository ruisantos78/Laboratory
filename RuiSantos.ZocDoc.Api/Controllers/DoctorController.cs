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

            return result is not null ? Ok(new DoctorContract(result)) : NotFound();
        }

        /// <summary>
        /// Get doctor's appointments
        /// </summary>
        /// <param name="license">Medical license</param>
        /// <response code="200">Doctors appointments</response>
        /// <response code="400">No records found</response>
        [HttpGet("{license}/Appointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DoctorAppointmentsContract>>> GetAppointmentsAsync(string license)
        {
            var result = new List<DoctorAppointmentsContract>();
            await foreach (var (patient, date) in management.GetAppointmentsAsync(license))
                result.Add(new DoctorAppointmentsContract(patient, date));

            return result.Any() ? Ok(result) : NotFound();
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
        /// <param name="week">Day of the week: 0 - Sunday, 7 - Saturday</param>
        /// <param name="hours">Array with office hours in HH:mm format</param>
        /// <response code="200">Doctor sucessufly created</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPut("{license}/OfficeHours/{week}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutOfficeHoursAsync(string license, DayOfWeek week, string[] hours)
        {
            try
            {
                var timespans = hours.Select(TimeSpan.Parse);
                await management.SetOfficeHoursAsync(license, week, timespans);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}
