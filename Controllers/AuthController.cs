using Microsoft.AspNetCore.Mvc;
using MyAPI.DTOs;
using MyAPI.Models;
using MyAPI.Services;

namespace MYAPI.Controllers;

[ApiController]
[Route("api/")]
public class AuthController(AuthService service) : ControllerBase
{
    private readonly AuthService _service = service;

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        var token = await _service.Login(dto);

        return token is null ? Unauthorized() : Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var user = await _service.Register(dto);
        return Created("user", user);
    }
}