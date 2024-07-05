using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalks.API.Data
{
  public class NZWalksAuthDbContext : IdentityDbContext
  {
    public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      //prepare data
      var readerRoleId = "017d9aad-e96d-4f93-9677-e3972a582ffc";
      var writerRoleId = "802b38ec-9821-45d2-ab7b-371d2bdfdda6";

      var roles = new List<IdentityRole>
      {
        new IdentityRole
        {
          Id = readerRoleId,
          ConcurrencyStamp = readerRoleId,
          Name = "Reader",
          NormalizedName = "Reader".ToUpper()
        },
        new IdentityRole
        {
          Id = writerRoleId,
          ConcurrencyStamp = writerRoleId,
          Name = "Writer",
          NormalizedName = "Writer".ToUpper()
        }
      };

      //seed data
      builder.Entity<IdentityRole>().HasData(roles);

    }
  }
}
