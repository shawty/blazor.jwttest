using System;

namespace blazor.jwttest.Server.Services.Exceptions
{
  // This exception is designed to be thrown when the specific entity ID is known, but not found.
  public class EntityNotFoundException : Exception
  {
    public int RecordId { get; set; }

    public EntityNotFoundException(int recordId, Type entityType)
      : base($"Requested entity of type {entityType.Name} with id {recordId.ToString()} was not found in database")
    {
      RecordId = recordId;
    }

  }
}
