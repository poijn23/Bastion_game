using System;
using Microsoft.Xna.Framework;
using Bastion.Presentation.GUI_MatchEnd;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_Match;

// CU-17 main flow step 7. The board itself belongs to the game renderer,
// which does not exist yet (Domain and Service are empty); this is the frame
// around it: whose turn it is, the two actions that are neither a move nor a
// wall (CU-22, CU-23), and the match chat.
public sealed class GuiMatch : FormScreen
{
    private const int SampleTurn = 3;

    private const int HeaderHeight = 22;
    private const int SectionGap = 14;
    private const int PlayerRowHeight = 46;
    private const int PlayerRowGap = 8;
    private const int BoardHeight = 90;
    private const int ChatTitleHeight = 20;
    private const int ChatLineHeight = 18;
    private const int ChatLineGap = 4;

    private const int PlayersTop = HeaderHeight + SectionGap;
    private const int ChipsTop = PlayersTop + (PlayerRowHeight * 2) + PlayerRowGap + SectionGap;
    private const int BoardTop = ChipsTop + Theme.ChipHeight + SectionGap;
    private const int ChatTop = BoardTop + BoardHeight + SectionGap;
    private const int ChatFieldTop = ChatTop + ChatTitleHeight + ChatLineGap + ChatLineHeight + ChatLineGap;
    private const int CardHeight = Theme.CardPadding + ChatFieldTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly TextLine _turnLabel;
    private readonly TextLine _clockLabel;
    private readonly DataRow _selfRow;
    private readonly DataRow _rivalRow;
    private readonly ChipRow _actionChips;
    private readonly PanelBox _board;
    private readonly TextLine _chatTitle;
    private readonly TextLine _chatSample;
    private readonly TextField _chatField;
    private readonly Button _drawButton;
    private readonly Button _resignButton;
    private int _actionIndex;

    public GuiMatch(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _turnLabel = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HeaderHeight)
        };
        _clockLabel = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsRightAligned = true,
            Bounds = new Rectangle(ContentX, top + 2, ContentWidth, HeaderHeight)
        };

        _selfRow = new DataRow
        {
            IsHighlighted = true,
            Bounds = new Rectangle(ContentX, top + PlayersTop, ContentWidth, PlayerRowHeight)
        };
        int rivalRowTop = top + PlayersTop + PlayerRowHeight + PlayerRowGap;
        _rivalRow = new DataRow
        {
            Bounds = new Rectangle(ContentX, rivalRowTop, ContentWidth, PlayerRowHeight)
        };

        _actionChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + ChipsTop, ContentWidth, Theme.ChipHeight)
        };
        _actionChips.ChipChosen += OnActionChosen;

        _board = new PanelBox
        {
            IsDashed = true,
            Bounds = new Rectangle(ContentX, top + BoardTop, ContentWidth, BoardHeight)
        };

        int chatSampleTop = top + ChatTop + ChatTitleHeight + ChatLineGap;
        _chatTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ChatTop, ContentWidth, ChatTitleHeight)
        };
        _chatSample = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, chatSampleTop, ContentWidth, ChatLineHeight)
        };
        _chatField = new TextField
        {
            Bounds = new Rectangle(ContentX, top + ChatFieldTop, ContentWidth, Theme.FieldHeight)
        };

        _drawButton = CreateSecondaryButton();
        _resignButton = CreatePrimaryButton(false);
        LayOutActionsInRow([_drawButton, _resignButton]);
        _drawButton.Clicked += OnDrawClicked;
        _resignButton.Clicked += OnResignClicked;

        Register(_turnLabel);
        Register(_clockLabel);
        Register(_selfRow);
        Register(_rivalRow);
        Register(_actionChips);
        Register(_board);
        Register(_chatTitle);
        Register(_chatSample);
        RegisterField(_chatField);
        Register(_drawButton);
        Register(_resignButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MatchSubtitle;
    }

    private void OnActionChosen(object? sender, SelectionChangedEventArgs e)
    {
        _actionIndex = e.SelectedIndex;
        RefreshActionChips();
    }

    private void OnDrawClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.MatchDrawBody,
            PrimaryLabel = TextCatalog.MatchDrawConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnDrawOffered
        });
    }

    private void OnDrawOffered()
    {
        Navigator.ShowMessage(DialogTone.Confirm, TextCatalog.MatchDrawSentBody);
    }

    private void OnResignClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.MatchResignBody,
            PrimaryLabel = TextCatalog.MatchResignConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnResignConfirmed
        });
    }

    private void OnResignConfirmed()
    {
        Navigator.GoTo(ScreenId.MatchEnd, GuiMatchEnd.DefeatOutcome);
    }

    // Placeholder match: the board, the turn and the chat come from the server.
    protected override void ApplyTexts()
    {
        _turnLabel.Text = string.Format(TextCatalog.MatchTurnFormat, SampleTurn);
        _clockLabel.Text = TextCatalog.MatchClockSample;

        _selfRow.Title = TestAccount.Nickname;
        _selfRow.Value = TextCatalog.MatchWallsSample;
        _rivalRow.Title = TextCatalog.RankingPlayerSample;
        _rivalRow.Value = TextCatalog.MatchWallsSample;

        RefreshActionChips();
        _board.Title = TextCatalog.MatchBoardPlaceholder;

        _chatTitle.Text = TextCatalog.MatchChatTitle;
        _chatSample.Text = TextCatalog.MatchChatSampleOne;
        _chatField.Placeholder = TextCatalog.MatchChatPlaceholder;

        _drawButton.Title = TextCatalog.MatchDrawButton;
        _resignButton.Title = TextCatalog.MatchResignButton;
    }

    private void RefreshActionChips()
    {
        _actionChips.Items.Clear();
        string[] actions = [TextCatalog.MatchMoveChip, TextCatalog.MatchWallChip];

        for (int i = 0; i < actions.Length; i++)
        {
            _actionChips.Items.Add(new Chip { Text = actions[i], IsSelected = i == _actionIndex });
        }
    }
}
