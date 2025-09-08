using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Config
{
    public record JwtSettings
    {
        public required string SecretKey { get; init; }
        public required string Audience { get; init; }
        public required string Issuer { get; init; }
        public int AccessTokenExpirationMinutes { get; init; }
    }
}
