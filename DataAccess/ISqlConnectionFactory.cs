using Microsoft.Data.SqlClient;

namespace Bastion.DataAccess;

public interface ISqlConnectionFactory
{
    Task<SqlConnection> OpenAsync(CancellationToken cancellation);
}
