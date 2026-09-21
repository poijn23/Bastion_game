using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Customize;

// CU-14 main flow step 1. The six slots and the balance. The rotating board
// preview belongs to the game renderer and is not part of this layer yet.
public sealed class GuiCustomize : FormScreen
{
    private const int CardHeight = 392;
    private const int SlotCount = 6;
    private const int SlotHeight = 46;
    private const int SlotGap = 10;
    private const int SectionGap = 20;
    private const int PreviewWidth = 320;
    private const int BalanceHeight = 68;

    private readonly List<Button> _slots = [];
    private readonly AvatarBox _preview;
    private readonly StatTile _balance;
    private readonly Button _equipButton;
    private readonly Button _backButton;

    public GuiCustomize(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;
        int listWidth = ContentWidth - PreviewWidth - SectionGap;

        for (int index = 0; index < SlotCount; index++)
        {
            var slot = new Button
            {
                Style = ButtonStyle.Secondary,
                Bounds = new Rectangle(ContentX, top + (index * (SlotHeight + SlotGap)), listWidth, SlotHeight)
            };

            _slots.Add(slot);
            Register(slot);
        }

        int previewX = Card.Right - Theme.CardPadding - PreviewWidth;
        int previewHeight = (SlotCount * (SlotHeight + SlotGap)) - SlotGap - BalanceHeight - SectionGap;

        _preview = new AvatarBox { Bounds = new Rectangle(previewX, top, PreviewWidth, previewHeight) };
        _balance = new StatTile
        {
            Bounds = new Rectangle(previewX, top + previewHeight + SectionGap, PreviewWidth, BalanceHeight)
        };

        _equipButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_equipButton, _backButton]);
        _equipButton.Clicked += OnEquipClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_preview);
        Register(_balance);
        Register(_equipButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.CustomizeSubtitle;
    }

    private void OnEquipClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    protected override void ApplyTexts()
    {
        IReadOnlyList<string> names = BuildSlotNames();

        for (int index = 0; index < _slots.Count; index++)
        {
            _slots[index].Title = names[index];
        }

        _preview.IconLabel = TextCatalog.CustomizePreviewPlaceholder;
        _balance.Caption = TextCatalog.CoinHistoryBalanceCaption;
        _balance.Value = TextCatalog.CoinHistoryBalanceSample;
        _equipButton.Title = TextCatalog.CustomizeEquipButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private static IReadOnlyList<string> BuildSlotNames()
    {
        return
        [
            TextCatalog.CustomizeSlotPawn,
            TextCatalog.CustomizeSlotWalls,
            TextCatalog.CustomizeSlotBoard,
            TextCatalog.CustomizeSlotFrame,
            TextCatalog.CustomizeSlotEmotes,
            TextCatalog.CustomizeSlotTitle
        ];
    }
}
