using System.ComponentModel.DataAnnotations.Schema;

namespace blazor.jwttest.Server.Database.Entities
{
  [Table("todos", Schema = "public")]
  public class DbTodo : DbEntityBase
  {
    [Column("title")]
    public string Title { get; set; }

    [Column("fulldescription")]
    public string FullDescription { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("done")]
    public bool Done { get; set; }

  }
}
