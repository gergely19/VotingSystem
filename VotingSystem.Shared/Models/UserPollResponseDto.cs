namespace VotingSystem.Shared.Models
{
    public class UserPollResponseDto
    {
        public Guid Id { get; init; }
        public string UserId { get; init; } = string.Empty;

        public Guid PollId { get; init; }
        public bool HasVoted { get; init; }
    }
}