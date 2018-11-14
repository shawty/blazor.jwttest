using blazor.jwttest.Server.Database;
using blazor.jwttest.Server.Database.Entities;
using blazor.jwttest.Server.Services.Exceptions;
using blazor.jwttest.Shared;
using Mapster;
using System;
using System.Linq;

namespace blazor.jwttest.Server.Services
{
  public class Users : GenericDbAccess<User, DbUser>
  {
    public Users(EfDataContext db) : base(db)
    {}

    public override void ApplyCustomMappings()
    {
      // From VM to DB
      TypeAdapterConfig<User, DbUser>.NewConfig()
        .IgnoreNullValues(true) // If any data passed in is null in the source DO NOT change the destination
        .AfterMapping((src, dest) => {
          if(!String.IsNullOrEmpty(src.Password))
          {
            // If the password field in the source object is not empty, then encrypt it in the destination
            // before the record is saved into the database.
            // NOTE: If you DO NOT want to change the users password, or want to keep it intact, then you MUST
            // make sure that the source password property is null or empty before it hits this service.
            dest.Password = BCrypt.Net.BCrypt.HashPassword(src.Password);
          }
        });

      // From DB to VM
      TypeAdapterConfig<DbUser, User>.NewConfig()
        .Ignore(dest => dest.Password);  // Don't allow password to be retrieved from DB

    }

    // Custom functions for the User Service Go Here
    public User ValidateLogin(string loginName, string password)
    {
      // This call uses the DB context directly, as we need to validate the encrypted password
      // using BCrypt to verify that the login details are correct.  For retrieving/updating etc
      // you should always use the methods in the base class (See GenericDbAccess.cs)
      DbUser foundUser = _db.Users.FirstOrDefault(r => r.LoginName == loginName);
      if (foundUser == null) throw new UserNotValidatedException(ValidationFailReason.UserNotFound, loginName);

      if(!BCrypt.Net.BCrypt.Verify(password, foundUser.Password))
      {
        throw new UserNotValidatedException(ValidationFailReason.PasswordDoesNotMatch, loginName);
      }

      User userToReturn = foundUser.Adapt<User>();

      return userToReturn;
    }

  }
}
