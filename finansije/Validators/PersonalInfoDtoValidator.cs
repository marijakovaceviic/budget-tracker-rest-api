using finansije.Models;
using FluentValidation;

namespace finansije.Validators
{
    public class PersonalInfoDtoValidator : AbstractValidator<PersonalInfoDto>
    {
        public PersonalInfoDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");

            var minimumAge = 18;
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .Must(BeInPast).WithMessage("Date of birth cannot be in the future")
                .Must(x => BeValidAge(x, minimumAge)).WithMessage($"You must be at least {minimumAge} years old");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(\+381|0)?\s*\(?6\d\)?\s*\d{3}\s*\d{3,4}$")
                .WithMessage("Phone number format is invalid.");
        }

        private static bool BeInPast(DateTime dateOfBirth)
        {
            return dateOfBirth < DateTime.Today;
        }
        private static bool BeValidAge(DateTime dateOfBirth, int minimumAge)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (DateTime.Today.AddYears(-age) < dateOfBirth)
            {
                age--;
            }
            return age >= minimumAge;
        }
    }
}
