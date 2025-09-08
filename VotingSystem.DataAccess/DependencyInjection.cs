using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;

namespace VotingSystem.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        // Config
        services.AddHttpContextAccessor();

        // Database
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<VotingDbContext>(options => options
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies()
        );

        //Identity
        services.AddIdentity<User, UserRole>(options =>
        {
            // Password settings.
            options.Password.RequiredLength = 6;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.User.RequireUniqueEmail = true;
        })
           .AddEntityFrameworkStores<VotingDbContext>()
           .AddDefaultTokenProviders();


        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IOptionService, OptionService>();
        services.AddScoped<IUserPollService, UserPollService>();
        services.AddScoped<IVoteService, VoteService>();

        return services;
    }
}
