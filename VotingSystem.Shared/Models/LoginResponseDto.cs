using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    /// <summary>
    /// LoginResponseDTO
    /// </summary>
    public record LoginResponseDto
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public required string UserId { get; init; }

        /// <summary>
        /// Gets or Sets AuthToken
        /// </summary>
        public required string AuthToken { get; init; }

        /// <summary>
        /// Gets or Sets Refresh token
        /// </summary>
        public required string RefreshToken { get; init; }
    }
}
