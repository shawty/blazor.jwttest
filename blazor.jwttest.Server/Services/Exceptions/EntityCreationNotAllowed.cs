using System;

namespace blazor.jwttest.Server.Services.Exceptions
{
  public class EntityCreationNotAllowed : Exception
  {
    public EntityCreationNotAllowed(Type entityType)
      : base($"Creation of entitys of type {entityType.Name} not allowed.")
    { }

  }
}
