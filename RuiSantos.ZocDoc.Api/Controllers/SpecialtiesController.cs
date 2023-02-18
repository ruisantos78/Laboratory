using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    public class SpecialtiesController : Controller
    {
        private readonly MedicalSpceialtiesManagement management;

        public SpecialtiesController(IDataContext context, ILogger<SpecialtiesController> logger)
        {
            this.management = new MedicalSpceialtiesManagement(context, logger);
        }

        /// <summary>
        /// List all medical specialties avaliable
        /// </summary>
        /// <response code="200">Array of medical specialties descriptions</response>
        /// <response code="400">No medical specialties founded</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string[]> Get()
        {
            var result = management.GetMedicalSpecialities().Select(s => s.Description);
            if (result?.Any() == true)
                return Ok(result);

            return NotFound();
        }

        /// <summary>
        /// Create a new medical specialty
        /// </summary>
        /// <param name="description">Description for medical specialty</param>
        /// <response code="200">Medical specialt sucessufly create</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] string description)
        {
            try
            {
                await management.CreateMedicalSpecialtiesAsync(description);
                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }

        /// <summary>
        /// Remove an existing new medical specialty
        /// </summary>
        /// <param name="description">Medical specialty description</param>
        /// <response code="200">Medical specialt sucessufly removed</response>
        [HttpDelete("{description}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string description)
        {
            try
            {
                await management.RemoveMedicalSpecialtiesAsync(description);
                return Ok();
            }
            catch (Exception ex)
            {
                return this.FromException(ex);
            }
        }
    }
}

