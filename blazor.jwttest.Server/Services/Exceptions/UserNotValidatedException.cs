using System;

namespace blazor.jwttest.Server.Services.Exceptions
{
  // This exception is designed to be thrown when a user login cannot be validated against the database.
  public class UserNotValidatedException : Exception
  {
    public ValidationFailReason ValidationFailReason { get; private set; }
    public string UserLoginName { get; private set; }

    public UserNotValidatedException(ValidationFailReason failReason, string userLoginName)
      : base($"User Validation Failed.")
    {
      UserLoginName = userLoginName;
      ValidationFailReason = failReason;
    }

  }
}
