namespace Bastion.Managers;

// Una fila de Usuario ya resuelta: la contrasena viene derivada y el codigo
// de amigo sorteado. El DAO la escribe sin decidir nada sobre ella.
public sealed record AccountRow
{
    public required string FirstName { get; init; }

    public required string Surnames { get; init; }

    public required string Nickname { get; init; }

    public required string Email { get; init; }

    public required byte[] PasswordHash { get; init; }

    public required byte[] PasswordSalt { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required string Language { get; init; }

    public required string FriendCode { get; init; }

    public required string TermsVersion { get; init; }

    public required string Address { get; init; }
}
