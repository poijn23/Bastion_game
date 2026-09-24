using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_AIMatchEnd;

// CU-27 main flow step 3. The result plus a review of three mistakes, each
// meant to open its board position once a board exists to open it on.
public sealed class GuiAIMatchEnd : FormScreen
{
    private const bool IsSampleVictory = true;
    private const int MistakeCount = 3;

    private const int ResultHeight = 20;
    private const int SectionGap = 18;
    private const int TileHeight = 76;
    private const int TileGap = 14;
    private const int TileCount = 2;
    private const int ReviewLabelHeight = LabelSpace;
    private const int RowHeight = 46;
    private const int RowGap = 8;

    private const int TilesTop = ResultHeight + SectionGap;
    private const int ReviewLabelTop = TilesTop + TileHeight + SectionGap;
    private const int RowsTop = ReviewLabelTop + ReviewLabelHeight;
    private const int ContentHeight = RowsTop + (MistakeCount * (RowHeight + RowGap)) - RowGap;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _result;
    private readonly StatTile _experienceTile;
    private readonly StatTile _movesTile;
    private readonly TextLine _reviewLabel;
    private readonly List<DataRow> _mistakes = [];
    private readonly Button _backButton;

    public GuiAIMatchEnd(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _result = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ResultHeight)
        };

        int tileWidth = (ContentWidth - TileGap) / TileCount;
        _experienceTile = new StatTile { Bounds = new Rectangle(ContentX, top + TilesTop, tileWidth, TileHeight) };
        _movesTile = new StatTile
        {
            Bounds = new Rectangle(ContentX + tileWidth + TileGap, top + TilesTop, tileWidth, TileHeight)
        };

        _reviewLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ReviewLabelTop, ContentWidth, ReviewLabelHeight)
        };

        for (int i = 0; i < MistakeCount; i++)
        {
            var row = new DataRow
            {
                Bounds = new Rectangle(ContentX, top + RowsTop + (i * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };
            _mistakes.Add(row);
            Register(row);
        }

        _backButton = CreatePrimaryButton(false);
        _backButton.Clicked += OnBackClicked;

        Register(_result);
        Register(_experienceTile);
        Register(_movesTile);
        Register(_reviewLabel);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AIMatchEndSubtitle;
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MainMenu);
    }

    // Placeholder result: the outcome and the review come from the server.
    protected override void ApplyTexts()
    {
        _result.Text = IsSampleVictory ? TextCatalog.AIMatchEndVictoryTitle : TextCatalog.AIMatchEndDefeatTitle;

        _experienceTile.Value = TextCatalog.MatchEndExperienceSample;
        _experienceTile.Caption = TextCatalog.AIMatchEndExperienceCaption;
        _movesTile.Value = TestProfile.AverageMoves.ToString("N0");
        _movesTile.Caption = TextCatalog.AIMatchEndMovesCaption;

        _reviewLabel.Text = TextCatalog.AIMatchEndReviewLabel;

        string[] mistakes =
        [
            TextCatalog.AIMatchEndMistakeOne, TextCatalog.AIMatchEndMistakeTwo, TextCatalog.AIMatchEndMistakeThree
        ];

        for (int i = 0; i < _mistakes.Count; i++)
        {
            _mistakes[i].Title = string.Format(TextCatalog.AIMatchEndMistakeFormat, i + 1);
            _mistakes[i].Subtitle = mistakes[i];
        }

        _backButton.Title = TextCatalog.MatchEndBackMenuButton;
    }
}
