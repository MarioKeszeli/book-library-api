using BookLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure;

public class CosmosDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .ToContainer("Books")
            .HasNoDiscriminator()
            .HasPartitionKey(x => x.Id);

        modelBuilder.Entity<User>()
            .ToContainer("Users")
            .HasNoDiscriminator()
            .HasPartitionKey(x => x.Id);

        modelBuilder.Entity<Borrowing>()
            .ToContainer("Borrowings")
            .HasNoDiscriminator()
            .HasPartitionKey(x => x.Id);
    }
}
