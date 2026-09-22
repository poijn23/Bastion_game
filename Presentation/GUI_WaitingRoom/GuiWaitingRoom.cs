using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_WaitingRoom;

// CU-18 main flow step 4 (host) and CU-19 main flow step 3 (guest). One class
// for both roles: the host waits with "Iniciar partida" disabled until the
// rest are ready, the guest gets "Estoy listo" instead.
public sealed class GuiWaitingRoom : FormScreen
{
    public const string HostRole = "host";
    public const string GuestRole = "guest";

    private const int CodeValueWidth = 220;
    private const int CodeLabelHeight = 20;
    private const int CodeValueTop = CodeLabelHeight + 4;
    private const int SectionGap = 20;
    private const int SeatCount = 2;
    private const int SeatHeight = 54;
    private const int SeatGap = 8;
    private const int ChatLineHeight = 20;
    private const int ChatLineGap = 4;

    private const int SeatsLabelTop = CodeValueTop + Theme.FieldHeight + SectionGap;
    private const int SeatsTop = SeatsLabelTop + LabelSpace;
    private const int ChatTitleTop = SeatsTop + (SeatCount * (SeatHeight + SeatGap)) + SectionGap;
    private const int ChatSampleTop = ChatTitleTop + ChatLineHeight + ChatLineGap;
    private const int ChatFieldTop = ChatSampleTop + ChatLineHeight + ChatLineGap;
    private const int CardHeight = Theme.CardPadding + ChatFieldTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly bool _isHost;
    private readonly TextLine _codeLabel;
    private readonly ValueBox _codeValue;
    private readonly TextLine _seatsLabel;
    private readonly List<DataRow> _seats = [];
    private readonly TextLine _chatTitle;
    private readonly TextLine _chatSample;
    private readonly TextField _chatField;
    private readonly Button _actionButton;
    private readonly Button _leaveButton;
    private bool _isReady;

    public GuiWaitingRoom(INavigator navigator, string role)
        : base(navigator, WideCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(role);

        _isHost = role == HostRole;
        int top = Card.Y + Theme.CardPadding;

        _codeLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, CodeLabelHeight)
        };
        _codeValue = new ValueBox
        {
            Bounds = new Rectangle(ContentX, top + CodeValueTop, CodeValueWidth, Theme.FieldHeight)
        };

        _seatsLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + SeatsLabelTop, ContentWidth, LabelSpace)
        };

        for (int i = 0; i < SeatCount; i++)
        {
            int seatTop = top + SeatsTop + (i * (SeatHeight + SeatGap));
            var seat = new DataRow
            {
                IsHighlighted = i == 0,
                Bounds = new Rectangle(ContentX, seatTop, ContentWidth, SeatHeight)
            };
            _seats.Add(seat);
            Register(seat);
        }

        _chatTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ChatTitleTop, ContentWidth, ChatLineHeight)
        };
        _chatSample = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, top + ChatSampleTop, ContentWidth, ChatLineHeight)
        };
        _chatField = new TextField
        {
            Bounds = new Rectangle(ContentX, top + ChatFieldTop, ContentWidth, Theme.FieldHeight)
        };

        _actionButton = CreatePrimaryButton(false);
        _leaveButton = CreateSecondaryButton();
        _actionButton.IsEnabled = !_isHost;
        _actionButton.Clicked += OnActionClicked;
        _leaveButton.Clicked += OnLeaveClicked;

        Register(_codeLabel);
        Register(_codeValue);
        Register(_seatsLabel);
        Register(_chatTitle);
        Register(_chatSample);
        RegisterField(_chatField);
        Register(_actionButton);
        Register(_leaveButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.WaitingRoomSubtitle;
    }

    private void OnActionClicked(object? sender, EventArgs e)
    {
        if (_isHost)
        {
            return;
        }

        _isReady = !_isReady;
        _actionButton.Title = _isReady ? TextCatalog.CommonCancelButton : TextCatalog.WaitingRoomReadyButton;
    }

    private void OnLeaveClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.WaitingRoomLeaveBody,
            PrimaryLabel = TextCatalog.WaitingRoomLeaveButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnLeaveConfirmed
        });
    }

    private void OnLeaveConfirmed()
    {
        Navigator.GoBack();
    }

    // Placeholder room: the code and the other seat come from the server.
    protected override void ApplyTexts()
    {
        _codeLabel.Text = TextCatalog.WaitingRoomCodeLabel;
        _codeValue.Value = TextCatalog.PrivateMatchCodePlaceholder;
        _seatsLabel.Text = TextCatalog.WaitingRoomPlayersLabel;

        _seats[0].Title = TestAccount.Nickname;
        _seats[0].Value = _isHost ? TextCatalog.WaitingRoomHostTag : TextCatalog.WaitingRoomYouTag;

        _seats[1].Title = _isHost ? TextCatalog.WaitingRoomFreeSlot : TextCatalog.RankingPlayerSample;
        _seats[1].Subtitle = _isHost ? TextCatalog.WaitingRoomHostNote : string.Empty;
        _seats[1].Value = _isHost ? string.Empty : TextCatalog.WaitingRoomHostTag;

        _chatTitle.Text = TextCatalog.MatchChatTitle;
        _chatSample.Text = TextCatalog.WaitingRoomChatSample;
        _chatField.Placeholder = TextCatalog.WaitingRoomChatPlaceholder;

        _actionButton.Title = GetActionLabel();
        _leaveButton.Title = TextCatalog.WaitingRoomLeaveButton;
    }

    private string GetActionLabel()
    {
        if (_isHost)
        {
            return TextCatalog.WaitingRoomStartButton;
        }

        return _isReady ? TextCatalog.CommonCancelButton : TextCatalog.WaitingRoomReadyButton;
    }
}
