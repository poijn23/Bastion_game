namespace Bastion.Presentation.Utils;

// What the registration form collected. The screen fills it and hands it to
// the gateway; nothing above this layer knows how it travels.
public sealed record NewAccountRequest
{
    public required string Nickname { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required bool AcceptsTerms { get; init; }
}
