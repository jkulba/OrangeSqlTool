using FluentValidation;

namespace Orange.Sql.Tool.Users;

public class UserInfoValidator : AbstractValidator<UserInfo>
{
    public UserInfoValidator()
    {
        RuleFor(user => user.EmployeeNum).NotEmpty().MaximumLength(50);
        RuleFor(user => user.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(user => user.LastName).NotEmpty().MaximumLength(50);
        RuleFor(user => user.Email).NotEmpty().MaximumLength(50).EmailAddress();
        RuleFor(user => user.CreatedBy).NotEmpty().MaximumLength(50);
        RuleFor(user => user.UpdatedBy).NotEmpty().MaximumLength(50);
    }

}