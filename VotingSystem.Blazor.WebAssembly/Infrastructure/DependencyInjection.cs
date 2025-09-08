using AutoMapper;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Text.Json.Serialization;
using VotingSystem.Blazor.WebAssembly.Services;
using VotingSystem.Blazor.WebAssembly.Config;

namespace VotingSystem.Blazor.WebAssembly.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBlazorServices(this IServiceCollection services, IConfiguration config)
        {
            var appConfig = config.GetSection("AppConfig").Get<AppConfig>();
            if (appConfig == null)
                throw new ArgumentNullException(nameof(appConfig), "Not exist or wrong appConfig");

            services.AddSingleton(appConfig);
            services.AddSingleton<IToastService, ToastService>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new BlazorMappingProfile()));
            mapperConfig.AssertConfigurationIsValid();
            services.AddAutoMapper(typeof(BlazorMappingProfile));

            services.AddBlazoredLocalStorage();

            services.AddScoped<JsonSerializerOptions>(_ =>
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                return options;
            });

            services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(config["ApiBaseUrl"]!) });
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IHttpRequestUtility, HttpRequestUtility>();

            return services;
        }
    }
}
