using Microsoft.EntityFrameworkCore;

namespace Tsisa.Blockchain.Persistence;

public class BlockchainContext : DbContext
{
    public DbSet<Block> Blocks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres");
    }
}