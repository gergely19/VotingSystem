using FluentValidation;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebAPI.Validators
{
    /// <summary>  
    /// Validator for the <see cref="OptionRequestDto"/> class.  
    /// Ensures that the Text property is not empty and does not exceed 100 characters.  
    /// </summary>  
    public class OptionRequestValidator : AbstractValidator<OptionRequestDto>
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="OptionRequestValidator"/> class.  
        /// </summary>  
        public OptionRequestValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Opció szövegének megadása kötelező.")
                .MaximumLength(100);
        }
    }
}
