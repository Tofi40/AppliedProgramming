using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RoomieMatch.Application.Matching;

public class MatchingEngine
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public MatchingEngine(ApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<(double Score, string[] Rationale)> ScoreSeekerToRoom(Guid seekerId, Guid roomId)
    {
        var cacheKey = $"match-score:{seekerId}:{roomId}";
        if (_cache.TryGetValue(cacheKey, out (double Score, string[] Rationale) cached))
        {
            return cached;
        }

        var seeker = await _dbContext.Users
            .Include(u => u.PreferenceProfile)
            .FirstOrDefaultAsync(u => u.Id == seekerId);
        var room = await _dbContext.Rooms
            .Include(r => r.Owner)
            .FirstOrDefaultAsync(r => r.Id == roomId);

        if (seeker is null || room is null)
        {
            return (0, new[] { "Seeker or room not found." });
        }

        var rationale = new List<string>();
        double score = 0;
        int metrics = 0;

        if (seeker.PreferenceProfile?.MustHaveAmenities?.Any() == true)
        {
            var missingMustHave = seeker.PreferenceProfile.MustHaveAmenities.Except(room.Amenities, StringComparer.OrdinalIgnoreCase).ToList();
            if (missingMustHave.Any())
            {
                return (0, new[] { $"Missing must-have amenities: {string.Join(", ", missingMustHave)}" });
            }
        }

        if (seeker.PreferenceProfile is not null)
        {
            metrics++;
            var overlap = BudgetOverlap(seeker.PreferenceProfile.BudgetMinCents, seeker.PreferenceProfile.BudgetMaxCents, room.PricePerMonthCents);
            score += overlap * 0.2;
            rationale.Add($"Budget overlap: {Math.Round(overlap * 100)}%.");
        }

        metrics++;
        var lifestyle = LifestyleScore(seeker, room.Owner);
        score += lifestyle * 0.25;
        rationale.Add($"Lifestyle alignment score {lifestyle:F2}.");

        metrics++;
        var interest = Jaccard(seeker.Interests, room.Owner.Interests);
        score += interest * 0.2;
        rationale.Add($"Shared interests similarity {interest:F2}.");

        metrics++;
        var habits = Jaccard(seeker.Habits, room.Owner.Habits);
        score += habits * 0.15;
        rationale.Add($"Habits match {habits:F2}.");

        metrics++;
        var personality = Cosine(seeker.Personality, room.Owner.Personality);
        score += personality * 0.1;
        rationale.Add($"Personality compatibility {personality:F2}.");

        metrics++;
        var geoBoost = GeoBoost(seeker.City, room.City, seeker.Country, room.Country, seeker.LatLng(), room.LatLng());
        score += geoBoost;
        if (geoBoost > 0)
        {
            rationale.Add($"Geographic boost {geoBoost:F2}.");
        }

        score = Math.Clamp(score / metrics * 1.5, 0, 1);
        var result = (score, rationale.ToArray());
        _cache.Set(cacheKey, result, TimeSpan.FromHours(24));
        return result;
    }

    public async Task<IReadOnlyList<Match>> TopMatchesForSeeker(Guid seekerId, int limit = 20)
    {
        var rooms = await _dbContext.Rooms.Where(r => r.IsPublished)
            .OrderByDescending(r => r.CreatedAt)
            .Take(100)
            .ToListAsync();

        var matches = new List<Match>();
        foreach (var room in rooms)
        {
            var (score, rationale) = await ScoreSeekerToRoom(seekerId, room.Id);
            matches.Add(new Match
            {
                Id = Guid.NewGuid(),
                SeekerId = seekerId,
                OwnerId = room.OwnerId,
                RoomId = room.Id,
                CompatibilityScore = score,
                Rationale = string.Join("\n", rationale),
                CreatedAt = DateTime.UtcNow
            });
        }

        return matches
            .OrderByDescending(m => m.CompatibilityScore)
            .Take(limit)
            .ToList();
    }

    public async Task<IReadOnlyList<Match>> TopSeekersForOwner(Guid ownerId, int limit = 20)
    {
        var seekers = await _dbContext.Users
            .Where(u => u.Role == UserRole.Seeker)
            .OrderByDescending(u => u.UpdatedAt)
            .Take(100)
            .ToListAsync();

        var rooms = await _dbContext.Rooms.Where(r => r.OwnerId == ownerId).ToListAsync();
        var matches = new List<Match>();
        foreach (var seeker in seekers)
        {
            foreach (var room in rooms)
            {
                var (score, rationale) = await ScoreSeekerToRoom(seeker.Id, room.Id);
                matches.Add(new Match
                {
                    Id = Guid.NewGuid(),
                    SeekerId = seeker.Id,
                    OwnerId = ownerId,
                    RoomId = room.Id,
                    CompatibilityScore = score,
                    Rationale = string.Join("\n", rationale),
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        return matches
            .OrderByDescending(m => m.CompatibilityScore)
            .Take(limit)
            .ToList();
    }

    private static double Jaccard(IEnumerable<string> first, IEnumerable<string> second)
    {
        var setA = first?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()).ToHashSet() ?? new();
        var setB = second?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()).ToHashSet() ?? new();
        if (setA.Count == 0 && setB.Count == 0) return 1;
        var intersection = setA.Intersect(setB).Count();
        var union = setA.Union(setB).Count();
        return union == 0 ? 0 : (double)intersection / union;
    }

    private static double Cosine(ICollection<string> traitsA, ICollection<string> traitsB)
    {
        var all = traitsA.Concat(traitsB).Select(t => t.ToLowerInvariant()).Distinct().ToList();
        if (all.Count == 0) return 1;
        var vectorA = all.Select(traitsA.ContainsInsensitive).ToArray();
        var vectorB = all.Select(traitsB.ContainsInsensitive).ToArray();
        double dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < all.Count; i++)
        {
            dot += vectorA[i] * vectorB[i];
            magA += vectorA[i] * vectorA[i];
            magB += vectorB[i] * vectorB[i];
        }
        if (magA == 0 || magB == 0) return 0;
        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }

    private static double BudgetOverlap(int minBudget, int maxBudget, int roomPrice)
    {
        if (maxBudget <= 0) return 0.5;
        if (roomPrice < minBudget) return Math.Max(0, 1 - (double)(minBudget - roomPrice) / minBudget);
        if (roomPrice > maxBudget) return Math.Max(0, 1 - (double)(roomPrice - maxBudget) / maxBudget);
        return 1;
    }

    private static double LifestyleScore(User seeker, User owner)
    {
        var interests = Jaccard(seeker.Interests, owner.Interests);
        var habits = Jaccard(seeker.Habits, owner.Habits);
        return (interests + habits) / 2;
    }

    private static double GeoBoost(string seekerCity, string roomCity, string seekerCountry, string roomCountry, (double? Lat, double? Lng) seekerGeo, (double? Lat, double? Lng) roomGeo)
    {
        if (string.Equals(seekerCity, roomCity, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(seekerCountry, roomCountry, StringComparison.OrdinalIgnoreCase))
        {
            return 0.10;
        }

        if (seekerGeo.Lat.HasValue && roomGeo.Lat.HasValue && seekerGeo.Lng.HasValue && roomGeo.Lng.HasValue)
        {
            var distance = Haversine(seekerGeo.Lat.Value, seekerGeo.Lng.Value, roomGeo.Lat.Value, roomGeo.Lng.Value);
            if (distance <= 25)
            {
                return 0.05;
            }
        }

        return 0;
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}

internal static class MatchingExtensions
{
    public static double ContainsInsensitive(this ICollection<string> source, string value)
        => source?.Any(v => string.Equals(v, value, StringComparison.OrdinalIgnoreCase)) == true ? 1d : 0d;

    public static (double? Lat, double? Lng) LatLng(this User user)
        => (user.Latitude, user.Longitude);
}
