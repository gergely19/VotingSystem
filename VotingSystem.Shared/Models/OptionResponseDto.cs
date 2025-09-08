namespace VotingSystem.Shared.Models
{
    public class OptionResponseDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid PollId { get; set; }
        public int VoteCount { get; set; }
    }
}