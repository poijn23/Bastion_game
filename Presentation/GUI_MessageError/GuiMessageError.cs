using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_MessageError;

// Reports a failure the player cannot correct by retyping: no connection,
// malformed response, server incident. Used by 38 of the 49 use cases.
public sealed class GuiMessageError : MessageScreen
{
    public GuiMessageError()
        : base(DialogTone.Error)
    {
        Dialog.Title = TextCatalog.DialogErrorTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }

    // CU-02 EX-01 and EX-04 offer a retry and carry an incident identifier.
    public void ShowRetryable(string body, string incident)
    {
        Show(body);
        Dialog.Detail = incident;
        Dialog.SecondaryLabel = TextCatalog.DialogRetryButton;
    }
}
