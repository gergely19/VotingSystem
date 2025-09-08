using VotingSystem.DataAccess.Models;
using VotingSystem.WebAPI;
using VotingSystem.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Voting.DataAccess.Models;
using System.Net;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Blazor.WebAssembly.Pages.Poll;
using Blazored.LocalStorage;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Routing;
using VotingSystem.Blazor.WebAssembly.Layout;
using VotingSystem.DataAccess;

namespace VotingSystem.Tests.Integration_Tests;

public class PollsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;
    private static readonly Guid pollId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid pollId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid deletedPollId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly LoginRequestDto AdminLogin = new()
    {
        Email = "admin@example.com",
        Password = "Admin@123"
    };

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
    public PollsControllerIntegrationTests(ITestOutputHelper helper)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _output = helper;
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real database with an in-memory database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<VotingDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<VotingDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestPollsDatabase");
                });

                //Seed the database with initial data
                using var scope = services.BuildServiceProvider().CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VotingDbContext>();
                db.Database.EnsureCreated();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
                SeedRoles(roleManager);


                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                SeedUsers(userManager);


                SeedPolls(db);
            });
        });

        _client = _factory.CreateClient();
    }

    #region Get

    
    [Fact]
    public async Task GetActivePolls_ReturnsPolls_WithoutDeleted()
    {
        Login();
     
        var response = await _client.GetAsync("polls/actives");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var polls = await response.Content.ReadFromJsonAsync<List<PollResponseDto>>();
        Assert.NotNull(polls);
        _output.WriteLine($"Polls count: {polls.Count}");
        Assert.True(polls.Count == 1);
    }
    [Fact]
    public async Task GetActivePolls_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("polls/actives");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task GetUserPolls_ReturnsPolls_WithoutDeleted()
    {
        Login(userNumber: 2);

        var response = await _client.GetAsync("polls");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var polls = await response.Content.ReadFromJsonAsync<List<PollResponseDto>>();
        Assert.NotNull(polls);
        Assert.True(polls.Count == 2); // 2 polls created by user2 and active
    }

    [Fact]
    public async Task GetUserPolls_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("polls");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClosedPolls_ReturnsPolls_WithoutDeleted()
    {
        Login(userNumber: 2);

        var response = await _client.GetAsync("polls/closed");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var polls = await response.Content.ReadFromJsonAsync<List<PollResponseDto>>();
        Assert.NotNull(polls);
        Assert.True(polls.Count == 1); // 0 polls created by user2 and closed
    }

    [Fact]
    public async Task GetClosedPolls_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("polls/closed");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    
    [Fact]
    public async Task GetMovieById_ReturnsMovie_WhenMovieExists()
    {
        Login(userNumber: 2);
        var pollId = pollId1;
        var response = await _client.GetAsync($"/polls/{pollId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var poll = await response.Content.ReadFromJsonAsync<PollResponseDto>();
        Assert.NotNull(poll);
        Assert.Equal(pollId, poll.Id);
        Assert.Equal("Question 1", poll.Question);
    }

    [Fact]
    public async Task GetMovieById_ReturnsNotFound_WhenMovieNotExists()
    {
        Login(userNumber: 2);
        var pollId = Guid.Parse("99999999-9999-9999-9999-999999999999");

        var response = await _client.GetAsync($"/polls/{pollId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMovieById_ReturnsNotFound_WhenMovieDeleted()
    {
        Login(userNumber: 2);
        var pollId = deletedPollId;

        var response = await _client.GetAsync($"/polls/{pollId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }



    #endregion

    #region Post

    [Fact]
    public async Task CreatePoll_ReturnsCreated_WhenDataIsValid()
    {
        Login(userNumber: 2);

        var newPoll = new PollRequestDto
        {
            Question = "New Test Poll",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            Options = new List<OptionRequestDto>
            {
                new OptionRequestDto { Text = "Option 1" },
                new OptionRequestDto { Text = "Option 2" }
            },
            UserPolls = new List<UserPollRequestDto>
            {
            }
        };

        var response = await _client.PostAsJsonAsync("polls", newPoll);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var poll = await response.Content.ReadFromJsonAsync<PollResponseDto>();
        Assert.NotNull(poll);
        Assert.Equal(newPoll.Question, poll.Question);
    }

    [Fact]
    public async Task CreatePoll_ReturnsUnauthorized_WhenNotLoggedIn()
    {
        var newPoll = new PollRequestDto
        {
            Question = "New Test Poll",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            Options = new List<OptionRequestDto>
            {
                new OptionRequestDto { Text = "Option 1" },
                new OptionRequestDto { Text = "Option 2" }
            },
            UserPolls = new List<UserPollRequestDto>
            {
            }
        };

        var response = await _client.PostAsJsonAsync("polls", newPoll);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task CreatePoll_ReturnsBadRequest_WhenModelInvalid()
    {
        Login(userNumber: 2);

        var invalidPoll = new PollRequestDto
        {
            Question = "",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            Options = new List<OptionRequestDto>
            {
                new OptionRequestDto { Text = "Option 1" }
            },
            UserPolls = new List<UserPollRequestDto>
            {
            }
        };

        var response = await _client.PostAsJsonAsync("polls", invalidPoll);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task CreatePoll_ReturnsConflict_WhenPollWithSameQuestionExists()
    {
        Login(userNumber: 2);

        var duplicatePoll = new PollRequestDto
        {
            Question = "Question 1",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            Options = new List<OptionRequestDto>
            {
                new OptionRequestDto { Text = "Option 1" },
                new OptionRequestDto { Text = "Option 2" }
            },
            UserPolls = new List<UserPollRequestDto>
            {
            }
        };

        var response = await _client.PostAsJsonAsync("polls", duplicatePoll);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }


    #endregion


    #region Helpers

    private void Login(LoginRequestDto? loginRequest = null, int? userNumber = null)
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
        var response = _client.PostAsJsonAsync("users/login", loginRequest).Result;
        var loginResponse = response.Content.ReadFromJsonAsync<LoginResponseDto>().Result;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.AuthToken);
    }
    private void SeedUsers(UserManager<User> userManager)
    {
        // Example to seed an Admin user
        var adminUser = userManager.FindByEmailAsync(AdminLogin.Email).Result;
        if (adminUser == null)
        {
            adminUser = new User { UserName = AdminLogin.Email, Email = AdminLogin.Email, Name = "Test Admin" };
            userManager.CreateAsync(adminUser, AdminLogin.Password).Wait();
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }

        // Example to seed normal user
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
        string[] roleNames = ["Admin"];

        foreach (var roleName in roleNames)
        {
            var roleExist = roleManager.RoleExistsAsync(roleName).Result;
            if (!roleExist)
            {
                roleManager.CreateAsync(new UserRole(roleName)).Wait();
            }
        }
    }

    private void SeedPolls(VotingDbContext context)
    {
        context.Polls.AddRange(
            new Poll { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Question = "Question 1", StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(-2), CreatedById = context.Users.FirstOrDefault(u => u.Email == UserLogin2.Email)!.Id, },
            new Poll { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Question = "Question 2", StartDate = DateTime.UtcNow.AddDays(-4), EndDate = DateTime.UtcNow.AddDays(5), CreatedById = context.Users.FirstOrDefault(u => u.Email == UserLogin2.Email)!.Id, },
            new Poll { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Question = "Question 3", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(6), CreatedById = context.Users.FirstOrDefault(u => u.Email == UserLogin2.Email)!.Id, DeletedAt = DateTime.UtcNow }
        );

        context.SaveChanges();
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


