using blazor.jwttest.Server.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace blazor.jwttest.Server.Database
{
  public class EfDataContext : DbContext
  {
    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbTodo> Todos { get; set; }

    public EfDataContext() : base() { }

    public EfDataContext(DbContextOptions<EfDataContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      //builder.Entity<DbUser>()
      //  .Property(b => b.Id)
      //  .ValueGeneratedOnAdd();

      // Make sure the base is called
      base.OnModelCreating(builder);

      // Call our data seeder
      SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
      // NOTE: We use ".HasData" here, this means this data will ONLY be seeded IF the table
      // is completley empty.
      builder.Entity<DbUser>()
        .HasData(
          new DbUser
          {
            Id = 1,
            LoginName = "admin",
            FullName = "Administrator",
            Email = "admin@mycorp.com",
            Password = BCrypt.Net.BCrypt.HashPassword("letmein"),
            AllowedRoles = new string[] {"admin", "user", "manager", "reporter"}
          }
        );

    }

  }
}
