using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RoomieMatch.Infrastructure.Persistence;

#nullable disable

namespace RoomieMatch.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0-preview.3.24172.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 128)
            .HasAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
            .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<string>("ClaimType")
                .HasColumnType("text");

            b.Property<string>("ClaimValue")
                .HasColumnType("text");

            b.Property<Guid>("RoleId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("RoleId");

            b.ToTable("AspNetRoleClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<string>("ClaimType")
                .HasColumnType("text");

            b.Property<string>("ClaimValue")
                .HasColumnType("text");

            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>", b =>
        {
            b.Property<string>("LoginProvider")
                .HasColumnType("text");

            b.Property<string>("ProviderKey")
                .HasColumnType("text");

            b.Property<string>("ProviderDisplayName")
                .HasColumnType("text");

            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.HasKey("LoginProvider", "ProviderKey");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserLogins", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>", b =>
        {
            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.Property<Guid>("RoleId")
                .HasColumnType("uuid");

            b.HasKey("UserId", "RoleId");

            b.HasIndex("RoleId");

            b.ToTable("AspNetUserRoles", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>", b =>
        {
            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.Property<string>("LoginProvider")
                .HasColumnType("text");

            b.Property<string>("Name")
                .HasColumnType("text");

            b.Property<string>("Value")
                .HasColumnType("text");

            b.HasKey("UserId", "LoginProvider", "Name");

            b.ToTable("AspNetUserTokens", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<Guid>", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<string>("ConcurrencyStamp")
                .HasColumnType("text");

            b.Property<string>("Name")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<string>("NormalizedName")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.HasKey("Id");

            b.HasIndex("NormalizedName")
                .IsUnique()
                .HasDatabaseName("RoleNameIndex");

            b.ToTable("AspNetRoles", (string)null);
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.BookingRequest", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<string>("Note")
                .HasColumnType("text");

            b.Property<Guid>("RoomId")
                .HasColumnType("uuid");

            b.Property<Guid>("SeekerId")
                .HasColumnType("uuid");

            b.Property<int>("Status")
                .HasColumnType("integer");

            b.Property<DateTime>("UpdatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.HasKey("Id");

            b.HasIndex("RoomId");

            b.HasIndex("SeekerId");

            b.ToTable("BookingRequests");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Match", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<double>("CompatibilityScore")
                .HasColumnType("double precision");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<Guid>("OwnerId")
                .HasColumnType("uuid");

            b.Property<string>("Rationale")
                .HasMaxLength(2048)
                .HasColumnType("character varying(2048)");

            b.Property<Guid>("RoomId")
                .HasColumnType("uuid");

            b.Property<Guid>("SeekerId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("OwnerId");

            b.HasIndex("RoomId");

            b.HasIndex("SeekerId");

            b.ToTable("Matches");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Message", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<string>("Content")
                .HasMaxLength(2048)
                .HasColumnType("character varying(2048)");

            b.Property<Guid>("SenderId")
                .HasColumnType("uuid");

            b.Property<Guid>("ThreadId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("SenderId");

            b.HasIndex("ThreadId");

            b.ToTable("Messages");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.MessageThread", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<DateTime?>("LastMessageAt")
                .HasColumnType("timestamp with time zone");

            b.Property<Guid>("OwnerId")
                .HasColumnType("uuid");

            b.Property<Guid>("RoomId")
                .HasColumnType("uuid");

            b.Property<Guid>("SeekerId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("OwnerId");

            b.HasIndex("RoomId");

            b.HasIndex("SeekerId");

            b.ToTable("MessageThreads");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.PreferenceProfile", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<int>("BudgetMaxCents")
                .HasColumnType("integer");

            b.Property<int>("BudgetMinCents")
                .HasColumnType("integer");

            b.Property<string[]>("DealBreakers")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string[]>("MustHaveAmenities")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string[]>("NiceToHaveAmenities")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string[]>("PreferredCities")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string[]>("RoommateTraits")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<Guid>("UserId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("UserId")
                .IsUnique();

            b.ToTable("PreferenceProfiles");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Review", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<string>("Body")
                .HasColumnType("text");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<int>("Rating")
                .HasColumnType("integer");

            b.Property<Guid>("RevieweeId")
                .HasColumnType("uuid");

            b.Property<Guid>("ReviewerId")
                .HasColumnType("uuid");

            b.HasKey("Id");

            b.HasIndex("RevieweeId");

            b.HasIndex("ReviewerId");

            b.ToTable("Reviews");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Room", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<string>("AddressLine1")
                .HasColumnType("text");

            b.Property<string>("AddressLine2")
                .HasColumnType("text");

            b.Property<string[]>("Amenities")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<DateOnly>("AvailableFrom")
                .HasColumnType("date");

            b.Property<bool>("BillsIncluded")
                .HasColumnType("boolean");

            b.Property<string>("City")
                .HasColumnType("text");

            b.Property<string>("Country")
                .HasColumnType("text");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<string>("Description")
                .HasColumnType("text");

            b.Property<bool>("IsPublished")
                .HasColumnType("boolean");

            b.Property<double?>("Lat")
                .HasColumnType("double precision");

            b.Property<double?>("Lng")
                .HasColumnType("double precision");

            b.Property<int>("MinTermMonths")
                .HasColumnType("integer");

            b.Property<Guid>("OwnerId")
                .HasColumnType("uuid");

            b.Property<string[]>("Photos")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<int>("PricePerMonthCents")
                .HasColumnType("integer");

            b.Property<string[]>("HouseRules")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string>("Title")
                .HasColumnType("text");

            b.Property<DateTime>("UpdatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.HasKey("Id");

            b.HasIndex("OwnerId");

            b.HasIndex("City", "PricePerMonthCents");

            b.ToTable("Rooms");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.User", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("uuid");

            b.Property<int>("AccessFailedCount")
                .HasColumnType("integer");

            b.Property<int?>("Age")
                .HasColumnType("integer");

            b.Property<string>("AvatarUrl")
                .HasColumnType("text");

            b.Property<string>("Bio")
                .HasColumnType("text");

            b.Property<string>("City")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("ConcurrencyStamp")
                .HasColumnType("text");

            b.Property<string>("Country")
                .IsRequired()
                .HasColumnType("text");

            b.Property<DateTime>("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.Property<string>("DisplayName")
                .IsRequired()
                .HasColumnType("text");

            b.Property<string>("Email")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<bool>("EmailConfirmed")
                .HasColumnType("boolean");

            b.Property<string>("Gender")
                .HasColumnType("text");

            b.Property<string[]>("Habits")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string[]>("Interests")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<bool>("LockoutEnabled")
                .HasColumnType("boolean");

            b.Property<DateTimeOffset?>("LockoutEnd")
                .HasColumnType("timestamp with time zone");

            b.Property<double?>("Latitude")
                .HasColumnType("double precision");

            b.Property<double?>("Longitude")
                .HasColumnType("double precision");

            b.Property<bool>("OnboardingComplete")
                .HasColumnType("boolean");

            b.Property<string>("NormalizedEmail")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<string>("NormalizedUserName")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<string>("Occupation")
                .HasColumnType("text");

            b.Property<string>("PasswordHash")
                .HasColumnType("text");

            b.Property<string[]>("Personality")
                .IsRequired()
                .HasColumnType("text[]");

            b.Property<string>("PhoneNumber")
                .HasColumnType("text");

            b.Property<bool>("PhoneNumberConfirmed")
                .HasColumnType("boolean");

            b.Property<string>("Pronouns")
                .HasColumnType("text");

            b.Property<int>("Role")
                .HasColumnType("integer");

            b.Property<string>("SecurityStamp")
                .HasColumnType("text");

            b.Property<string>("UserName")
                .HasMaxLength(256)
                .HasColumnType("character varying(256)");

            b.Property<bool>("TwoFactorEnabled")
                .HasColumnType("boolean");

            b.Property<DateTime>("UpdatedAt")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");

            b.HasKey("Id");

            b.HasIndex("Email")
                .IsUnique();

            b.HasIndex("NormalizedEmail")
                .HasDatabaseName("EmailIndex");

            b.HasIndex("NormalizedUserName")
                .IsUnique()
                .HasDatabaseName("UserNameIndex");

            b.ToTable("AspNetUsers", (string)null);
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.BookingRequest", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Seeker")
                .WithMany()
                .HasForeignKey("SeekerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.Room", "Room")
                .WithMany("BookingRequests")
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Room");

            b.Navigation("Seeker");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Match", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Owner")
                .WithMany()
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.Room", "Room")
                .WithMany()
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.User", "Seeker")
                .WithMany()
                .HasForeignKey("SeekerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Owner");

            b.Navigation("Room");

            b.Navigation("Seeker");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Message", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Sender")
                .WithMany("Messages")
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.MessageThread", "Thread")
                .WithMany("Messages")
                .HasForeignKey("ThreadId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Sender");

            b.Navigation("Thread");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.MessageThread", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Owner")
                .WithMany()
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.Room", "Room")
                .WithMany()
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.User", "Seeker")
                .WithMany()
                .HasForeignKey("SeekerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Owner");

            b.Navigation("Room");

            b.Navigation("Seeker");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.PreferenceProfile", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "User")
                .WithOne("PreferenceProfile")
                .HasForeignKey("RoomieMatch.Domain.Entities.PreferenceProfile", "UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("User");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Review", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Reviewee")
                .WithMany()
                .HasForeignKey("RevieweeId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("RoomieMatch.Domain.Entities.User", "Reviewer")
                .WithMany()
                .HasForeignKey("ReviewerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Reviewee");

            b.Navigation("Reviewer");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Room", b =>
        {
            b.HasOne("RoomieMatch.Domain.Entities.User", "Owner")
                .WithMany("Rooms")
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("Owner");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.User", b =>
        {
            b.Navigation("Messages");

            b.Navigation("PreferenceProfile");

            b.Navigation("Rooms");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.MessageThread", b =>
        {
            b.Navigation("Messages");
        });

        modelBuilder.Entity("RoomieMatch.Domain.Entities.Room", b =>
        {
            b.Navigation("BookingRequests");
        });
#pragma warning restore 612, 618
    }
}
