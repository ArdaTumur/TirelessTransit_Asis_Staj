using Microsoft.AspNetCore.Mvc;
using MyAPI.DTOs;
using MyAPI.Models;
using MyAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/bus-lines")]
public class BusLinesController(BusLineService service, AuthService userservice) : ControllerBase
{
    private readonly BusLineService _service = service;
    private readonly AuthService _userService = userservice;

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<ActionResult<List<BusLine>>> GetBusLines()
    {
        return Ok(await _service.GetAll());
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BusLine>> GetBusLine(int id)
    {
        var busLine = await _service.GetById(id);

        return busLine is null ? NotFound() : Ok(busLine);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BusLine>> CreateBusLine(CreateBusLineDto dto)
    {
        var user = await GetCurrentUser();
        if (user is null)
        {
            return Unauthorized();
        }

        var busLine = await _service.Add(dto, user.Id);

        return CreatedAtAction(nameof(GetBusLine), new { id = busLine.Id }, busLine);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BusLine>> UpdateBusLine(int id, UpdateBusLineDto dto)
    {
        var user = await GetCurrentUser();
        if (user is null)
        {
            return Unauthorized();
        }

        var busLine = await _service.Update(id, dto, user.Id);

        return busLine is null ? NotFound() : Ok(busLine);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBusLine(int id)
    {
        return await _service.Delete(id) ? NoContent() : NotFound();
    }

    private async Task<User?> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        return string.IsNullOrWhiteSpace(username)
            ? null
            : await _userService.GetUserByUsername(username);
    }
}