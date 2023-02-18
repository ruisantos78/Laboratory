using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Contracts;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;

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
        /// List all medicals avaliable
        /// </summary>
        /// <response code="200">Array of medicals</response>
        /// <response code="400">No medicals founded</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetAsync()
        {
            var result = await management.GetDoctorsAsync();
            if (result.Any())
                return Ok(result);

            return NotFound();
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
        public async Task<IActionResult> PostAsync(DoctorsPostRequest request)
        {
            try
            {
                await management.CreateDoctorAsync(request.License, request.Specialities, request.Email, 
                    request.FirstName, request.LastName, request.ContactNumbers);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }

        /// <summary>
        /// Set the avaliability of the doctor for the week days
        /// </summary>
        /// <param name="uid">Doctor Identifier</param>
        /// <param name="dayOfWeek">Day of the week: 0 - Sunday, 7 - Saturday</param>
        /// <param name="hours">Doctor's office hours</param>
        /// <returns></returns>
        [HttpPut("availability/{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAvailabilityAsync([FromRoute] Guid uid, [FromQuery] DayOfWeek dayOfWeek, string[] hours)
        {
            try
            {
                await management.SetOfficeHoursAsync(uid, dayOfWeek, hours);

                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}
