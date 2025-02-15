using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Data;

public class AppDbContext : DbContext
{
    public DbSet<EmailLogger> EmailLoggers { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
 
}