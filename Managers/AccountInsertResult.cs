namespace Bastion.Managers;

public sealed record AccountInsertResult
{
    public required AccountInsert Result { get; init; }

    public int AccountId { get; init; }
}
