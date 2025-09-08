using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.WebAPI.Infrasturture
{
    /// <summary>  
    /// Provides extension methods for dependency injection.  
    /// </summary>  
    public static class DependencyInjection
    {
        /// <summary>  
        /// Adds AutoMapper configuration and services to the specified IServiceCollection.  
        /// </summary>  
        /// <param name="services">The IServiceCollection to add the services to.</param>  
        /// <returns>The IServiceCollection with AutoMapper services added.</returns>  
        public static IServiceCollection AddAutomapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            mapperConfig.AssertConfigurationIsValid();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
