using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_EditProfile;

// CU-12 main flow step 1. The nickname is shown but not edited here: it has its
// own use case because the change is unique and irreversible (CU-12 RN-01).
public sealed class GuiEditProfile : FormScreen
{
    private const int WideCardWidth = 760;
    private const int CardHeight = 452;
    private const int IconColumns = 8;
    private const int IconRows = 4;
    private const int IconSize = 44;
    private const int IconGap = 8;
    private const int SectionGap = 22;
    private const int LinkFieldCount = 2;
    private const int MaxLinkLength = 120;
    private const int CheckBoxHeight = 24;

    private readonly List<AvatarBox> _icons = [];
    private readonly ValueBox _nicknameBox;
    private readonly List<TextField> _linkFields = [];
    private readonly CheckBox _spectatorsCheckBox;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public GuiEditProfile(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int gridTop = Card.Y + Theme.CardPadding;

        for (int index = 0; index < IconColumns * IconRows; index++)
        {
            var icon = new AvatarBox
            {
                IsSelected = index == 0,
                Bounds = GetIconBounds(index, gridTop)
            };

            _icons.Add(icon);
            Register(icon);
        }

        int gridBottom = gridTop + (IconRows * (IconSize + IconGap)) - IconGap;
        int column = GetColumnWidth();

        _nicknameBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, gridBottom + SectionGap + LabelSpace, column, Theme.FieldHeight)
        };

        for (int index = 0; index < LinkFieldCount; index++)
        {
            var field = new TextField
            {
                MaxLength = MaxLinkLength,
                Bounds = new Rectangle(
                    ContentX + column + SectionGap,
                    gridBottom + SectionGap + LabelSpace + (index * RowSpacing),
                    column,
                    Theme.FieldHeight)
            };

            _linkFields.Add(field);
            RegisterField(field);
        }

        _spectatorsCheckBox = new CheckBox
        {
            IsChecked = true,
            Bounds = new Rectangle(
                ContentX,
                _nicknameBox.Bounds.Bottom + RowSpacing - Theme.FieldHeight,
                column,
                CheckBoxHeight)
        };

        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        LayOutActionsInRow([_saveButton, _cancelButton]);
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_nicknameBox);
        Register(_spectatorsCheckBox);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.EditProfileSubtitle;
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
    }

    // CU-12 FA: leaving with unsaved changes asks before discarding them.
    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.EditProfileDiscardBody,
            PrimaryLabel = TextCatalog.RegisterDiscardButton,
            SecondaryLabel = TextCatalog.RegisterKeepEditingButton,
            OnConfirm = GoBack
        });
    }

    private void GoBack()
    {
        Navigator.GoBack();
    }

    private void ApplyTexts()
    {
        for (int index = 0; index < _icons.Count; index++)
        {
            _icons[index].IconLabel = (index + 1).ToString();
        }

        _nicknameBox.Label = TextCatalog.EditProfileNicknameLabel;
        _linkFields[0].Label = TextCatalog.EditProfileLinkLabel;
        _linkFields[0].Placeholder = TextCatalog.EditProfileLinkPlaceholder;
        _linkFields[1].Label = TextCatalog.EditProfileSecondLinkLabel;
        _linkFields[1].Placeholder = TextCatalog.EditProfileLinkPlaceholder;
        _spectatorsCheckBox.Text = TextCatalog.EditProfileSpectatorsLabel;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private int GetColumnWidth()
    {
        return (ContentWidth - SectionGap) / 2;
    }

    private Rectangle GetIconBounds(int index, int top)
    {
        int column = index % IconColumns;
        int row = index / IconColumns;
        int x = ContentX + (column * (IconSize + IconGap));
        int y = top + (row * (IconSize + IconGap));

        return new Rectangle(x, y, IconSize, IconSize);
    }
}
