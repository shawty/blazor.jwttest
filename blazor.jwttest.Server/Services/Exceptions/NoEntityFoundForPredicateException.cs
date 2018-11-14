using System;

namespace blazor.jwttest.Server.Services.Exceptions
{
  // This Exception is designed to be thrown when using a predicate to look for a single entity which is not found
  public class NoEntityFoundForPredicateException : Exception
  {
    public Guid RecordId { get; set; }

    public NoEntityFoundForPredicateException()
      : base($"No entity was found matching the supplied predicate.")
    { }

  }
}
