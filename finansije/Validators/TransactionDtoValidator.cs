using finansije.Models;
using FluentValidation;

namespace finansije.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator() 
        { 
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be a positive value");

            RuleFor(x => x.DateTime)
                .NotEmpty().WithMessage("Date and time are required")
                .Must(BeInPast).WithMessage("Date of transaction cannot be in the future");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category id is required");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .Must(ValidType).WithMessage("Transaction type must be 'income' or 'expense'");
        }
        private bool ValidType(string type)
        {
            return type.ToLower() == "income" || type.ToLower() == "expense";
        }
        private static bool BeInPast(DateTime dateTime)
        {
            return dateTime < DateTime.Today;
        }
    }
}
