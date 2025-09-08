using VotingSystem.DataAccess.Models;
using VotingSystem.WebAPI;
using VotingSystem.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Xunit.Abstractions;
using Xunit;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess;

namespace VotingSystem.Tests.Integration_Tests;

public class VoteControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    private static readonly LoginRequestDto UserLogin1 = new()
    {
        Email = "user1@example.com",
        Password = "User@123"
    };
    private static readonly LoginRequestDto UserLogin2 = new()
    {
        Email = "user2@example.com",
        Password = "User@123"
    };

    private static readonly Guid optionId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid optionIdInvalid = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public VoteControllerIntegrationTests(ITestOutputHelper helper)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _output = helper;
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<VotingDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<VotingDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestVotesDatabase");
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VotingDbContext>();
                db.Database.EnsureCreated();

                var roleManager = scopedServices.GetRequiredService<RoleManager<UserRole>>();
                SeedRoles(roleManager);

                var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                SeedUsers(userManager);

                SeedPollsAndOptions(db, userManager).Wait();
            });
        });

        _client = _factory.CreateClient();
    }

    #region Tests

    [Fact]
    public async Task VoteAsync_ReturnsOk_WhenVoteIsValid()
    {
        await Login(userNumber: 1);

        var voteRequest = new VoteRequestDto
        {
            OptionId = optionId1
        };

        var response = await _client.PostAsJsonAsync("/votes", voteRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task VoteAsync_ReturnsBadRequest_WhenUserNotLoggedIn()
    {
        var voteRequest = new VoteRequestDto
        {
            OptionId = optionId1
        };

        var response = await _client.PostAsJsonAsync("/votes", voteRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VoteAsync_ReturnsBadRequest_WhenOptionDoesNotExist()
    {
        await Login(userNumber: 2);

        var voteRequest = new VoteRequestDto
        {
            OptionId = optionIdInvalid
        };

        var response = await _client.PostAsJsonAsync("/votes", voteRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Helpers

    private async Task Login(LoginRequestDto? loginRequest = null, int? userNumber = null)
    {
        if (loginRequest == null)
        {
            loginRequest = new()
            {
                Email = UserLogin1.Email,
                Password = UserLogin1.Password
            };
        }
        if (userNumber != null)
        {
            if (userNumber == 1)
                loginRequest = new()
                {
                    Email = UserLogin1.Email,
                    Password = UserLogin1.Password
                };
            else if (userNumber == 2)
                loginRequest = new()
                {
                    Email = UserLogin2.Email,
                    Password = UserLogin2.Password
                };
        }
        var response = await _client.PostAsJsonAsync("users/login", loginRequest);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.AuthToken);
    }

    private void SeedUsers(UserManager<User> userManager)
    {
        var user1 = userManager.FindByEmailAsync(UserLogin1.Email).Result;
        if (user1 == null)
        {
            user1 = new User { UserName = UserLogin1.Email, Email = UserLogin1.Email, Name = "Test User 1" };
            userManager.CreateAsync(user1, UserLogin1.Password).Wait();
        }

        var user2 = userManager.FindByEmailAsync(UserLogin2.Email).Result;
        if (user2 == null)
        {
            user2 = new User { UserName = UserLogin2.Email, Email = UserLogin2.Email, Name = "Test User 2" };
            userManager.CreateAsync(user2, UserLogin2.Password).Wait();
        }
    }

    private void SeedRoles(RoleManager<UserRole> roleManager)
    {
        string[] roleNames = { "Admin" };

        foreach (var roleName in roleNames)
        {
            var roleExist = roleManager.RoleExistsAsync(roleName).Result;
            if (!roleExist)
            {
                roleManager.CreateAsync(new UserRole(roleName)).Wait();
            }
        }
    }

    private async Task SeedPollsAndOptions(VotingDbContext context, UserManager<User> userManager)
    {
        var user2 = await userManager.FindByEmailAsync(UserLogin2.Email);

        var poll = new Poll
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Question = "Test Poll for Voting",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            CreatedById = user2!.Id
        };
        context.Polls.Add(poll);

        var option1 = new Option
        {
            Id = optionId1,
            PollId = poll.Id,
            Text = "Option 1"
        };
        context.Options.Add(option1);

        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<VotingDbContext>();
        db.Database.EnsureDeleted();

        _factory.Dispose();
        _client.Dispose();
    }

    #endregion
}
