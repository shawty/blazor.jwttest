using System;

namespace blazor.jwttest.Shared
{
  public class User : IViewModel
  {
    public int Id { get; set; }

    public string LoginName { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string[] AllowedRoles { get; set; }

  }
}
