using finansije.Models;
using FluentValidation;

namespace finansije.Validators
{
    public class AddressInfoDtoValidator : AbstractValidator<AddressInfoDto>
    {
        public AddressInfoDtoValidator() 
        {
            RuleFor(x => x.Street)
               .NotEmpty().WithMessage("Street is required");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required");

        }
    }
}
