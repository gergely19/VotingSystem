using System.Collections.Generic;
using System.Reflection.Emit;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess
{
    /// <summary>
    /// Az adatbázis kontextus, amely a Voting rendszerhez készült.
    /// </summary>
    public class VotingDbContext : IdentityDbContext<User, UserRole, string>
    {
        public DbSet<Poll> Polls { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;
        public DbSet<UserPoll> UserPolls { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;

        public VotingDbContext(DbContextOptions<VotingDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Option>()
               .HasOne(o => o.Poll)
               .WithMany(p => p.Options)
               .HasForeignKey(o => o.PollId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserPoll>()
                .HasOne(up => up.Poll)
                .WithMany(p => p.UserPolls)
                .HasForeignKey(up => up.PollId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Option)
                .WithMany(o => o.Votes)
                .HasForeignKey(v => v.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
