using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class VoteResponseDto
    {
        public Guid Id { get; set; }
        public Guid OptionId { get; set; }
    }
}
