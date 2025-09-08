using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.DataAccess.Models;

namespace VotingSystem.DataAccess.Models
{
    public class User : IdentityUser
    {
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Poll> CreatedPolls { get; set; } = new List<Poll>();
        public virtual ICollection<UserPoll> UserPolls { get; set; } = new List<UserPoll>();
        public Guid? RefreshToken { get; set; }

    }
}
