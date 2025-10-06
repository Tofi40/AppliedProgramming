using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomieMatch.Application.Matching;
using RoomieMatch.Domain.Entities;
using RoomieMatch.Infrastructure.Persistence;
using RoomieMatch.Infrastructure.Storage;

namespace RoomieMatch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddMemoryCache();
        services.AddScoped<MatchingEngine>();
        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }
}
