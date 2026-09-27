using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Bastion.Managers;

namespace Bastion.DataAccess;

// Peticiones sobre Usuario y lo que cuelga de un alta. Nada de aqui decide:
// escribe lo que le dan y traduce lo que la base contesta.
public sealed class AccountDao : IAccountDao
{
    private const int DuplicateKeyError = 2627;
    private const int DuplicateIndexError = 2601;
    private const int TokenBytes = 32;
    private const int VerificationHours = 24;

    private const string NicknameIndex = "UQ_Usuario_nickname";
    private const string EmailIndex = "UX_Usuario_correo";
    private const string FriendCodeIndex = "UX_Usuario_codigo_amigo";

    private readonly ISqlConnectionFactory _connections;

    public AccountDao(ISqlConnectionFactory connections)
    {
        ArgumentNullException.ThrowIfNull(connections);

        _connections = connections;
    }

    // Las tres filas van en una transaccion: una cuenta sin los terminos que
    // acepto seria un hueco en la auditoria que pide el CU-02.
    public async Task<AccountInsertResult> InsertAsync(AccountRow row, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(row);

        await using SqlConnection connection = await _connections.OpenAsync(cancellation);
        await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellation);

        try
        {
            int accountId = await InsertAccountAsync(new Insertion(connection, transaction, row), cancellation);

            await InsertTermsAsync(new Insertion(connection, transaction, row), accountId, cancellation);
            await InsertVerificationAsync(new Insertion(connection, transaction, row), accountId, cancellation);
            await transaction.CommitAsync(cancellation);

            return new AccountInsertResult { Result = AccountInsert.Written, AccountId = accountId };
        }
        catch (SqlException exception) when (IsDuplicate(exception))
        {
            await transaction.RollbackAsync(cancellation);

            return new AccountInsertResult { Result = Explain(exception) };
        }
    }

    public async Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation)
    {
        return await ExistsAsync("SELECT 1 FROM Usuario WHERE nickname = @value", nickname, cancellation);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation)
    {
        return await ExistsAsync("SELECT 1 FROM Usuario WHERE correo = @value", email, cancellation);
    }

    // estado_cuenta se queda con su valor por omision, PENDIENTE: la cuenta
    // no sirve hasta que se verifique el correo (CU-02 RN-09).
    private static async Task<int> InsertAccountAsync(Insertion insertion, CancellationToken cancellation)
    {
        await using SqlCommand command = insertion.CreateCommand(
            """
            INSERT INTO Usuario
                (tipo_cuenta, nombre, apellidos, nickname, correo, contrasena_hash, contrasena_sal,
                 fecha_nacimiento, idioma_preferido, codigo_amigo)
            OUTPUT INSERTED.id_usuario
            VALUES
                ('REGISTRADA', @firstName, @surnames, @nickname, @email, @hash, @salt,
                 @birthDate, @language, @friendCode);
            """);

        command.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = insertion.Row.FirstName;
        command.Parameters.Add("@surnames", SqlDbType.NVarChar, 100).Value = insertion.Row.Surnames;
        command.Parameters.Add("@nickname", SqlDbType.NVarChar, 30).Value = insertion.Row.Nickname;
        command.Parameters.Add("@email", SqlDbType.NVarChar, 254).Value = insertion.Row.Email;
        command.Parameters.Add("@hash", SqlDbType.VarBinary, insertion.Row.PasswordHash.Length).Value =
            insertion.Row.PasswordHash;
        command.Parameters.Add("@salt", SqlDbType.VarBinary, insertion.Row.PasswordSalt.Length).Value =
            insertion.Row.PasswordSalt;
        command.Parameters.Add("@birthDate", SqlDbType.Date).Value = insertion.Row.BirthDate.ToDateTime(default);
        command.Parameters.Add("@language", SqlDbType.VarChar, 10).Value = insertion.Row.Language;
        command.Parameters.Add("@friendCode", SqlDbType.Char, insertion.Row.FriendCode.Length).Value =
            insertion.Row.FriendCode;

        return (int)(await command.ExecuteScalarAsync(cancellation))!;
    }

    private static async Task InsertTermsAsync(Insertion insertion, int accountId, CancellationToken cancellation)
    {
        await using SqlCommand command = insertion.CreateCommand(
            """
            INSERT INTO AceptacionTerminos (id_usuario, version_terminos, idioma, direccion_ip)
            VALUES (@accountId, @version, @language, @address);
            """);

        command.Parameters.Add("@accountId", SqlDbType.Int).Value = accountId;
        command.Parameters.Add("@version", SqlDbType.VarChar, 20).Value = insertion.Row.TermsVersion;
        command.Parameters.Add("@language", SqlDbType.VarChar, 10).Value = insertion.Row.Language;
        command.Parameters.Add("@address", SqlDbType.VarChar, 45).Value = insertion.Row.Address;

        await command.ExecuteNonQueryAsync(cancellation);
    }

    // Solo se guarda el hash: el token viaja en el mensaje y nunca se
    // almacena, asi que una copia de la tabla no activa ninguna cuenta.
    private static async Task InsertVerificationAsync(
        Insertion insertion, int accountId, CancellationToken cancellation)
    {
        await using SqlCommand command = insertion.CreateCommand(
            """
            INSERT INTO TokenVerificacionCorreo (id_usuario, token_hash, proposito, fecha_expiracion)
            VALUES (@accountId, @tokenHash, 'ALTA', @expiresAt);
            """);

        command.Parameters.Add("@accountId", SqlDbType.Int).Value = accountId;
        command.Parameters.Add("@tokenHash", SqlDbType.VarBinary, TokenBytes).Value =
            SHA256.HashData(RandomNumberGenerator.GetBytes(TokenBytes));
        command.Parameters.Add("@expiresAt", SqlDbType.DateTime2).Value =
            DateTime.UtcNow.AddHours(VerificationHours);

        await command.ExecuteNonQueryAsync(cancellation);
    }

    private async Task<bool> ExistsAsync(string sql, string value, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        await using SqlConnection connection = await _connections.OpenAsync(cancellation);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@value", SqlDbType.NVarChar, 254).Value = value;

        return await command.ExecuteScalarAsync(cancellation) is not null;
    }

    private static bool IsDuplicate(SqlException exception)
    {
        return exception.Number is DuplicateKeyError or DuplicateIndexError;
    }

    private static AccountInsert Explain(SqlException exception)
    {
        if (exception.Message.Contains(NicknameIndex, StringComparison.Ordinal))
        {
            return AccountInsert.NicknameTaken;
        }

        if (exception.Message.Contains(EmailIndex, StringComparison.Ordinal))
        {
            return AccountInsert.EmailTaken;
        }

        return exception.Message.Contains(FriendCodeIndex, StringComparison.Ordinal)
            ? AccountInsert.FriendCodeTaken
            : throw exception;
    }

    private sealed record Insertion(SqlConnection Connection, SqlTransaction Transaction, AccountRow Row)
    {
        public SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql, Connection, Transaction);
        }
    }
}
