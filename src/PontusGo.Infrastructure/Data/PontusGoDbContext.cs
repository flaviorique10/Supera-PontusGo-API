using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Models;

namespace PontusGo.Infrastructure.Data;

public class PontusGoDbContext : DbContext
{
    public PontusGoDbContext(DbContextOptions<PontusGoDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<PointTransaction> PointTransactions { get; set; }
    public DbSet<Redemption> Redemptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Name).IsRequired().HasMaxLength(150);
            entity.Property(user => user.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(100);
            entity.Property(product => product.PointsCost).IsRequired();
        });

        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.ToTable("PointTransactions");
            entity.HasKey(transaction => transaction.Id);
            entity.Property(transaction => transaction.ActivityDescription).IsRequired().HasMaxLength(255);

            entity.HasOne(transaction => transaction.Student)
                .WithMany(user => user.Transactions)
                .HasForeignKey(transaction => transaction.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Redemption>(entity =>
        {
            entity.ToTable("Redemptions");
            entity.HasKey(redemption => redemption.Id);
            entity.Property(redemption => redemption.VoucherCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(redemption => redemption.VoucherCode).IsUnique();
            entity.Property(redemption => redemption.Status).IsRequired();
            entity.Property(redemption => redemption.ExpiresAt).IsRequired();

            entity.HasOne(redemption => redemption.Student)
                .WithMany(user => user.Redemptions)
                .HasForeignKey(redemption => redemption.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(redemption => redemption.Product)
                .WithMany()
                .HasForeignKey(redemption => redemption.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
