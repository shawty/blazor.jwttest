using blazor.jwttest.Server.Database;
using blazor.jwttest.Server.Database.Entities;
using blazor.jwttest.Shared;

namespace blazor.jwttest.Server.Services
{
  public class Todos : GenericDbAccess<Todo, DbTodo>
  {
    public Todos(EfDataContext db) : base(db)
    { }

    public override void ApplyCustomMappings()
    {
      // If you need any custom mappings for mapster put them in here, and add "using mapster" to the usings at the top
      // See Users.cs for an example
    }

    // Custom object functions go here

  }
}
