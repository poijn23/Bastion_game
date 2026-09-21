using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_BoxPurchaseConfirm;

// CU-39 main flow step 4. Besides the price it shows how many boxes have come
// without a rare item and when the guarantee triggers (CU-39 RN-05).
public sealed class GuiBoxPurchaseConfirm : FormScreen
{
    private const int CardHeight = 330;
    private const int SectionGap = 20;
    private const int NoticeHeight = 76;

    private readonly ValueBox _priceBox;
    private readonly ValueBox _remainingBox;
    private readonly NoticeBox _guaranteeNotice;
    private readonly Button _confirmButton;
    private readonly Button _cancelButton;

    public GuiBoxPurchaseConfirm(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / 2;

        _priceBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight));
        _remainingBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop, column, Theme.FieldHeight));

        _guaranteeNotice = new NoticeBox
        {
            Bounds = new Rectangle(
                ContentX,
                FirstRowTop + Theme.FieldHeight + SectionGap,
                ContentWidth,
                NoticeHeight)
        };

        _confirmButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _confirmButton.Clicked += OnConfirmClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_priceBox);
        Register(_remainingBox);
        Register(_guaranteeNotice);
        Register(_confirmButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.BoxPurchaseSubtitle;
    }

    private void OnConfirmClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder amounts: the price and the streak come from the server.
    protected override void ApplyTexts()
    {
        _priceBox.Label = TextCatalog.PurchaseConfirmPriceLabel;
        _priceBox.Value = TextCatalog.BoxPurchasePriceSample;
        _remainingBox.Label = TextCatalog.PurchaseConfirmRemainingLabel;
        _remainingBox.Value = TextCatalog.BoxPurchaseRemainingSample;
        _guaranteeNotice.Text = TextCatalog.BoxPurchaseGuaranteeNotice;
        _confirmButton.Title = TextCatalog.BoxPurchaseConfirmButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
