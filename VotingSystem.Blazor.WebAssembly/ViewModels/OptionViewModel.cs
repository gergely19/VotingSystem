using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class OptionViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid PollId { get; set; }

        public int VoteCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                yield return new ValidationResult(
                    "A válasz szöveg megadása kötelező.",
                    new[] { nameof(Text) });
            }
            else if (Text.Length > 100)
            {
                yield return new ValidationResult(
                    "A válasz szöveg legfeljebb 100 karakter lehet.",
                    new[] { nameof(Text) });
            }

            if (PollId == Guid.Empty)
            {
                yield return new ValidationResult(
                    "A PollId megadása kötelező.",
                    new[] { nameof(PollId) });
            }

            if (VoteCount < 0)
            {
                yield return new ValidationResult(
                    "A szavazatok száma nem lehet negatív.",
                    new[] { nameof(VoteCount) });
            }
        }
    }
}
