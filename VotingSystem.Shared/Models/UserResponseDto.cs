using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record UserResponseDto
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Email of the person making the poll
        /// </summary>
        public required string Email { get; init; }

        /// <summary>
        /// Name of the person making the poll
        /// </summary>
        public required string Name { get; init; }
    }
}
