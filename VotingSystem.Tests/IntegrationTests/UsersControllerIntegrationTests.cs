using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using VotingSystem.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using VotingSystem.WebAPI;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit.Abstractions;
using VotingSystem.DataAccess;

namespace VotingSystem.Tests.Integration_Tests
{
    public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;

        public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
            _output = helper;
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VotingDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<VotingDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestUsersDb");
                    });

                    using var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<VotingDbContext>();
                    db.Database.EnsureCreated();

                    var roleManager = scopedServices.GetRequiredService<RoleManager<UserRole>>();
                    SeedRoles(roleManager).Wait();

                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    SeedUsers(userManager).Wait();
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateUser_Returns201Created_WhenValid()
        {
            var newUser = new UserRequestDto
            {
                Email = "newuser@example.com",
                Password = "User@1234",
                Name = "New User"
            };

            var response = await _client.PostAsJsonAsync("/users", newUser);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var returnedUser = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            Assert.NotNull(returnedUser);
            Assert.Equal(newUser.Email, returnedUser.Email);
        }

        [Fact]
        public async Task CreateUser_Returns400BadRequest_WhenInvalid()
        {
            var invalidUser = new UserRequestDto
            {
                Email = "not-an-email",
                Password = "short",
                Name = "Invalid User"
            };

            var response = await _client.PostAsJsonAsync("/users", invalidUser);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_Returns409Conflict_WhenEmailExists()
        {
            var existingUser = new UserRequestDto
            {
                Email = "existinguser@example.com",
                Password = "Password123!",
                Name = "Existing User"
            };

            var firstResponse = await _client.PostAsJsonAsync("/users", existingUser);
            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            var secondResponse = await _client.PostAsJsonAsync("/users", existingUser);
            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }

        [Fact]
        public async Task GetUser_Returns200OK_WhenUserExists_AndAuthenticated()
        {
            var (userId, email, password) = await CreateAndGetUserIdAsync();

            await AuthenticateAsync(email, password);
            var response = await _client.GetAsync($"/users/{userId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
        }

        [Fact]
        public async Task GetUser_Returns401Unauthorized_WhenNotAuthenticated()
        {
            var userId = "anyid";

            var response = await _client.GetAsync($"/users/{userId}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUser_Returns404NotFound_WhenUserDoesNotExist()
        {
            await AuthenticateAsync();

            var invalidUserId = "nonexistent-id-123";

            var response = await _client.GetAsync($"/users/{invalidUserId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Login_Returns200OK_WhenCredentialsAreValid()
        {
            var email = "loginuser@example.com";
            var password = "Password123!";

            await CreateUserAsync(email, password);

            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _client.PostAsJsonAsync("/users/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResponse);
            Assert.False(string.IsNullOrEmpty(loginResponse.AuthToken));
        }

        [Fact]
        public async Task Login_Returns400BadRequest_WhenModelInvalid()
        {
            var invalidLogin = new LoginRequestDto
            {
                Email = "",
                Password = ""
            };

            var response = await _client.PostAsJsonAsync("/users/login", invalidLogin);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_Returns403Forbidden_WhenCredentialsInvalid()
        {
            var loginRequest = new LoginRequestDto
            {
                Email = "doesnotexist@example.com",
                Password = "WrongPassword!"
            };

            var response = await _client.PostAsJsonAsync("/users/login", loginRequest);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Returns204NoContent_WhenAuthenticated()
        {
            await AuthenticateAsync();

            var response = await _client.PostAsync("/users/logout", null);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Returns401Unauthorized_WhenNotAuthenticated()
        {
            var response = await _client.PostAsync("/users/logout", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task RedeemRefreshToken_Returns200OK_WhenValid()
        {
            var (authToken, refreshToken, userId) = await LoginAndGetTokensAsync();

            var response = await _client.PostAsJsonAsync("/users/refresh", refreshToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResponse);
            Assert.Equal(refreshToken, loginResponse.RefreshToken);
        }

        [Fact]
        public async Task RedeemRefreshToken_Returns403Forbidden_WhenInvalidToken()
        {
            var invalidToken = "invalid-refresh-token";

            var response = await _client.PostAsJsonAsync("/users/refresh", invalidToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        private async Task AuthenticateAsync(string? email = null, string? password = null)
        {
            if (email == null || password == null)
            {
                var (userId, userEmail, userPassword) = await CreateAndGetUserIdAsync();
                email = userEmail;
                password = userPassword;
            }

            _output.WriteLine($"Authenticating with email: {email}, password: {password}");
            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var loginResponse = await _client.PostAsJsonAsync("/users/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.AuthToken);
        }


        public async Task<(string userId, string email, string password)> CreateAndGetUserIdAsync()
        {
            var email = $"testuser_{Guid.NewGuid()}@example.com";
            var password = "Test123$";

            var dto = new UserRequestDto { Email = email, Password = password, Name="Name" };
            var response = await _client.PostAsJsonAsync("/users", dto);
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();

            return (user!.Id, email, password);
        }


        private async Task CreateUserAsync(string email, string password)
        {
            var newUser = new UserRequestDto
            {
                Email = email,
                Password = password,
                Name = "Created User"
            };

            var response = await _client.PostAsJsonAsync("/users", newUser);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception($"CreateUserAsync failed: StatusCode={response.StatusCode}, Response={content}");
            }
        }

        private async Task<(string AuthToken, string RefreshToken, string UserId)> LoginAndGetTokensAsync()
        {
            var email = "tuser2@example.com";
            var password = "User@1234";

            await CreateUserAsync(email, password);

            var loginRequest = new LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var response = await _client.PostAsJsonAsync("/users/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            return (loginResponse!.AuthToken, loginResponse.RefreshToken, loginResponse.UserId);
        }

        private async Task SeedRoles(RoleManager<UserRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new UserRole(role));
            }
        }

        private async Task SeedUsers(UserManager<User> userManager)
        {
            var email = "user1@example.com";
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    Name = "User 1"
                };
                var result = await userManager.CreateAsync(user, "User@1234");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to seed user '{email}': {errors}");
                }
            }
        }

        public void Dispose()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VotingDbContext>();
            db.Database.EnsureDeleted();

            _client.Dispose();
            _factory.Dispose();
        }
    }
}
