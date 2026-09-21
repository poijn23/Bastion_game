using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Presentation.Utils;

// Chrome, focus order and button geometry shared by every account screen.
public abstract class FormScreen : IScreen
{
    protected const int NarrowCardWidth = 560;
    protected const int WideCardWidth = 900;
    protected const int RowSpacing = 98;
    protected const int LabelSpace = 22;
    protected const int Gutter = 32;

    private const int ButtonGap = 22;
    private const int ButtonSpacing = 12;

    private readonly List<Control> _controls = [];
    private readonly List<TextField> _focusableFields = [];
    private readonly DropDown _languagePicker;

    protected FormScreen(INavigator navigator, int cardWidth, int cardHeight)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        Navigator = navigator;

        int x = (Theme.WindowWidth - cardWidth) / 2;
        Card = new Rectangle(x, ScreenChrome.CardTop, cardWidth, cardHeight);

        int primaryTop = Card.Bottom + ButtonGap;
        PrimaryButtonBounds = new Rectangle(x, primaryTop, cardWidth, Theme.PrimaryButtonHeight);

        int secondaryTop = primaryTop + Theme.PrimaryButtonHeight + ButtonSpacing;
        SecondaryButtonBounds = new Rectangle(x, secondaryTop, cardWidth, Theme.SecondaryButtonHeight);

        _languagePicker = LanguagePicker.Create();
        _languagePicker.SelectionChanged += OnLanguageSelected;
    }

    protected INavigator Navigator { get; }

    protected Rectangle Card { get; }

    protected Rectangle PrimaryButtonBounds { get; }

    protected Rectangle SecondaryButtonBounds { get; }

    protected int ContentX => Card.X + Theme.CardPadding;

    protected int ContentWidth => Card.Width - (Theme.CardPadding * 2);

    protected int FirstRowTop => Card.Y + Theme.CardPadding + LabelSpace;

    protected int ColumnWidth => (ContentWidth - Gutter) / 2;

    protected virtual int RowPitch => RowSpacing;

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        bool pickerWasOpen = _languagePicker.IsOpen;
        _languagePicker.Update(input);

        if (pickerWasOpen && input.HasClicked)
        {
            return;
        }

        if (input.HasClicked)
        {
            ResolveFocus(input);
        }

        if (input.IsKeyNewlyPressed(Keys.Tab))
        {
            MoveFocus(IsShiftPressed(input));
        }

        foreach (Control control in _controls)
        {
            control.Update(input);
        }
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        ScreenChrome.Draw(canvas, GetSubtitle());
        canvas.Shapes.DrawRoundedRectangle(Card, Theme.CardCornerRadius, Theme.Card);

        foreach (Control control in _controls)
        {
            control.Draw(canvas);
        }

        _languagePicker.Draw(canvas);
    }

    protected abstract string GetSubtitle();

    protected abstract void ApplyTexts();

    // Draw order is registration order, so anything that can overlap the rest
    // is registered last.
    protected void Register(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        _controls.Add(control);
    }

    protected void RegisterField(TextField field)
    {
        ArgumentNullException.ThrowIfNull(field);

        _controls.Add(field);
        _focusableFields.Add(field);
    }

    protected Button CreatePrimaryButton(bool hasArrow)
    {
        return new Button
        {
            Style = ButtonStyle.Primary,
            HasArrow = hasArrow,
            Bounds = PrimaryButtonBounds
        };
    }

    protected Button CreateSecondaryButton()
    {
        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = SecondaryButtonBounds
        };
    }

    protected void FocusFirstField()
    {
        if (_focusableFields.Count > 0)
        {
            _focusableFields[0].IsFocused = true;
        }
    }

    protected Rectangle GetRow(int index)
    {
        return new Rectangle(ContentX, FirstRowTop + (index * RowPitch), ContentWidth, Theme.FieldHeight);
    }

    protected Rectangle GetCell(int row, bool isRightColumn)
    {
        int x = isRightColumn ? ContentX + ColumnWidth + Gutter : ContentX;

        return new Rectangle(x, FirstRowTop + (row * RowPitch), ColumnWidth, Theme.FieldHeight);
    }

    private void OnLanguageSelected(object? sender, SelectionChangedEventArgs e)
    {
        LanguagePicker.Apply(e.SelectedIndex);
        _languagePicker.Options = LanguagePicker.GetNames();
        ApplyTexts();
    }

    private void ResolveFocus(InputState input)
    {
        foreach (TextField field in _focusableFields)
        {
            field.IsFocused = field.Bounds.Contains(input.MousePosition);
        }
    }

    private void MoveFocus(bool isBackwards)
    {
        if (_focusableFields.Count == 0)
        {
            return;
        }

        int current = _focusableFields.FindIndex(IsFieldFocused);
        int step = isBackwards ? -1 : 1;
        int next = current < 0 ? 0 : (current + step + _focusableFields.Count) % _focusableFields.Count;

        for (int i = 0; i < _focusableFields.Count; i++)
        {
            _focusableFields[i].IsFocused = i == next;
        }
    }

    private static bool IsFieldFocused(TextField field)
    {
        return field.IsFocused;
    }

    private static bool IsShiftPressed(InputState input)
    {
        return input.IsKeyPressed(Keys.LeftShift) || input.IsKeyPressed(Keys.RightShift);
    }
}
