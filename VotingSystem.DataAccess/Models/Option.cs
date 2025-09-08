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
    public class Option
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public Guid PollId { get; set; }

        [ForeignKey("PollId")]
        public virtual Poll? Poll { get; set; }

        public virtual ICollection<Vote>? Votes { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
