using Microsoft.AspNetCore.Mvc;
using RuiSantos.ZocDoc.Api.Core;
using RuiSantos.ZocDoc.Core.Managers;

namespace RuiSantos.ZocDoc.Api.Controllers;

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
    /// Get all available medical specialties.
    /// </summary>
    /// <response code="200">An array of medical specialty descriptions.</response>
    /// <response code="404">No medical specialties found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]        
    public async Task<ActionResult<IEnumerable<string>>> GetAsync()
    {
        var result = await management.GetMedicalSpecialitiesAsync();

        return result.Any() ? Ok(result.Select(s => s.Description)) : NotFound();
    }

    /// <summary>
    /// Create one or more medical specialties.
    /// </summary>
    /// <param name="descriptions">An array of medical specialty descriptions.</param>
    /// <response code="200">Medical specialties created successfully.</response>
    /// <response code="400">Invalid arguments.</response>
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
    /// Remove an existing medical specialty.
    /// </summary>
    /// <param name="description">The description of the medical specialty to be removed.</param>
    /// <response code="200">The medical specialty was successfully removed.</response>
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

