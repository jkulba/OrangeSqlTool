using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Orange.Sql.Tool.Database;

internal sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
        await connection.OpenAsync();
        return connection;
    }
}