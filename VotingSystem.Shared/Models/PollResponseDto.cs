using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class PollResponseDto
    {
        public Guid Id { get; init; }
        public required string Question { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime CreatedAt { get; init; }

        public List<OptionResponseDto>? Options { get; init; }
        public List<UserPollResponseDto>? UserPolls { get; init; }
    }
}
