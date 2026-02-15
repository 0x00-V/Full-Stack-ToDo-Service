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


    [HttpGet("get")]
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

    [HttpGet("get/{id}")]
    public IActionResult GetItem([FromHeader(Name = "Authorization")] string? authorization, [FromRoute] int id)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        return Unauthorized();
        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        var item = ToDoActions.GetItem(id, username);
        return Ok(item);
    }

    [HttpPut("edit/{id}")]
    public IActionResult EditItem([FromHeader(Name = "Authorization")] string? authorization, [FromRoute] int id, [FromBody] CreateToDoItem todoitem)
    {
        if(string.IsNullOrEmpty(todoitem.Title) || string.IsNullOrEmpty(todoitem.Description)) return BadRequest();
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        return Unauthorized();
        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        var item = ToDoActions.UpdateItem(id, username, todoitem.Title, todoitem.Description);
        if(item == false) return BadRequest();
        return Ok("Item Updated.");
    }

    [HttpPut("update/{id}")]
    public IActionResult UpdateItem([FromHeader(Name = "Authorization")] string? authorization, [FromRoute] int id)
    {
        if(string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer")) return Unauthorized();

        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        if(id < 0) return BadRequest();

        bool result = ToDoActions.ToggleItem(id, username);
        if (result == false) return BadRequest();
        return Ok();
    }


    [HttpDelete("delete/{id}")]
    public IActionResult DeleteItems([FromHeader(Name = "Authorization")] string? authorization, [FromRoute] int id)
    {
        if(string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer")) return Unauthorized();

        var token = authorization["Bearer ".Length..];
        var principal = AccountController.ValidateJWTToken(token);
        if(principal == null) return Unauthorized();

        var username = principal.Identity!.Name;
        if(username == null) return Unauthorized();

        if(id < 0) return BadRequest();

        bool result = ToDoActions.DeleteItem(id, username);
        if (result == false) return BadRequest();

        return Ok();
    }
}