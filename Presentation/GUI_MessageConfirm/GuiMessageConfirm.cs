using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MessageConfirm;

// Asks before doing something that discards work or cannot be undone. Always
// has two options, and the one that keeps the current state is the secondary.
public sealed class GuiMessageConfirm : MessageScreen
{
    public GuiMessageConfirm()
        : base(DialogTone.Confirm)
    {
        Dialog.Title = TextCatalog.DialogConfirmTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogConfirmButton;
        Dialog.SecondaryLabel = TextCatalog.DialogCancelButton;
    }

    public void ShowWithLabels(string body, string primaryLabel, string secondaryLabel)
    {
        Show(body);
        Dialog.PrimaryLabel = primaryLabel;
        Dialog.SecondaryLabel = secondaryLabel;
    }
}
