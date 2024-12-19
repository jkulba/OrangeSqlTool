using Cocona;
using Microsoft.Extensions.Logging;

namespace Orange.Sql.Tool.Users;

public class UserInfoCommands
{
    private readonly IUserInfoService _service;
    private readonly ILogger<UserInfoCommands> _logger;

    public UserInfoCommands(IUserInfoService service, ILogger<UserInfoCommands> logger)
    {
        _service = service;
        _logger = logger;
    }

    [Command("list", Description = "Lists all users")]
    public async Task<int> GetAllUsers()
    {
        Console.WriteLine("Listing all users");
        _logger.LogInformation("Getting all users");
        Task<IEnumerable<UserInfo?>> users = _service.GetAllUsersAsync();

        foreach (var user in await users)
        {
            Console.WriteLine(user);
        }

        return 0;

    }
}