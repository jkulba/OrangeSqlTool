using FluentValidation;

namespace Orange.Sql.Tool.Users;

public class UserInfoValidator : AbstractValidator<UserInfo>
{
    public UserInfoValidator()
    {
        RuleFor(x => x.EmployeeNum).NotEmpty();

        RuleFor(x => x.FirstName).NotEmpty();
        
        RuleFor(x => x.LastName).NotEmpty();
    }
}