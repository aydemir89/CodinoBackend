// AdminController.cs
using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codino_UserCredential.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] 
public class AdminController : ControllerBase
{
    private readonly IContentOperations contentOperations;
    
    public AdminController(IContentOperations contentOperations)
    {
        this.contentOperations = contentOperations;
    }
    
    // WorldMap CRUD
    [HttpGet("WorldMaps")]
    public IActionResult GetAllWorldMaps()
    {
        var result = contentOperations.GetAllWorldMaps();
        return Ok(result);
    }
    
    [HttpPost("WorldMap")]
    public IActionResult CreateWorldMap([FromBody] WorldMapCreateRequest request)
    {
        var result = contentOperations.CreateWorldMap(request);
        return Ok(result);
    }
    
    [HttpPut("WorldMap/{id}")]
    public IActionResult UpdateWorldMap(int id, [FromBody] WorldMapCreateRequest request)
    {
        var result = contentOperations.UpdateWorldMap(id, request);
        return Ok(result);
    }
    
    [HttpDelete("WorldMap/{id}")]
    public IActionResult DeleteWorldMap(int id)
    {
        var result = contentOperations.DeleteWorldMap(id);
        return Ok(result);
    }
    
    // Biome CRUD
    [HttpGet("Biomes")]
    public IActionResult GetAllBiomes()
    {
        var result = contentOperations.GetAllBiomes();
        return Ok(result);
    }
    
    [HttpPost("Biome")]
    public IActionResult CreateBiome([FromBody] BiomeCreateRequest request)
    {
        var result = contentOperations.CreateBiome(request);
        return Ok(result);
    }
    
    [HttpPut("Biome/{id}")]
    public IActionResult UpdateBiome(int id, [FromBody] BiomeCreateRequest request)
    {
        var result = contentOperations.UpdateBiome(id, request);
        return Ok(result);
    }
    
    [HttpDelete("Biome/{id}")]
    public IActionResult DeleteBiome(int id)
    {
        var result = contentOperations.DeleteBiome(id);
        return Ok(result);
    }
    
    // Toy CRUD
    [HttpGet("Toys")]
    public IActionResult GetAllToys()
    {
        var result = contentOperations.GetAllToys();
        return Ok(result);
    }
    
    [HttpPost("Toy")]
    public IActionResult CreateToy([FromBody] ToyCreateRequest request)
    {
        var result = contentOperations.CreateToy(request);
        return Ok(result);
    }
    
    [HttpPut("Toy/{id}")]
    public IActionResult UpdateToy(int id, [FromBody] ToyCreateRequest request)
    {
        var result = contentOperations.UpdateToy(id, request);
        return Ok(result);
    }
    
    [HttpDelete("Toy/{id}")]
    public IActionResult DeleteToy(int id)
    {
        var result = contentOperations.DeleteToy(id);
        return Ok(result);
    }
    
    // Task CRUD
    [HttpGet("Tasks")]
    public IActionResult GetAllTasks()
    {
        var result = contentOperations.GetAllTasks();
        return Ok(result);
    }
    
    [HttpPost("Task")]
    public IActionResult CreateTask([FromBody] TaskCreateRequest request)
    {
        var result = contentOperations.CreateTask(request);
        return Ok(result);
    }
    
    [HttpPut("Task/{id}")]
    public IActionResult UpdateTask(int id, [FromBody] TaskCreateRequest request)
    {
        var result = contentOperations.UpdateTask(id, request);
        return Ok(result);
    }
    
    [HttpDelete("Task/{id}")]
    public IActionResult DeleteTask(int id)
    {
        var result = contentOperations.DeleteTask(id);
        return Ok(result);
    }
}