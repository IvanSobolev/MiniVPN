using Microsoft.AspNetCore.Mvc;
using UserChecker.Server.Model;
using UserChecker.Server.Service.Interfaces;

namespace UserChecker.Server.Controller;

[ApiController]
[Route("[controller]")]
public class UserController(IUserManager userManager) : ControllerBase
{
    private readonly IUserManager _userManager = userManager;

    [HttpGet("admin/get/all/")]
    public IActionResult GetAllUsers()
    {
        var resp = _userManager.GetAllUsers();
        if (resp.Any())
        {
            return Ok(resp);
        }
        return NotFound();
    }
    
    [HttpPost("admin/add/")]
    public IActionResult AddUser([FromBody] UserRequestDTO user)
    {
        _userManager.AddUser(user);
        return Ok();
    }

    [HttpPut("admin/update/")]
    public IActionResult UpdateUser([FromBody] User user)
    {
        if (_userManager.UpdateUser(user))
        {
            return Ok();
        }
        return NotFound();
    }
    
    [HttpPut("admin/updatetg/")]
    public IActionResult UpdateUserByTgId([FromBody] User user)
    {
        if (_userManager.UpdateUserByTgId(user))
        {
            return Ok();
        }
        return NotFound();
    }

    [HttpDelete("admin/delete/{id}")]
    public IActionResult DeleteUserById(long id)
    {
        if(_userManager.DeleteUser(id))
        {
            return Ok();
        }
        return NotFound();
    }
    [HttpDelete("admin/deletetg/{id}")]
    public IActionResult DeleteUserByTgId(long id)
    {
        if(_userManager.DeleteUserByTgId(id))
        {
            return Ok();
        }
        return NotFound();
    }
    
    
    [HttpGet ("admin/get/{id}")]
    public IActionResult GetUserById(long id)
    {
        var user = _userManager.GetUserById(id);
        if (user == default)
        { return NotFound(); }
        return Ok(user);
    }
    
    [HttpGet ("get/{tgId}")]
    public IActionResult GetUserByTgId(long tgId)
    {
        var user = _userManager.GetUserByTgId(tgId);
        if (user == default)
        { return NotFound(); }
        return Ok(user);
    }
    
    [HttpGet("admin/getsort/")]
    public IActionResult GetAllUserSortedByTime()
    {
        var resp = _userManager.GetAllUserSortedByTime();
        if (resp.Any())
        {
            return Ok(resp);
        }
        return NotFound();
    }
    
    [HttpPut("admin/adddays/{tgId}/{days}")]
    public IActionResult AddDaysToUserByTgId(long tgId, int days)
    {
        if (_userManager.AddDaysToUserByTgId(tgId, days))
        {
            return Ok();
        }

        return NotFound();
    }
    
    [HttpPut("admin/updatecode/{tgId}/{code}")]
    public IActionResult UpdateActiveCodeByTgId(long tgId, string code)
    {
        if (_userManager.UpdateActiveCodeByTgId(tgId, code))
        {
            return Ok();
        }
        return NotFound();
    }
}