using System;

namespace Bastion.Presentation.Utils;

public sealed class SelectionChangedEventArgs : EventArgs
{
    public required int SelectedIndex { get; init; }
}
