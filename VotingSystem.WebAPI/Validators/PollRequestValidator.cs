using FluentValidation;
using VotingSystem.Shared.Models;
using VotingSystem.WebAPI.Validators;

namespace VotingSystem.WebAPI.Validators;
/// <summary>  
/// Validator for the <see cref="PollRequestDto"/> class.  
/// Ensures that the poll request contains valid data such as a non-empty question,  
/// at least two options, and valid start and end dates.  
/// </summary>  
public class PollRequestValidator : AbstractValidator<PollRequestDto>
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="PollRequestValidator"/> class.  
    /// </summary>  
    public PollRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("A kérdés megadása kötelező.")
            .MaximumLength(250);

        RuleFor(x => x.Options)
            .NotNull().WithMessage("Legalább két válasz opció szükséges.")
            .Must(o => o!.Count >= 2).WithMessage("Legalább két válasz opció szükséges.");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A kezdő időpontnak a jövőben kell lennie.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("A befejező időpontnak a jövőben kell lennie.")
            .Must((dto, end) => end > dto.StartDate.AddMinutes(15))
            .WithMessage("A befejező időpontnak legalább 15 perccel kell követnie a kezdő időpontot.");

        RuleForEach(x => x.Options!).SetValidator(new OptionRequestValidator());
    }
}
