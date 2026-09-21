using System;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_ResetPassword;

// CU-03 main flow step 8: the code that arrived by email, the new password and
// its confirmation.
public sealed class GuiResetPassword : FormScreen
{
    private const int CardHeight = 328;
    private const int MaxCodeLength = 8;

    private readonly TextField _codeField;
    private readonly TextField _newPasswordField;
    private readonly TextField _confirmationField;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;

    public GuiResetPassword(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _codeField = new TextField { MaxLength = MaxCodeLength, Bounds = GetRow(0) };
        _newPasswordField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _confirmationField = new TextField { IsPassword = true, Bounds = GetRow(2) };
        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_codeField);
        RegisterField(_newPasswordField);
        RegisterField(_confirmationField);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ResetPasswordSubtitle;
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
        _codeField.Label = TextCatalog.ResetPasswordCodeLabel;
        _codeField.Placeholder = TextCatalog.ResetPasswordCodePlaceholder;
        _newPasswordField.Label = TextCatalog.ResetPasswordNewLabel;
        _newPasswordField.Placeholder = TextCatalog.ResetPasswordNewPlaceholder;
        _confirmationField.Label = TextCatalog.ResetPasswordConfirmLabel;
        _confirmationField.Placeholder = TextCatalog.ResetPasswordConfirmPlaceholder;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
