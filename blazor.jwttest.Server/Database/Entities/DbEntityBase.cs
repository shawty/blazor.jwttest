using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blazor.jwttest.Server.Database.Entities
{
  public class DbEntityBase
  {
    [Column("id")]
    public int Id { get; set; }

    [Column("datecreated")]
    public DateTime DateCreated { get; set; }

    [Column("datemodified")]
    public DateTime? DateModified { get; set; }

  }
}
