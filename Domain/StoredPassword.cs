namespace Bastion.Domain;

// A password as the Usuario table keeps it: never the text, only the hash and
// the salt it was derived with.
public sealed record StoredPassword
{
    public required byte[] Hash { get; init; }

    public required byte[] Salt { get; init; }
}
