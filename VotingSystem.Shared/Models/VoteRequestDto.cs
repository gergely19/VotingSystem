using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class VoteRequestDto
    {
        [Required]
        public Guid OptionId { get; init; }
    }
}
