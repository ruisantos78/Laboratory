using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SchedulesController : Controller
    {
        private readonly AppointmentManagement management;

        public SchedulesController(AppointmentManagement management)
        {
            this.management = management;
        }

        /// <summary>
        /// Get doctor's schedules
        /// </summary>
        /// <param name="date">Expected date for appointment</param>
        /// <param name="specialty">Medical specialty</param>
        /// <response code="200">Doctor's schedules</response>
        /// <response code="404">No records found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DoctorScheduleContract>>> GetAsync([FromQuery] DateTime date, [FromQuery] string specialty)
        {
            var result = new List<DoctorScheduleContract>();
            await foreach (var (doctor, schedule) in management.GetAvailabilityAsync(specialty, date))
                result.Add(new DoctorScheduleContract(doctor, schedule));

            return result.Any() ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Set an appointment
        /// </summary>
        /// <param name="request">Appointment request</param>
        /// <response code="200">Appointment sucessufly created</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(AppointmentContract request)
        {
            try
            {
                await management.CreateAppointmentAsync(request.PatientSecuritySocialNumber, request.MedicalLicense, request.Date);
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
        /// <response code="200">Appointment sucessufly deleted</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(AppointmentContract request)
        {
            try
            {
                await management.DeleteAppointmentAsync(request.PatientSecuritySocialNumber, request.MedicalLicense, request.Date);
                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}
