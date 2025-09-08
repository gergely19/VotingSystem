using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class PollRequestDto
    {
        public required string Question { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public List<OptionRequestDto>? Options { get; set; }
        public List<UserPollRequestDto>? UserPolls { get; set; }

    }
}
