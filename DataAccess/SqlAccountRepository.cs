using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Bastion.Domain;

namespace Bastion.DataAccess;

// CU-02 main flow steps 15 to 22. The account, the accepted terms and the
// verification token are one transaction: an account that exists without the
// terms it accepted would be a hole in the audit the use case asks for.
public sealed class SqlAccountRepository : IAccountRepository
{
    private const int DuplicateKeyError = 2627;
    private const int DuplicateIndexError = 2601;
    private const int FriendCodeAttempts = 5;
    private const int TokenBytes = 32;
    private const int VerificationHours = 24;

    private const string NicknameIndex = "UQ_Usuario_nickname";
    private const string EmailIndex = "UX_Usuario_correo";
    private const string FriendCodeIndex = "UX_Usuario_codigo_amigo";

    private readonly ISqlConnectionFactory _connections;

    public SqlAccountRepository(ISqlConnectionFactory connections)
    {
        ArgumentNullException.ThrowIfNull(connections);

        _connections = connections;
    }

    public async Task<RegistrationResult> RegisterAsync(NewAccount account, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(account);

        // The unique indexes are the ones that decide, so a taken nickname is
        // read from the rejection instead of from a question asked before.
        for (int attempt = 0; attempt < FriendCodeAttempts; attempt++)
        {
            RegistrationResult? result = await TryRegisterAsync(account, cancellation);

            if (result is not null)
            {
                return result;
            }
        }

        throw new InvalidOperationException("No free friend code was found after several attempts.");
    }

    public async Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken cancellation)
    {
        return await ExistsAsync("SELECT 1 FROM Usuario WHERE nickname = @value", nickname, cancellation);
    }

    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellation)
    {
        return await ExistsAsync("SELECT 1 FROM Usuario WHERE correo = @value", email, cancellation);
    }

    // Null means the friend code collided and the caller should try again.
    private async Task<RegistrationResult?> TryRegisterAsync(NewAccount account, CancellationToken cancellation)
    {
        await using SqlConnection connection = await _connections.OpenAsync(cancellation);
        await using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellation);
        string friendCode = FriendCode.Next();

        try
        {
            int accountId = await InsertAccountAsync(
                new Insertion(connection, transaction, account), friendCode, cancellation);

            await InsertTermsAsync(new Insertion(connection, transaction, account), accountId, cancellation);
            await InsertVerificationAsync(new Insertion(connection, transaction, account), accountId, cancellation);
            await transaction.CommitAsync(cancellation);

            return new RegistrationResult
            {
                Outcome = RegistrationOutcome.Registered,
                AccountId = accountId,
                FriendCode = friendCode
            };
        }
        catch (SqlException exception) when (IsDuplicate(exception))
        {
            await transaction.RollbackAsync(cancellation);

            return Explain(exception);
        }
    }

    private static async Task<int> InsertAccountAsync(
        Insertion insertion, string friendCode, CancellationToken cancellation)
    {
        // estado_cuenta keeps its default, PENDIENTE: the account is not
        // usable until the address is verified (CU-02 RN-09).
        await using SqlCommand command = insertion.CreateCommand(
            """
            INSERT INTO Usuario
                (tipo_cuenta, nickname, correo, contrasena_hash, contrasena_sal,
                 fecha_nacimiento, idioma_preferido, codigo_amigo)
            OUTPUT INSERTED.id_usuario
            VALUES
                ('REGISTRADA', @nickname, @email, @hash, @salt,
                 @birthDate, @language, @friendCode);
            """);

        command.Parameters.Add("@nickname", SqlDbType.NVarChar, 30).Value = insertion.Account.Nickname;
        command.Parameters.Add("@email", SqlDbType.NVarChar, 254).Value = insertion.Account.Email;
        command.Parameters.Add("@hash", SqlDbType.VarBinary, PasswordHasher.HashBytes).Value =
            insertion.Account.Password.Hash;
        command.Parameters.Add("@salt", SqlDbType.VarBinary, PasswordHasher.SaltBytes).Value =
            insertion.Account.Password.Salt;
        command.Parameters.Add("@birthDate", SqlDbType.Date).Value = insertion.Account.BirthDate.ToDateTime(default);
        command.Parameters.Add("@language", SqlDbType.VarChar, 10).Value = insertion.Account.Language;
        command.Parameters.Add("@friendCode", SqlDbType.Char, FriendCode.Length).Value = friendCode;

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
        command.Parameters.Add("@version", SqlDbType.VarChar, 20).Value = insertion.Account.TermsVersion;
        command.Parameters.Add("@language", SqlDbType.VarChar, 10).Value = insertion.Account.Language;
        command.Parameters.Add("@address", SqlDbType.VarChar, 45).Value = insertion.Account.Address;

        await command.ExecuteNonQueryAsync(cancellation);
    }

    // Only the hash is kept: the token itself travels in the message and is
    // never stored, so a copy of the table cannot activate an account.
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

    // A friend code clash is the server's own bad luck, not the person's, so
    // it answers null and the caller draws another code.
    private static RegistrationResult? Explain(SqlException exception)
    {
        if (exception.Message.Contains(NicknameIndex, StringComparison.Ordinal))
        {
            return new RegistrationResult { Outcome = RegistrationOutcome.NicknameTaken };
        }

        if (exception.Message.Contains(EmailIndex, StringComparison.Ordinal))
        {
            return new RegistrationResult { Outcome = RegistrationOutcome.EmailTaken };
        }

        return exception.Message.Contains(FriendCodeIndex, StringComparison.Ordinal) ? null : throw exception;
    }

    private sealed record Insertion(SqlConnection Connection, SqlTransaction Transaction, NewAccount Account)
    {
        public SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql, Connection, Transaction);
        }
    }
}
