using Dapper;
using FluentValidation;
using InterpolatedSql.Dapper;
using Microsoft.Extensions.Logging;
using Orange.Sql.Tool.Database;
using Orange.Sql.Tool.Validation;

namespace Orange.Sql.Tool.Users;

public interface IUserInfoService
{
    Task<UserInfo?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserInfo?>> GetAllUsersSortedByDateCreatedAsync(SortDirection sortDirection);
    Task<IEnumerable<UserInfo?>> GetAllUsersSortedByLastNameAsync(SortDirection sortDirection);
    Task<IEnumerable<UserInfo?>> GetAllUsersAsync();
    Task<Result<UserInfo, ValidationFailed>> CreateUserAsync(UserInfo user);
    Task<Result<UserInfo?, ValidationFailed>> UpdateUserAsync(UserInfo user);
    Task<bool> DeleteUserByIdAsync(Guid id);

}
public class UserInfoService : IUserInfoService
{
    private readonly ILogger<UserInfoService> _logger;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<UserInfo> _validator;

    public UserInfoService(ILogger<UserInfoService> logger, IDbConnectionFactory connectionFactory, IValidator<UserInfo> validator)
    {
        _logger = logger;
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
        var query = dbConnection.SqlBuilder($"SELECT * FROM users WHERE UserId = {id}").Build();
         _logger.LogInformation(query.Sql);
        return await query.QueryFirstOrDefaultAsync<UserInfo>();
        // var userInfo = await dbConnection.QuerySingleOrDefaultAsync<UserInfo>(
        //     "SELECT * FROM users WHERE UserId = @id", new { id });
        // return userInfo;
    }

    public async Task<IEnumerable<UserInfo?>> GetAllUsersSortedByDateCreatedAsync(SortDirection sortDirection)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        
        var query = dbConnection.SqlBuilder($"SELECT * FROM users");
        
        switch (sortDirection)
        {
            case SortDirection.Ascending: 
                query += $" ORDER BY UtcCreatedAt ASC"; 
                break; 
            case SortDirection.Descending: 
                query += $" ORDER BY UtcCreatedAt DESC"; 
                break; 
            default: 
                throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
        }
        return query.Query<UserInfo>();
    }

    public async Task<IEnumerable<UserInfo?>> GetAllUsersSortedByLastNameAsync(SortDirection sortDirection)
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        
        string queryString = "SELECT * FROM users";

        switch (sortDirection)
        {
            case SortDirection.Ascending: 
                queryString += " ORDER BY LastName ASC"; 
                break; 
            case SortDirection.Descending: 
                queryString += " ORDER BY LastName DESC"; 
                break; 
            default: 
                throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
        }

        return await dbConnection.QueryAsync<UserInfo>(queryString);

    }

    /// <summary>
    /// Method used to get all the UserInfo from the Users table.
    /// </summary>
    /// <returns>IEnumerable of UserInfo</returns>
    public async Task<IEnumerable<UserInfo?>> GetAllUsersAsync()
    {
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        var query = dbConnection.SqlBuilder($"SELECT * FROM users").Build();
        _logger.LogInformation(query.Sql);
        return await query.QueryAsync<UserInfo>();
    }

    /// <summary>
    /// Method used to create new UserInfo in Users table.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Orange.Sql.Tool.Users.UserInfo</returns>
    public async Task<Result<UserInfo, ValidationFailed>> CreateUserAsync(UserInfo user)
    {
        var userInfoValidator = new UserInfoValidator();
        await userInfoValidator.ValidateAndThrowAsync(user);  // Any validation errors will return with a list of errors.
        
        using var dbConnection = await _connectionFactory.CreateConnectionAsync();
        
        user.UtcCreatedAt = DateTime.UtcNow;
        
        var cmd = dbConnection.SqlBuilder(
            $@" INSERT INTO users (EmployeeNum, FirstName, LastName, Email, UtcCreatedAt, CreatedBy, IsEnabled) 
            OUTPUT inserted.* 
            VALUES ({user.EmployeeNum}, {user.FirstName}, {user.LastName}, {user.Email}, {user.UtcCreatedAt}, {user.CreatedBy}, {user.IsEnabled}); "); 
        
        var createdUser = await cmd.QuerySingleAsync<UserInfo>();
        
        return createdUser;
        
        // var cmd = dbConnection.SqlBuilder($"INSERT INTO users (EmployeeNum, FirstName, LastName, Email, UtcCreatedAt, CreatedBy, IsEnabled) VALUES ({user.EmployeeNum}, {user.FirstName}, {user.LastName}, {user.Email}, {user.UtcCreatedAt}, {user.CreatedBy}, {user.IsEnabled})");
        // await cmd.ExecuteAsync();
        //
        // return user;
    }

    /// <summary>
    /// Method used to update a single UserInfo in the Users table.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Orange.Sql.Tool.Users.UserInfo</returns>
    public async Task<Result<UserInfo?, ValidationFailed>> UpdateUserAsync(UserInfo user)
    {

        var existingUser = await GetUserByIdAsync(user.UserId);
        if (existingUser is null)
        {
            return default(UserInfo);
        }
        
        user.UtcUpdatedAt = DateTime.UtcNow;

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
        
        var cmd = dbConnection.SqlBuilder($"DELETE FROM Orders WHERE UserId = {id};");
        var deletedRows = await cmd.ExecuteAsync();
        
        return deletedRows > 0;
    }
}


