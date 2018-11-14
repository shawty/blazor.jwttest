using System.ComponentModel.DataAnnotations.Schema;

namespace blazor.jwttest.Server.Database.Entities
{
  [Table("users", Schema = "public")]
  public class DbUser : DbEntityBase
  {
    [Column("loginname")]
    public string LoginName { get; set; }

    [Column("fullname")]
    public string FullName { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("allowedroles")]
    public string[] AllowedRoles { get; set; }

  }
}
