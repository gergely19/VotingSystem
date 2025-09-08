using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess
{
    public static class DbInitializer
    {
        public static void Initialize(VotingDbContext context, RoleManager<UserRole>? roleManager = null, UserManager<User>? userManager = null)
        {
            context.Database.Migrate();

            if (context.Polls.Any())
            {
                return;
            }

            if (roleManager != null)
            {
                SeedRolesAsync(roleManager).Wait();
            }

            if (userManager != null)
            {
                SeedUsersAsync(userManager, context).Wait();
            }

            User? adminUser = (userManager?.Users.FirstOrDefault(u => u.Email == "admin@example.com")) ?? throw new Exception("Admin user not found.");
            List<User> users = userManager?.Users
                .Where(u => u.Email != "admin@example.com")
                .ToList() ?? [];

            List<Poll> polls = [];
            List<Option> options = [];
            List<UserPoll> userPolls = [];
            List<Vote> votes = [];

            var pollData = new[]
            {
            new {
                Question = "Mennyire vagy elégedett a cég cafeteria rendszerével?",
                Options = new[] { "Nagyon elégedett", "Elmegy", "Semleges", "Nem annyira", "Egyáltalán nem" }
            },
            new {
                Question = "Mennyire vagy elégedett a munka-magánélet egyensúllyal?",
                Options = new[] { "Kiváló", "Jó", "Átlagos", "Gyenge", "Katasztrófa" }
            },
            new {
                Question = "Mennyire érzed magad megbecsülve a munkahelyeden?",
                Options = new[] { "Teljes mértékben", "Részben", "Kissé", "Egyáltalán nem" }
            },
            new {
                Question = "Milyen gyakran kapsz visszajelzést a munkádról?",
                Options = new[] { "Hetente", "Havonta", "Ritkán", "Soha" }
            },
            new {
                Question = "Hogyan értékeled a vezetőség kommunikációját?",
                Options = new[] { "Átlátható és rendszeres", "Néha információhiányos", "Káoszos", "Nincs kommunikáció" }
            },
            new {
                Question = "Mennyire tartod hatékonynak a céges meetingeket?",
                Options = new[] { "Nagyon hatékonyak", "Elfogadhatóak", "Többnyire feleslegesek", "Időpocsékolás" }
            },
            new {
                Question = "Hogyan értékelnéd a céges juttatásokat (pl. bónusz, támogatások)?",
                Options = new[] { "Kiemelkedő", "Jó", "Közepes", "Gyenge", "Nincs" }
            },
            new {
                Question = "Milyen a munkahelyi légkör?",
                Options = new[] { "Pozitív", "Semleges", "Feszült", "Negatív" }
            },
            new {
                Question = "Milyen gyakran érzed magad stresszesnek a munkahelyeden?",
                Options = new[] { "Soha", "Ritkán", "Gyakran", "Állandóan" }
            },
            new {
                Question = "Mennyire könnyű előrelépni a cégnél?",
                Options = new[] { "Nagyon könnyű", "Lehetséges", "Nehéz", "Lehetetlen" }
            }
        };

            Random random = new();
            for (int i = 0; i < pollData.Length; i++)
            {
                User createdByUser = users[random.Next(users.Count)];

                Poll poll = new Poll
                {
                    Id = Guid.NewGuid(),
                    Question = pollData[i].Question,
                    StartDate = DateTime.Now.AddDays(-5 + i).AddHours(i).AddMinutes(i * 3),
                    EndDate = DateTime.Now.AddDays(-3 + i).AddHours(i + 1).AddMinutes(i * 4),
                    CreatedById = createdByUser.Id,
                    CreatedAt = DateTime.Now.AddDays(-1 * i)
                };

                polls.Add(poll);

                List<Option> optionList = pollData[i].Options.Select(o => new Option
                {
                    Id = Guid.NewGuid(),
                    PollId = poll.Id,
                    Text = o
                }).ToList();

                options.AddRange(optionList);

                foreach (var user in users)
                {
                    var userPoll = new UserPoll
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PollId = poll.Id,
                        HasVoted = random.Next(0, 2) == 1
                    };

                    userPolls.Add(userPoll);

                    if (userPoll.HasVoted)
                    {
                        var selectedOption = optionList[(user.Email!.GetHashCode() + i & int.MaxValue) % optionList.Count];
                        votes.Add(new Vote
                        {
                            Id = Guid.NewGuid(),
                            OptionId = selectedOption.Id
                        });
                    }
                }
            }

            context.Polls.AddRange(polls);
            context.Options.AddRange(options);
            context.UserPolls.AddRange(userPolls);
            context.Votes.AddRange(votes);

            context.SaveChanges();
        }

        private static async Task SeedRolesAsync(RoleManager<UserRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new UserRole(roleName));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager, VotingDbContext context)
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User { UserName = adminEmail, Email = adminEmail, Name = "Admin User" };
                adminUser.RefreshToken = Guid.NewGuid();
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            for (int i = 0; i < 10; i++)
            {
                var userEmail = $"user{i + 1}@example.com";
                var userPassword = "User@123";
                var userName = $"User {i + 1}";

                var existingUser = await userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        Name = userName,
                        RefreshToken = Guid.NewGuid()
                    };

                    await userManager.CreateAsync(newUser, userPassword);

                    // 🔄 Újra lekérjük az ID miatt
                    var createdUser = await userManager.FindByEmailAsync(userEmail);
                    if (createdUser != null)
                    {
                        await userManager.AddToRoleAsync(createdUser, "User");

                        var pollId = context.Polls.FirstOrDefault()?.Id;
                        if (pollId != null)
                        {
                            var userPoll = new UserPoll
                            {
                                Id = Guid.NewGuid(),
                                UserId = createdUser.Id,
                                PollId = pollId.Value,
                                HasVoted = false
                            };

                            context.UserPolls.Add(userPoll);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}