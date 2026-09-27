using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bastion.Presentation.Utils;

namespace Bastion.Presentation.Tests;

// Presses the controls of a screen the way a person would, without a window.
// A control raises its event from inside itself, so the test reaches the
// backing delegate the compiler writes for the event and calls it.
public static class ScreenDriver
{
    private const string ClickedEvent = "Clicked";

    private const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;

    public static IReadOnlyList<Button> GetButtons(object screen)
    {
        return GetControls<Button>(screen);
    }

    public static IReadOnlyList<TextField> GetFields(object screen)
    {
        return GetControls<TextField>(screen);
    }

    public static IReadOnlyList<DataRow> GetRows(object screen)
    {
        return GetControls<DataRow>(screen);
    }

    public static IReadOnlyList<SidebarMenu> GetMenus(object screen)
    {
        return GetControls<SidebarMenu>(screen);
    }

    public static IReadOnlyList<CheckBox> GetCheckBoxes(object screen)
    {
        return GetControls<CheckBox>(screen);
    }

    public static IReadOnlyList<CheckBox> GetCheckBoxes(Navigator navigator)
    {
        return GetCheckBoxes(ScreenOf(navigator));
    }

    // Everything a person can press: buttons, list rows, setting rows and any
    // other control that answers with a plain event of its own.
    public static IReadOnlyList<Control> GetClickables(object screen)
    {
        return GetControls<Control>(screen).Where(control => GetPressEvent(control) is not null).ToList();
    }

    // The screen the navigator has open. It answers with a guard instead of
    // letting a test silence the nullable warning at every press.
    public static IScreen ScreenOf(Navigator navigator)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        return navigator.Current
            ?? throw new InvalidOperationException("The navigator has no screen open.");
    }

    public static IReadOnlyList<Button> GetButtons(Navigator navigator)
    {
        return GetButtons(ScreenOf(navigator));
    }

    public static IReadOnlyList<Control> GetClickables(Navigator navigator)
    {
        return GetClickables(ScreenOf(navigator));
    }

    public static IReadOnlyList<SidebarMenu> GetMenus(Navigator navigator)
    {
        return GetMenus(ScreenOf(navigator));
    }

    public static void Click(Navigator navigator, string title)
    {
        Click(ScreenOf(navigator), title);
    }

    public static void ClickAt(Navigator navigator, int index)
    {
        ClickAt(ScreenOf(navigator), index);
    }

    public static void ClickRow(Navigator navigator, int index)
    {
        ClickRow(ScreenOf(navigator), index);
    }

    public static void ClickMenuItem(Navigator navigator, int index)
    {
        ClickMenuItem(ScreenOf(navigator), index);
    }

    public static void Type(Navigator navigator, int index, string text)
    {
        Type(ScreenOf(navigator), index, text);
    }

    public static void TickEveryBox(Navigator navigator)
    {
        TickEveryBox(ScreenOf(navigator));
    }

    public static void ConfirmDialog(Navigator navigator)
    {
        ConfirmDialog(ScreenOf(navigator));
    }

    public static void DismissDialog(Navigator navigator)
    {
        DismissDialog(ScreenOf(navigator));
    }

    // The label is what the person reads, so tests name the button by it
    // instead of by the private field behind it.
    public static Button FindButton(object screen, string title)
    {
        Button? button = GetButtons(screen).FirstOrDefault(
            candidate => string.Equals(candidate.Title, title, StringComparison.Ordinal));

        return button ?? throw new InvalidOperationException(
            $"No button titled '{title}' on {screen.GetType().Name}.");
    }

    public static void Click(object screen, string title)
    {
        Raise(FindButton(screen, title), ClickedEvent);
    }

    public static void ClickAt(object screen, int index)
    {
        Control control = GetClickables(screen)[index];
        EventInfo? press = GetPressEvent(control);

        if (press is null)
        {
            throw new InvalidOperationException($"{control.GetType().Name} cannot be pressed.");
        }

        Raise(control, press.Name);
    }

    public static void ClickRow(object screen, int index)
    {
        Raise(GetRows(screen)[index], ClickedEvent);
    }

    public static void ClickMenuItem(object screen, int index)
    {
        SidebarMenu menu = GetMenus(screen)[0];

        if (!menu.Items[index].IsEnabled || index == menu.SelectedIndex)
        {
            return;
        }

        menu.SelectedIndex = index;
        Raise(menu, nameof(SidebarMenu.ItemChosen), new SelectionChangedEventArgs { SelectedIndex = index });
    }

    public static void Type(object screen, int index, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        GetFields(screen)[index].SetText(text);
    }

    // A form that asks to accept something rejects the press until it is ticked.
    public static void TickEveryBox(object screen)
    {
        foreach (CheckBox box in GetControls<CheckBox>(screen))
        {
            box.IsChecked = true;
        }
    }

    // A dialog is not a control: it hangs off the screen that shows it and
    // answers with its own two events.
    public static void ConfirmDialog(object screen)
    {
        Raise(AsDialog(screen), nameof(MessageDialog.PrimaryChosen));
    }

    public static void DismissDialog(object screen)
    {
        Raise(AsDialog(screen), nameof(MessageDialog.SecondaryChosen));
    }

    public static bool IsShowingADialog(object? screen)
    {
        return screen is MessageScreen;
    }

    private static MessageDialog AsDialog(object screen)
    {
        ArgumentNullException.ThrowIfNull(screen);

        return screen is MessageScreen message
            ? message.Dialog
            : throw new InvalidOperationException($"{screen.GetType().Name} is not a dialog.");
    }

    // The one event a control declares for being pressed. A control whose
    // event carries data, such as a picker, is driven by its own helper.
    private static EventInfo? GetPressEvent(Control control)
    {
        return GetOwnEvents(control).FirstOrDefault(
            candidate => candidate.EventHandlerType == typeof(EventHandler));
    }

    // A control that declares an event answers for itself, so the search stops
    // there instead of reaching whatever it draws inside.
    private static bool IsRespondingByItself(Control control)
    {
        return GetOwnEvents(control).Length > 0;
    }

    private static EventInfo[] GetOwnEvents(Control control)
    {
        return control.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    }

    private static IReadOnlyList<T> GetControls<T>(object screen)
        where T : Control
    {
        ArgumentNullException.ThrowIfNull(screen);

        var found = new List<T>();
        Collect(screen, found, new HashSet<object>(ReferenceEqualityComparer.Instance));

        return found;
    }

    private static void Collect<T>(object owner, List<T> found, HashSet<object> seen)
        where T : Control
    {
        if (!seen.Add(owner))
        {
            return;
        }

        foreach (FieldInfo field in owner.GetType().GetFields(Hidden))
        {
            Consider(field.GetValue(owner), found, seen);
        }
    }

    private static void Consider<T>(object? value, List<T> found, HashSet<object> seen)
        where T : Control
    {
        if (value is T control)
        {
            found.Add(control);
        }

        if (value is Control nested)
        {
            if (!IsRespondingByItself(nested) || value is T)
            {
                Collect(nested, found, seen);
            }

            return;
        }

        if (value is IEnumerable many and not string)
        {
            foreach (object? item in many)
            {
                Consider(item, found, seen);
            }
        }
    }

    private static void Raise(object control, string eventName)
    {
        Raise(control, eventName, EventArgs.Empty);
    }

    private static void Raise(object control, string eventName, EventArgs arguments)
    {
        FieldInfo backing = control.GetType().GetField(eventName, Hidden)
            ?? throw new InvalidOperationException($"{control.GetType().Name} has no event {eventName}.");

        var handler = backing.GetValue(control) as Delegate;
        handler?.DynamicInvoke(control, arguments);
    }
}
