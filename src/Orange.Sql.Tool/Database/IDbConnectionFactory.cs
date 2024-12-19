using System.Data;

namespace Orange.Sql.Tool.Database;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}