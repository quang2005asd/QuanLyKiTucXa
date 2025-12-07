using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Building> Buildings { get; set; } = null!;
    public DbSet<Floor> Floors { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasMaxLength(50)
            .IsRequired();

        // Building configuration
        modelBuilder.Entity<Building>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Building>()
            .HasMany(b => b.Floors)
            .WithOne(f => f.Building)
            .HasForeignKey(f => f.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Floor configuration
        modelBuilder.Entity<Floor>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Floor>()
            .HasMany(f => f.Rooms)
            .WithOne(r => r.Floor)
            .HasForeignKey(r => r.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Room configuration
        modelBuilder.Entity<Room>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Room>()
            .Property(r => r.RentPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Room>()
            .HasMany(r => r.Contracts)
            .WithOne(c => c.Room)
            .HasForeignKey(c => c.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student configuration
        modelBuilder.Entity<Student>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentCode)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasMany(s => s.Contracts)
            .WithOne(c => c.Student)
            .HasForeignKey(c => c.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contract configuration
        modelBuilder.Entity<Contract>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Contract>()
            .HasIndex(c => c.ContractNumber)
            .IsUnique();

        modelBuilder.Entity<Contract>()
            .Property(c => c.DepositAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Contract>()
            .Property(c => c.MonthlyRent)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Contract>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Contract)
            .HasForeignKey(i => i.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice configuration
        modelBuilder.Entity<Invoice>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .Property(i => i.RentAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.ServiceAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.TotalAmount)
            .HasPrecision(18, 2);

        // Global Query Filters for Soft Delete
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<Building>()
            .HasQueryFilter(b => !b.IsDeleted);

        modelBuilder.Entity<Floor>()
            .HasQueryFilter(f => !f.IsDeleted);

        modelBuilder.Entity<Room>()
            .HasQueryFilter(r => !r.IsDeleted);

        modelBuilder.Entity<Student>()
            .HasQueryFilter(s => !s.IsDeleted);

        modelBuilder.Entity<Contract>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Invoice>()
            .HasQueryFilter(i => !i.IsDeleted);
    }
}