using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Extentions;
using Codino.UserCredential.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codino_UserCredential.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserOperations _userOperations;
    
    public UserController(IUserOperations userOperations)
    {
        _userOperations = userOperations;
    }
    
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var result = _userOperations.Login(request);
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register([FromBody] UserRegisterRequest request)
    {
        var result = _userOperations.Register(request);
        return Ok(result);
    }
    
    [HttpPost("set-avatar")]
    public IActionResult SetAvatar([FromBody] SetAvatarRequest request)
    {
        var result = _userOperations.SetAvatar(request);
        return Ok(result);
    }
    
    [HttpGet("get-avatar/{userId}")]
    public IActionResult GetAvatar(int userId)
    {
        var result = _userOperations.GetAvatar(userId);
        return Ok(result);
    }
    
    [HttpGet("available-avatars")]
    public IActionResult GetAvailableAvatars([FromQuery] GetAvailableAvatarsRequest request)
    {
        var result = _userOperations.GetAvailableAvatars(request);
        return Ok(result);
    }

    [HttpPost("set-avatar-by-id")]
    public IActionResult SetUserAvatar([FromBody] SetUserAvatarRequest request)
    {
        var result = _userOperations.SetUserAvatar(request);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("activate-toy")]
    public async Task<IActionResult> ActivateToy([FromBody] ActivateToyRequest request)
    {
        var result = await _userOperations.ActivateToyAsync(request);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("my-toys")]
    public async Task<IActionResult> GetUserToys([FromQuery] int userId)
    {
        var result = await _userOperations.GetUserToysAsync(userId);
        return Ok(result);
    }
    
}