using blazor.jwttest.Server.Services;
using blazor.jwttest.Server.Services.Exceptions;
using blazor.jwttest.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace blazor.jwttest.Server.Controllers
{
  [Route("api/[controller]")]
  public class TodosController : Controller
  {
    private Todos _todos;

    public TodosController(Todos todos)
    {
      _todos = todos;
    }

    [HttpGet("[action]")]
    public IEnumerable<Todo> All()
    {
      var allTodos = _todos.FetchAll();
      return allTodos;

    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Create([FromBody]Todo todo)
    {
      Todo newTodo = null;
      if (ModelState.IsValid)
        newTodo = _todos.Add(todo);

      if (newTodo == null)
        return BadRequest();
      
      return Ok(newTodo);
    }

    [HttpGet("[action]/{todoId:int}")]
    public IActionResult Retrieve(int todoId)
    {
      try
      {
        var thisTodo = _todos.FetchSingle(todoId);
        return Ok(thisTodo);
      }
      catch(EntityNotFoundException)
      {
        return NotFound();
      }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Update([FromBody]Todo todo)
    {
      if (ModelState.IsValid)
        _todos.Update(todo.Id, todo);

      return Ok(todo);
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Delete([FromBody]Todo todo)
    {
      try
      {
        if (ModelState.IsValid)
          _todos.Delete(todo.Id);

        return NoContent();
      }
      catch(EntityNotFoundException)
      {
        return NotFound();
      }
    }

  }
}
