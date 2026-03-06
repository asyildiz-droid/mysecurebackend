using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environment2DRepository;
    private readonly IAuthenticationService _authenticationService;

    public Environment2DController(IEnvironment2DRepository environment2DRepository, IAuthenticationService authenticationService)
    {
        _environment2DRepository = environment2DRepository;
        _authenticationService = authenticationService;
    }

    [HttpGet(Name = "GetEnvironment2D")]
    public async Task<ActionResult<List<Environment2D>>> GetAsync()
    {
        var environment2D = await _environment2DRepository.SelectAsync();
        return Ok(environment2D);
    }

    [HttpGet("{environment2DId}", Name = "GetEnvironment2DById")]
    public async Task<ActionResult<Environment2D>> GetByIdAsync(Guid environment2DId)
    {
        var environment2D = await _environment2DRepository.SelectAsync(environment2DId);

        if (environment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment2D {environment2DId} not found" });

        return Ok(environment2D);
    }

    [HttpPost(Name = "AddEnvironment2D")]
    public async Task<ActionResult<Environment2D>> AddAsync(Environment2D environment2D)
    {
        // AC1 - Gebruiker moet ingelogd zijn
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ProblemDetails { Detail = "Gebruiker niet ingelogd" });

        // AC2 - Maximum van 5 eigen 2D-werelden
        var userEnvironments = await _environment2DRepository.SelectByUserIdAsync(userId);
        if (userEnvironments.Count() >= 5)
            return BadRequest(new ProblemDetails { Detail = "Maximum aantal 2D-werelden bereikt" });

        // AC3 - Naam moet uniek zijn per gebruiker
        var existingEnvironment = await _environment2DRepository.SelectByUserIdAndNameAsync(userId, environment2D.Name);
        if (existingEnvironment != null)
            return Conflict(new ProblemDetails { Detail = "Naam bestaat al" });

        environment2D.Id = Guid.NewGuid();
        environment2D.UserId = userId;

        await _environment2DRepository.InsertAsync(environment2D);

        return CreatedAtRoute("GetEnvironment2DById", new { environment2DId = environment2D.Id }, environment2D);
    }

    [HttpPut("{environment2DId}", Name = "UpdateEnvironment2D")]
    public async Task<ActionResult<Environment2D>> UpdateAsync(Guid environment2DId, Environment2D environment2D)
    {
        var existingEnvironment2D = await _environment2DRepository.SelectAsync(environment2DId);

        if (existingEnvironment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment2D {environment2DId} not found" });

        if (environment2D.Id != environment2DId)
            return Conflict(new ProblemDetails { Detail = "The id of the Environment2D in the route does not match the id of the Environment2D in the body" });

        await _environment2DRepository.UpdateAsync(environment2D);

        return Ok(environment2D);
    }

    [HttpDelete("{environment2DId}", Name = "DeleteEnvironment2D")]
    public async Task<ActionResult> DeleteAsync(Guid environment2DId)
    {
        var environment2D = await _environment2DRepository.SelectAsync(environment2DId);

        if (environment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment2D {environment2DId} not found" });

        await _environment2DRepository.DeleteAsync(environment2DId);

        return Ok();
    }
}