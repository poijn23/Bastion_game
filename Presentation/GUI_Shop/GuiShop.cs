using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Shop;

// CU-38 main flow step 1. The balance stays visible because every price on the
// grid is read against it.
public sealed class GuiShop : FormScreen
{
    private const int CardHeight = 400;
    private const int BalanceWidth = 180;
    private const int BalanceHeight = 68;
    private const int SectionGap = 20;
    private const int TileColumns = 4;
    private const int TileRows = 2;
    private const int TileHeight = 132;
    private const int TileGap = 14;

    private readonly StatTile _balance;
    private readonly List<AvatarBox> _items = [];
    private readonly Button _buyButton;
    private readonly Button _backButton;

    public GuiShop(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _balance = new StatTile
        {
            Bounds = new Rectangle(Card.Right - Theme.CardPadding - BalanceWidth, top, BalanceWidth, BalanceHeight)
        };

        int gridTop = top + BalanceHeight + SectionGap;

        for (int index = 0; index < TileColumns * TileRows; index++)
        {
            var item = new AvatarBox
            {
                IsSelected = index == 0,
                Bounds = GetTileBounds(index, gridTop)
            };

            _items.Add(item);
            Register(item);
        }

        _buyButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_buyButton, _backButton]);
        _buyButton.Clicked += OnBuyClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_balance);
        Register(_buyButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ShopSubtitle;
    }

    private void OnBuyClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.PurchaseConfirm);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder catalogue: the items come from the server.
    protected override void ApplyTexts()
    {
        _balance.Caption = TextCatalog.CoinHistoryBalanceCaption;
        _balance.Value = TextCatalog.CoinHistoryBalanceSample;

        foreach (AvatarBox item in _items)
        {
            item.IconLabel = TextCatalog.ShopItemSample;
        }

        _buyButton.Title = TextCatalog.ShopBuyButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    // Only the width follows the card. The height cannot, because the two rows
    // have to keep fitting between the balance and the bottom of the card.
    private int GetTileWidth()
    {
        return (ContentWidth - (TileGap * (TileColumns - 1))) / TileColumns;
    }

    private Rectangle GetTileBounds(int index, int top)
    {
        int width = GetTileWidth();
        int column = index % TileColumns;
        int row = index / TileColumns;

        return new Rectangle(
            ContentX + (column * (width + TileGap)),
            top + (row * (TileHeight + TileGap)),
            width,
            TileHeight);
    }
}
