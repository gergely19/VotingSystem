using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record LoginRequestDto
    {
        /// <summary>
        /// Email of the user
        /// </summary>
        [Required(ErrorMessage = "Email megadása kötelező")]
        [EmailAddress(ErrorMessage = "Helytelen email formátum")]
        public required string Email { get; init; }

        /// <summary>
        /// Password of the user
        /// </summary>
        [Required(ErrorMessage = "Jelszó megadása kötelező")]
        public required string Password { get; init; }
    }

}
