using GenericAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GenericAPI.Domain.DBContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Player>()
                   .HasKey(p => p.Id);

            builder.Entity<Player>()
                    .Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);


            foreach (var property in typeof(Player).GetProperties())
            {
                if (property.Name != "Id")
                {
                    builder.Entity<Player>()
                        .Property(property.Name)
                        .IsRequired();
                }
            }
            builder.Entity<Provider>()
                   .Property(p => p.ProviderName)
                   .IsRequired();

            builder.Entity<Game>()
                  .HasOne(l => l.Provider)
                  .WithMany(u => u.Games)
                  .HasForeignKey(l => l.ProviderId);

            base.OnModelCreating(builder);
        }
    }
}
