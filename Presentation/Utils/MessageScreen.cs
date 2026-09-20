using System;

namespace Bastion.Presentation.Utils;

// Base of the four message screens. They differ only in tone and in which
// labels they take from the catalog, so the loop lives here once.
public abstract class MessageScreen : IScreen
{
    protected MessageScreen(DialogTone tone)
    {
        Dialog = new MessageDialog(tone);
    }

    public MessageDialog Dialog { get; }

    public void Show(string body)
    {
        Dialog.Body = body;
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Dialog.Update(input);
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        Dialog.Draw(canvas);
    }
}
