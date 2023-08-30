using Microsoft.AspNetCore.Mvc;
using RuiSantos.Labs.Api.Core;
using RuiSantos.Labs.Core.Services;

namespace RuiSantos.Labs.Api.Controllers;

[Route("[controller]")]
[Produces("application/json")]
[ApiController]
public class SpecialtiesController : Controller
{
    private readonly IMedicalSpecialtiesService service;

    public SpecialtiesController(IMedicalSpecialtiesService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Get all available medical specialties.
    /// </summary>
    /// <response code="200">An array of medical specialty descriptions.</response>
    /// <response code="204">No medical specialties found.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string[]))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]    
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var result = await service.GetMedicalSpecialitiesAsync();
            return this.OkOrNoContent<String>(result.OrderBy(s => s));
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }

    /// <summary>
    /// Create one or more medical specialties.
    /// </summary>
    /// <param name="descriptions">An array of medical specialty descriptions.</param>
    /// <response code="200">Medical specialties created successfully.</response>
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody]List<string> descriptions)
    {
        try
        {
            await service.CreateMedicalSpecialtiesAsync(descriptions);
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
    /// <response code="400">If the request object contains invalid arguments.</response>
    [HttpDelete("{description}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAsync(string description)
    {
        try
        {
            await service.RemoveMedicalSpecialtiesAsync(description);
            return Ok();
        }
        catch (Exception ex)
        {
            return this.FromException(ex);
        }
    }
}

