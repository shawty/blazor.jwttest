using blazor.jwttest.Server.Services;
using blazor.jwttest.Server.Services.Exceptions;
using blazor.jwttest.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace blazor.jwttest.Server.Controllers
{
  [Route("api/[controller]")]
  public class UsersController : Controller
  {
    private Users _users;

    public UsersController(Users users)
    {
      _users = users;
    }

    [HttpGet("[action]")]
    public IEnumerable<User> All()
    {
      var allUsers = _users.FetchAll();
      return allUsers;

    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Create([FromBody]User user)
    {
      User newUser = null;
      if (ModelState.IsValid)
        newUser = _users.Add(user);

      if (newUser == null)
        return BadRequest();
      
      return Ok(newUser);
    }

    [HttpGet("[action]/{mapId:int}")]
    public IActionResult Retrieve(int userId)
    {
      try
      {
        var thisUser = _users.FetchSingle(userId);
        return Ok(thisUser);
      }
      catch(EntityNotFoundException)
      {
        return NotFound();
      }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Update([FromBody]User user)
    {
      if (ModelState.IsValid)
        _users.Update(user.Id, user);

      return Ok(user);
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Delete([FromBody]User user)
    {
      try
      {
        if (ModelState.IsValid)
          _users.Delete(user.Id);

        return NoContent();
      }
      catch(EntityNotFoundException)
      {
        return NotFound();
      }
    }

  }
}
