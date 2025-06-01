using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Commands.Account;
using FluentValidation;

namespace doft.Application.Validators.Account
{
    public class SignInCommandValidator : AbstractValidator<LoginCommand>
    {
        public SignInCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
                
        }
    }
}