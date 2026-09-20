using System;

namespace Bastion.Presentation.Utils;

// What a screen needs to say to ask for a confirmation. The labels are optional
// because most confirmations use the generic pair from the catalog.
public sealed record ConfirmRequest
{
    public required string Body { get; init; }

    public required Action OnConfirm { get; init; }

    public string PrimaryLabel { get; init; } = string.Empty;

    public string SecondaryLabel { get; init; } = string.Empty;
}
