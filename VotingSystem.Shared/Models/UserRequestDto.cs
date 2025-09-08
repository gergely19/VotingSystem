using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record UserRequestDto
    {
        [Required(ErrorMessage = "Email megadása kötelező")]
        [EmailAddress(ErrorMessage = "Email helytelen")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "Jelszó kötelező")]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakter hosszúnak kell lennie")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
    ErrorMessage = "A jelszónak tartalmaznia kell kis- és nagybetűt, számot és speciális karaktert is")]
        public required string Password { get; init; }

        [Required(ErrorMessage = "Név kötelező")]
        public required string Name { get; init; }
    }
}
