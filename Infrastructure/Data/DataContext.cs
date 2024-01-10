using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
namespace Infrastructure.Data;

public class DataContext : IdentityDbContext<IdentityUser<Guid>,IdentityRole<Guid>, Guid>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
       
    }

    public DbSet<Student> Students { get; set; } 
    public DbSet<Group> Groups { get; set; }

}
