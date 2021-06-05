using FluentValidation;
using SmplBank.Application.Requests.Commands;
using SmplBank.Domain.Repository;

namespace SmplBank.Application.Requests.Validators
{
    public class InsertUserValidator : AbstractValidator<InsertUserCommand>
    {
        private readonly IUserRepository userRepository;

        public InsertUserValidator(IUserRepository userRepository) : this()
        {
            this.userRepository = userRepository;
        }

        protected InsertUserValidator() : base()
        {
            RuleFor(x => x.Username)
                   .NotEmpty();

            RuleFor(x => x.Username)
                   .MustAsync(async (username, _) =>
                   {
                       var doesUsernameExist = await this.userRepository.CheckExist(user => user.Username, username);

                       return !doesUsernameExist;
                   })
                   .When(x => !string.IsNullOrEmpty(x.Username))
                   .WithMessage("Username already exist.");

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
