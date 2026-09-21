using System;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ChangePassword;

// CU-09 main flow step 1. The current password is asked again because changing
// credentials closes every other open session.
public sealed class GuiChangePassword : FormScreen
{
    private const int CardHeight = 328;

    private readonly TextField _currentField;
    private readonly TextField _newField;
    private readonly TextField _confirmationField;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public GuiChangePassword(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _currentField = new TextField { IsPassword = true, Bounds = GetRow(0) };
        _newField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _confirmationField = new TextField { IsPassword = true, Bounds = GetRow(2) };
        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_currentField);
        RegisterField(_newField);
        RegisterField(_confirmationField);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangePasswordSubtitle;
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        _currentField.Label = TextCatalog.ChangePasswordCurrentLabel;
        _currentField.Placeholder = TextCatalog.ChangePasswordCurrentPlaceholder;
        _newField.Label = TextCatalog.ChangePasswordNewLabel;
        _newField.Placeholder = TextCatalog.ChangePasswordNewPlaceholder;
        _confirmationField.Label = TextCatalog.ChangePasswordConfirmLabel;
        _confirmationField.Placeholder = TextCatalog.ChangePasswordConfirmPlaceholder;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
