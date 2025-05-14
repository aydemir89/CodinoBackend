using Codino_UserCredential.Repository.Models;
using Codino_UserCredential.Repository.Models.Content;
using Codino_UserCredential.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Codino_UserCredential.Repository.Context;

public class CodinoDbContext : DbContext
{
    public CodinoDbContext(DbContextOptions<CodinoDbContext> options) : base(options)
    {
        // PostgreSQL'de şemaları oluştur
        Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS \"user\"");
        Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS \"content\"");
    }
    
    // User İlişkili Tablolar
    public DbSet<User> Users { get; set; }
    public DbSet<UserLoginRequest> UserLoginRequests { get; set; }
    
    // Content İlişkili Tablolar
    public DbSet<WorldMap> WorldMaps { get; set; }
    public DbSet<Biome> Biomes { get; set; }
    public DbSet<Toy> Toys { get; set; }
    public DbSet<ProgrammingTask> Tasks { get; set; } // Task yerine ProgrammingTask kullanılıyor
    public DbSet<TaskSubmission> TaskSubmissions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Schema tanımlamaları
        modelBuilder.HasDefaultSchema("user");
        
        // User tablosu konfigürasyonu
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User", "user");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.TCKN).HasMaxLength(11);
            entity.Property(e => e.CellPhone).HasMaxLength(20);
            
            // E-posta için unique index
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // UserLoginRequest tablosu konfigürasyonu
        modelBuilder.Entity<UserLoginRequest>(entity =>
        {
            entity.ToTable("UserLoginRequest", "user");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Hash).HasMaxLength(256);
            entity.Property(e => e.Salt).HasMaxLength(256);
            entity.Property(e => e.Cellphone).HasMaxLength(20);
            entity.Property(e => e.Otp).HasMaxLength(10);
        });
        
        // WorldMap tablosu konfigürasyonu
        modelBuilder.Entity<WorldMap>(entity =>
        {
            entity.ToTable("WorldMap", "content");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BackgroundImageUrl).HasMaxLength(500);
        });
        
        modelBuilder.Entity<Biome>(entity =>
        {
            entity.ToTable("Biome", "content");
            entity.HasKey(e => e.id);
        
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WorldMapId).HasColumnName("WorldMapid").IsRequired(); // Küçük harfle 'i'
            entity.Property(e => e.BackgroundImageUrl).HasMaxLength(500);
        
            entity.HasOne(d => d.WorldMapNavigation) 
                .WithMany()
                .HasForeignKey(d => d.WorldMapId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Toy tablosu konfigürasyonu
        modelBuilder.Entity<Toy>(entity =>
        {
            entity.ToTable("Toy", "content");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IconImageUrl).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            // Biome ile ilişki
            entity.HasOne<Biome>()
                .WithMany()
                .HasForeignKey(e => e.BiomeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ProgrammingTask tablosu konfigürasyonu
        modelBuilder.Entity<ProgrammingTask>(entity =>
        {
            entity.ToTable("Task", "content");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ExpectedPattern).IsRequired().HasMaxLength(1000);
            
            // Toy ile ilişki
            entity.HasOne<Toy>()
                .WithMany()
                .HasForeignKey(e => e.ToyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // TaskSubmission tablosu konfigürasyonu
        modelBuilder.Entity<TaskSubmission>(entity =>
        {
            entity.ToTable("TaskSubmission", "content");
            entity.HasKey(e => e.id);
            
            // Alanlar için kısıtlamalar
            entity.Property(e => e.SubmittedCode).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            
            // Task ve User ile ilişki
            entity.HasOne<ProgrammingTask>()
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}