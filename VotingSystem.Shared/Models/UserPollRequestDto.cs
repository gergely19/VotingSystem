using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Shared.Models
{
    public class UserPollRequestDto
    {
        public bool HasVoted { get; init; } = false;
    }
}