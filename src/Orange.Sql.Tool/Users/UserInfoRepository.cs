namespace Orange.Sql.Tool.Users;

public interface IUserInfoRepository
{
    Task<UserInfo?> GetUserByIdAsync(Guid id);
    Task<IReadOnlyList<UserInfo?>> GetAllUsersAsync();
    Task<UserInfo> CreateUserAsync(UserInfo user);
    Task<UserInfo> UpdateUserAsync(UserInfo user);
    Task<bool> DeleteUserByIdAsync(Guid id);

}
public class UserInfoRepository : IUserInfoRepository
{
    public Task<UserInfo> CreateUserAsync(UserInfo user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<UserInfo?>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserInfo?> GetUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UserInfo> UpdateUserAsync(UserInfo user)
    {
        throw new NotImplementedException();
    }
}
