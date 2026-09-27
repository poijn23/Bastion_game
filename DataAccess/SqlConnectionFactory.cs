using Microsoft.Data.SqlClient;

namespace Bastion.DataAccess;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public async Task<SqlConnection> OpenAsync(CancellationToken cancellation)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellation);

        return connection;
    }
}
