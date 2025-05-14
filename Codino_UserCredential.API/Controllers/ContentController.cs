using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codino_UserCredential.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly IContentOperations contentOperations;
    
    public ContentController(IContentOperations contentOperations)
    {
        this.contentOperations = contentOperations;
    }
    
    [Authorize]
    [HttpGet("WorldMap")]
    public IActionResult GetWorldMap()
    {
        var result = contentOperations.GetWorldMap();
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("UserProgress/{userId}")]
    public IActionResult GetUserProgress(int userId)
    {
        var result = contentOperations.GetUserProgress(userId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("Biome/{biomeId}")]
    public IActionResult GetBiome(int biomeId)
    {
        var result = contentOperations.GetBiome(biomeId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("Toy/{toyId}")]
    public IActionResult GetToy(int toyId)
    {
        var result = contentOperations.GetToy(toyId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("Tasks/{toyId}")]
    public IActionResult GetTasks(int toyId)
    {
        var result = contentOperations.GetTasks(toyId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("SubmitTask")]
    public IActionResult SubmitTask([FromBody] TaskSubmissionRequest request)
    {
        var result = contentOperations.SubmitTask(request);
        return Ok(result);
    }
}