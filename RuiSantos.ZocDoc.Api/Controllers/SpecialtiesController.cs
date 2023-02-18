using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SpecialtiesController : Controller
    {
        private readonly MedicalSpecialtiesManagement management;

        public SpecialtiesController(MedicalSpecialtiesManagement management)
        {
            this.management = management;
        }

        /// <summary>
        /// List all medical specialties avaliable
        /// </summary>
        /// <response code="200">Array of medical specialties descriptions</response>
        /// <response code="400">No medical specialties founded</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<ActionResult<string[]>> GetAsync()
        {
            var result = await management.GetMedicalSpecialitiesAsync();
            if (result.Any())
                return Ok(result.Select(s => s.Description).ToArray());

            return NotFound();
        }

        /// <summary>
        /// Create one or more medical specialties
        /// </summary>
        /// <param name="descriptions">Array of medical specialties</param>
        /// <response code="200">Medical specialty sucessufly create</response>
        /// <response code="400">Invalid arguments</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(string[] descriptions)
        {
            try
            {
                await management.CreateMedicalSpecialtiesAsync(descriptions);
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

