namespace blazor.jwttest.Server.Services.Exceptions
{
  public enum ValidationFailReason
  {
    Unknown = 0,
    UserNotFound,
    PasswordDoesNotMatch,
    DatabaseError
  }
}
