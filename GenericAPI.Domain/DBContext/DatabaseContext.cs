using GenericAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GenericAPI.Domain.DBContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Game>()
                   .HasOne(l => l.player)
                   .WithMany(u => u.Games)
                   .HasForeignKey(l => l.PlayerId);

            builder.Entity<Game>()
                  .HasOne(l => l.Provider)
                  .WithMany(u => u.Games)
                  .HasForeignKey(l => l.ProviderId);

            base.OnModelCreating(builder);
        }
    }
}
