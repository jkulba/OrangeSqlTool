using Microsoft.Extensions.Logging;

namespace Orange.Sql.Tool.Users;

public class UserInfoCommands(ILogger<UserInfoCommands> logger)
{
    private readonly ILogger<UserInfoCommands> _logger = logger;
}