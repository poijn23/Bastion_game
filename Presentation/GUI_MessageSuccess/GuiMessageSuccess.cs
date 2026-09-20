using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MessageSuccess;

// Confirms that an operation finished. It is the only tone that does not use
// the accent color, so success is not read as another failure.
public sealed class GuiMessageSuccess : MessageScreen
{
    public GuiMessageSuccess()
        : base(DialogTone.Success)
    {
        Dialog.Title = TextCatalog.DialogSuccessTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }
}
