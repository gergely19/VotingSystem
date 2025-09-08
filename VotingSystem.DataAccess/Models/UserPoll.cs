using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.DataAccess.Models;

namespace VotingSystem.DataAccess.Models
{
    public class UserPoll
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public Guid PollId { get; set; }

        [ForeignKey("PollId")]
        public virtual Poll? Poll { get; set; }

        public bool HasVoted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
