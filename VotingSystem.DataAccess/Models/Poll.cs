using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;

namespace Voting.DataAccess.Models
{
    public class Poll
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
        public required string Question { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [ForeignKey("CreatedBy")]
        public required string? CreatedById { get; set; }
        public virtual User? CreatedBy { get; set; }

        public virtual ICollection<Option>? Options { get; set; } = new List<Option>();
        public virtual ICollection<UserPoll>? UserPolls { get; set; } = new List<UserPoll>();
        public DateTime CreatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }


    }
}
