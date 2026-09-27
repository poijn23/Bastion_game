using Bastion.Domain;

namespace Bastion.DataAccess;

// Everything a row of Usuario needs that the person supplied, plus what the
// server adds around it. Travels as one piece because a registration is one
// transaction, not seven arguments.
public sealed record NewAccount
{
    public required string Nickname { get; init; }

    public required string Email { get; init; }

    public required StoredPassword Password { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required string Language { get; init; }

    public required string TermsVersion { get; init; }

    public required string Address { get; init; }
}
