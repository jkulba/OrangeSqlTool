using Dapper;
using FluentValidation;
using Orange.Sql.Tool.Database;
using Orange.Sql.Tool.Validation;

namespace Orange.Sql.Tool.Users;

public interface IUserInfoService
{
    Task<UserInfo?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserInfo?>> GetAllUsersAsync();
    Task<Result<UserInfo, ValidationFailed>> CreateUserAsync(UserInfo user);
    Task<Result<UserInfo?, ValidationFailed>> UpdateUserAsync(UserInfo user);
    Task<bool> DeleteUserByIdAsync(Guid id);

}
public class UserInfoService : IUserInfoService
{
    private readonly IValidator<UserInfo> _validator;
    private readonly IDbConnectionFactory _connectionFactory;

    public UserInfoService(IValidator<UserInfo> validator, IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    /// <summary>
    /// Method used to return UserInfo from Users table.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Orange.Sql.Tool.Users.UserInfo</returns>
    public async Task<UserInfo?> GetUserByIdAsync(Guid id)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        var userInfo = await dbConnection.QuerySingleOrDefaultAsync<UserInfo>(
            "SELECT * FROM users WHERE UserId = @id", new { id });
        return userInfo;
    }

    /// <summary>
    /// Method used to get all the UserInfo from the Users table.
    /// </summary>
    /// <returns>IEnumerable of UserInfo</returns>
    public async Task<IEnumerable<UserInfo?>> GetAllUsersAsync()
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        return await dbConnection.QueryAsync<UserInfo>("SELECT * FROM users");
    }

    /// <summary>
    /// Method used to create new UserInfo in Users table.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Orange.Sql.Tool.Users.UserInfo</returns>
    public async Task<Result<UserInfo, ValidationFailed>> CreateUserAsync(UserInfo user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }
        
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        await dbConnection.ExecuteAsync(
            """
            INSERT INTO users (EmployeeNum, FirstName, LastName, Email, UtcCreatedAt, CreatedBy, IsEnabled)
            VALUES (@EmployeeNum, @FirstName, @LastName, @Email, @UtcCreatedAt, @CreatedBy, @IsEnabled)
            """, user);

        return user;
    }

    /// <summary>
    /// Method used to update a single UserInfo in the Users table.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Orange.Sql.Tool.Users.UserInfo</returns>
    public async Task<Result<UserInfo?, ValidationFailed>> UpdateUserAsync(UserInfo user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        var existingUser = await GetUserByIdAsync(user.UserId);
        if (existingUser is null)
        {
            return default(UserInfo);
        }
        
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        await dbConnection.ExecuteAsync(
            """
            UPDATE users SET EmployeeNum=@EmployeeNum, FirstName=@FirstName, LastName=@LastName, Email=@Email, UtcUpdatedAt=@UtcCreatedAt, UpdatedBy=@UpdatedBy, IsEnabled=@IsEnabled
            WHERE UserId = @UserId
            """, user);
        
        return user;
    }

    /// <summary>
    /// Method used to remove UserInfo from Users table.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>boolean</returns>
    public async Task<bool> DeleteUserByIdAsync(Guid id)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync(); 
        var result = await dbConnection.ExecuteAsync("DELETE FROM users where UserId = @id", new { id }); 
        return result > 0;
    }
}
