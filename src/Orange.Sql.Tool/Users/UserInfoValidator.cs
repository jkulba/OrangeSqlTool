using FluentValidation;

namespace Orange.Sql.Tool.Users;

public class UserInfoValidator : AbstractValidator<UserInfo>
{
    public UserInfoValidator()
    {
        RuleFor(x => x.EmployeeNum)
            .NotEmpty()
            .NotNull()
            .WithMessage(x => $"The EmployeeNum can not be null or empty.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .NotNull()
            .WithMessage(x => $"The FirstName can not be null or empty.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .NotNull()
            .WithMessage(x => $"The LastName can not be null or empty.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress()
            .WithMessage("The Email address is not valid.");
       
    }
}