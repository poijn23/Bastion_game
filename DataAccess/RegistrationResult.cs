namespace Bastion.DataAccess;

public sealed record RegistrationResult
{
    public required RegistrationOutcome Outcome { get; init; }

    public int AccountId { get; init; }

    public string FriendCode { get; init; } = string.Empty;
}
