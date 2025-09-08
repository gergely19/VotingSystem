using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class PollViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OptionViewModel> Options { get; set; } = new();

        public List<UserPollViewModel> UserPolls { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Question))
            {
                yield return new ValidationResult(
                    "A kérdés megadása kötelező.",
                    new[] { nameof(Question) });
            }
            else if (Question.Length > 250)
            {
                yield return new ValidationResult(
                    "A kérdés legfeljebb 250 karakter lehet.",
                    new[] { nameof(Question) });
            }

            if (StartDate == default)
            {
                yield return new ValidationResult(
                    "A kezdési időpont megadása kötelező.",
                    new[] { nameof(StartDate) });
            }

            if (EndDate == default)
            {
                yield return new ValidationResult(
                    "A befejezési időpont megadása kötelező.",
                    new[] { nameof(EndDate) });
            }

            if (Options == null || Options.Count < 2)
            {
                yield return new ValidationResult(
                    "Legalább két válasz opció szükséges.",
                    new[] { nameof(Options) });
            }

            if (StartDate.ToUniversalTime() <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "A kezdő időpontnak a jövőben kell lennie.",
                    new[] { nameof(StartDate) });
            }

            if (EndDate.ToUniversalTime() <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "A befejező időpontnak a jövőben kell lennie.",
                    new[] { nameof(EndDate) });
            }

            if (EndDate.ToUniversalTime() <= StartDate.ToUniversalTime().AddMinutes(15))
            {
                yield return new ValidationResult(
                    "A befejező időpontnak legalább 15 perccel kell követnie a kezdő időpontot.",
                    new[] { nameof(StartDate), nameof(EndDate) });
            }
        }
    }
}
