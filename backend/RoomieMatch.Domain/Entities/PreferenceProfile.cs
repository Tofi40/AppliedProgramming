using System;
using System.Collections.Generic;

namespace RoomieMatch.Domain.Entities;

public class PreferenceProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public int BudgetMinCents { get; set; }
    public int BudgetMaxCents { get; set; }
    public List<string> PreferredCities { get; set; } = new();
    public List<string> MustHaveAmenities { get; set; } = new();
    public List<string> NiceToHaveAmenities { get; set; } = new();
    public List<string> DealBreakers { get; set; } = new();
    public List<string> RoommateTraits { get; set; } = new();
}
