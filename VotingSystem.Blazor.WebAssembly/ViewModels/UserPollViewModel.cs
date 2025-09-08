using System;
using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class UserPollViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public Guid PollId { get; set; }

        public bool HasVoted { get; set; }

    }
}
