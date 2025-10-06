using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomieMatch.Domain.Entities;

namespace RoomieMatch.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task EnsureSeededAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var faker = new Faker("en");
        var passwordHasher = new PasswordHasher<User>();
        var owners = new List<User>();
        for (int i = 0; i < 10; i++)
        {
            var ownerEmail = faker.Internet.Email();
            var owner = new User
            {
                Id = Guid.NewGuid(),
                Email = ownerEmail,
                UserName = ownerEmail,
                Role = UserRole.Owner,
                DisplayName = faker.Name.FullName(),
                AvatarUrl = faker.Internet.Avatar(),
                Bio = faker.Lorem.Paragraph(),
                City = faker.Address.City(),
                Country = faker.Address.Country(),
                Latitude = faker.Address.Latitude(),
                Longitude = faker.Address.Longitude(),
                Interests = faker.Make(5, () => faker.Hacker.Noun()),
                Habits = faker.Make(3, () => faker.Random.Word()),
                Personality = faker.Make(3, () => faker.Hacker.Verb()),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OnboardingComplete = true
            };
            owner.PasswordHash = passwordHasher.HashPassword(owner, "Password123!");
            owners.Add(owner);
        }

        var seekers = new List<User>();
        for (int i = 0; i < 20; i++)
        {
            var seekerEmail = faker.Internet.Email();
            var seeker = new User
            {
                Id = Guid.NewGuid(),
                Email = seekerEmail,
                UserName = seekerEmail,
                Role = UserRole.Seeker,
                DisplayName = faker.Name.FullName(),
                AvatarUrl = faker.Internet.Avatar(),
                Bio = faker.Lorem.Sentence(),
                City = faker.Address.City(),
                Country = faker.Address.Country(),
                Latitude = faker.Address.Latitude(),
                Longitude = faker.Address.Longitude(),
                Interests = faker.Make(5, () => faker.Hacker.Noun()),
                Habits = faker.Make(3, () => faker.Random.Word()),
                Personality = faker.Make(3, () => faker.Hacker.Verb()),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OnboardingComplete = true,
                PreferenceProfile = new PreferenceProfile
                {
                    Id = Guid.NewGuid(),
                    BudgetMinCents = faker.Random.Int(30000, 60000),
                    BudgetMaxCents = faker.Random.Int(65000, 120000),
                    PreferredCities = faker.Make(2, () => faker.Address.City()),
                    MustHaveAmenities = faker.Make(2, () => faker.Random.Word()),
                    NiceToHaveAmenities = faker.Make(2, () => faker.Random.Word()),
                    DealBreakers = faker.Make(2, () => faker.Random.Word()),
                    RoommateTraits = faker.Make(3, () => faker.Random.Word())
                }
            };
            seeker.PasswordHash = passwordHasher.HashPassword(seeker, "Password123!");
            seekers.Add(seeker);
        }

        context.Users.AddRange(owners);
        context.Users.AddRange(seekers);
        await context.SaveChangesAsync();

        var roomFaker = new Faker<Room>("en")
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .RuleFor(r => r.OwnerId, f => f.PickRandom(owners).Id)
            .RuleFor(r => r.Title, f => f.Lorem.Sentence())
            .RuleFor(r => r.Description, f => f.Lorem.Paragraph())
            .RuleFor(r => r.Photos, f => f.Make(3, () => f.Image.PicsumUrl()))
            .RuleFor(r => r.AddressLine1, f => f.Address.StreetAddress())
            .RuleFor(r => r.City, f => f.Address.City())
            .RuleFor(r => r.Country, f => f.Address.Country())
            .RuleFor(r => r.AvailableFrom, _ => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)))
            .RuleFor(r => r.MinTermMonths, _ => faker.Random.Int(3, 12))
            .RuleFor(r => r.PricePerMonthCents, _ => faker.Random.Int(40000, 120000))
            .RuleFor(r => r.BillsIncluded, _ => faker.Random.Bool())
            .RuleFor(r => r.Amenities, f => f.Make(5, () => f.Random.Word()))
            .RuleFor(r => r.HouseRules, f => f.Make(3, () => f.Random.Word()))
            .RuleFor(r => r.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(r => r.UpdatedAt, _ => DateTime.UtcNow)
            .RuleFor(r => r.IsPublished, _ => true);

        var rooms = roomFaker.Generate(10);
        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();
    }
}
