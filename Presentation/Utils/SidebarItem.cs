namespace Bastion.Presentation.Utils;

public sealed class SidebarItem
{
    public string Text { get; set; } = string.Empty;

    public bool IsEnabled { get; init; } = true;
}
