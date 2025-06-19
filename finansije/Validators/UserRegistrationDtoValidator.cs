using System.Collections.Generic;
using finansije.Models;
using FluentValidation;

namespace finansije.Validators
{
    public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
    {
        public UserRegistrationDtoValidator() {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Length(4, 20).WithMessage("Username must contain between 4 and 20 characters")
                .Must(StartWithLetter).WithMessage("Username must start with a letter")
                .Must(ContainOnlyAllowedCharacters).WithMessage("Username can contain only letters, numbers, '.', and '_'");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Must(ContainSpecialCharactersOrUppercaseLetter).WithMessage("Password must contain at least one uppercase letter or one special character")
                .Must(ContainDigit).WithMessage("Password must contain at least one digit");
            
            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password).WithMessage("Passwords do not match");

            RuleFor(u => u.Role)
                .Must(ValidRole).WithMessage("Role must be 'user' or 'admin");

            RuleFor(u =>u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid mail foramt");

            RuleFor(x => x.PersonalInfo)
                .NotNull().WithMessage("Personal information is required")
                .SetValidator(new PersonalInfoDtoValidator());

            RuleFor(x => x.AddressInfo)
                .NotNull().WithMessage("Address information is required")
                .SetValidator(new AddressInfoDtoValidator());

            RuleFor(x => x.AcceptTerms)
               .Equal(true).WithMessage("You must accept the terms and conditions");

        }

        private bool StartWithLetter(string userName)
        {
            return !string.IsNullOrEmpty(userName) && char.IsLetter(userName[0]);
        }

        private bool ContainOnlyAllowedCharacters(string userName)
        {
            return userName.All(
                c => char.IsLetterOrDigit(c) || c == '.' || c == '_'
            );
        }

        private bool ContainSpecialCharactersOrUppercaseLetter(string password)
        {
            var specialChars = new[] { '.', '_', '!', '%' };

            bool hasSpecialChar = password.Any(c => specialChars.Contains(c));

            bool hasUpperCaseLetter = password.Any(c => char.IsUpper(c));

            return hasSpecialChar || hasUpperCaseLetter;
        }

        private bool ContainDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }

        private bool ValidRole(string role)
        {
            return role.ToLower() == "admin" || role.ToLower() == "user";
        }

    }
}
