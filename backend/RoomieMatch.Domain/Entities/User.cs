using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace RoomieMatch.Domain.Entities;

public enum UserRole
{
    Owner = 0,
    Seeker = 1
}

public class User : IdentityUser<Guid>
{
    public UserRole Role { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Pronouns { get; set; }
    public string? Occupation { get; set; }
    public List<string> Interests { get; set; } = new();
    public List<string> Habits { get; set; } = new();
    public List<string> Personality { get; set; } = new();
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool OnboardingComplete { get; set; }
    public PreferenceProfile? PreferenceProfile { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
