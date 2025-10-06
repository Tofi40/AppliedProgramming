using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomieMatch.Domain.Entities;

namespace RoomieMatch.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<PreferenceProfile> PreferenceProfiles => Set<PreferenceProfile>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MessageThread> MessageThreads => Set<MessageThread>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.Interests).HasColumnType("text[]");
            entity.Property(u => u.Habits).HasColumnType("text[]");
            entity.Property(u => u.Personality).HasColumnType("text[]");
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql("now()");
            entity.HasOne(u => u.PreferenceProfile)
                .WithOne(p => p.User)
                .HasForeignKey<PreferenceProfile>(p => p.UserId);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => new { r.City, r.PricePerMonthCents });
            entity.Property(r => r.Photos).HasColumnType("text[]");
            entity.Property(r => r.Amenities).HasColumnType("text[]");
            entity.Property(r => r.HouseRules).HasColumnType("text[]");
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(r => r.UpdatedAt).HasDefaultValueSql("now()");
            entity.HasOne(r => r.Owner)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PreferenceProfile>(entity =>
        {
            entity.Property(p => p.PreferredCities).HasColumnType("text[]");
            entity.Property(p => p.MustHaveAmenities).HasColumnType("text[]");
            entity.Property(p => p.NiceToHaveAmenities).HasColumnType("text[]");
            entity.Property(p => p.DealBreakers).HasColumnType("text[]");
            entity.Property(p => p.RoommateTraits).HasColumnType("text[]");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Rationale).HasMaxLength(2048);
        });

        modelBuilder.Entity<MessageThread>(entity =>
        {
            entity.HasKey(mt => mt.Id);
            entity.Property(mt => mt.CreatedAt).HasDefaultValueSql("now()");
            entity.HasMany(mt => mt.Messages)
                .WithOne(m => m.Thread)
                .HasForeignKey(m => m.ThreadId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).HasMaxLength(2048);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<BookingRequest>(entity =>
        {
            entity.HasKey(br => br.Id);
            entity.Property(br => br.Note).HasMaxLength(1024);
            entity.Property(br => br.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(br => br.UpdatedAt).HasDefaultValueSql("now()");
        });
    }
}
