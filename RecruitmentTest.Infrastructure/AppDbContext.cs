using Microsoft.EntityFrameworkCore;
using RecruitmentTask.Domain.Entities;

namespace RecruitmentTest.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Exchange> Exchanges { get; set; }
    public DbSet<Kind> Kinds { get; set; }
    public DbSet<Mapping> Mappings { get; set; }
    public DbSet<Provider> Providers { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

}