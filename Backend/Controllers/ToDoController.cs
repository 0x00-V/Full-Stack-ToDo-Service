using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.dbactions;


namespace api.Controllers;

[ApiController]
[Route("todo")]
public class ToDoController : ControllerBase
{
    

    [HttpPost("create")]
    public IActionResult CreateItem([FromHeader(Name = "Authorization")] string? authorization, [FromBody] CreateToDoItem item)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        return Unauthorized();
        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        var success = ToDoActions.CreateEntry(username, item);
        if(!success) return BadRequest("Failed to create todo item");
        return Ok(new { message = "Todo item created" });
    }

    [HttpPost("get")]
    public IActionResult GetItems([FromHeader(Name = "Authorization")] string? authorization)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        return Unauthorized();
        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        var items = ToDoActions.GetItems(username);
        return Ok(items);
    }
}