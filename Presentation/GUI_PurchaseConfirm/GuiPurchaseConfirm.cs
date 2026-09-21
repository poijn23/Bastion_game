using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_PurchaseConfirm;

// CU-38 main flow step 4. The price, the balance now and the balance after,
// which CU-38 RN-07 asks to show before charging anything.
public sealed class GuiPurchaseConfirm : FormScreen
{
    private const int CardHeight = 330;
    private const int SectionGap = 20;

    private readonly ValueBox _itemBox;
    private readonly ValueBox _priceBox;
    private readonly ValueBox _balanceBox;
    private readonly ValueBox _remainingBox;
    private readonly Button _confirmButton;
    private readonly Button _cancelButton;

    public GuiPurchaseConfirm(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / 2;

        _itemBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, ContentWidth, Theme.FieldHeight));
        _priceBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _balanceBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _remainingBox = CreateValueBox(
            new Rectangle(ContentX, FirstRowTop + (RowSpacing * 2), ContentWidth, Theme.FieldHeight));

        _confirmButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _confirmButton.Clicked += OnConfirmClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_itemBox);
        Register(_priceBox);
        Register(_balanceBox);
        Register(_remainingBox);
        Register(_confirmButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PurchaseConfirmSubtitle;
    }

    private void OnConfirmClicked(object? sender, EventArgs e)
    {
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder amounts: the price and the balance come from the server.
    protected override void ApplyTexts()
    {
        _itemBox.Label = TextCatalog.PurchaseConfirmItemLabel;
        _itemBox.Value = TextCatalog.ShopItemSample;
        _priceBox.Label = TextCatalog.PurchaseConfirmPriceLabel;
        _priceBox.Value = TextCatalog.PurchaseConfirmPriceSample;
        _balanceBox.Label = TextCatalog.PurchaseConfirmBalanceLabel;
        _balanceBox.Value = TextCatalog.CoinHistoryBalanceSample;
        _remainingBox.Label = TextCatalog.PurchaseConfirmRemainingLabel;
        _remainingBox.Value = TextCatalog.PurchaseConfirmRemainingSample;
        _confirmButton.Title = TextCatalog.PurchaseConfirmButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }
}
