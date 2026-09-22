using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation.GUI_TutorialIndex;

// CU-41 main flow step 1. Seven lessons, the one being worked on highlighted
// (RN of "continuar destacado"); opening a lesson itself needs a board, which
// does not exist yet, so each row is informational until then.
public sealed class GuiTutorialIndex : FormScreen
{
    private const int LessonCount = 7;
    private const int DoneCount = 2;
    private const int InProgressIndex = 2;

    private const int ProgressHeight = 20;
    private const int SectionGap = 14;
    private const int RowHeight = 40;
    private const int RowGap = 6;

    private const int RowsTop = ProgressHeight + SectionGap;
    private const int ContentHeight = RowsTop + (LessonCount * (RowHeight + RowGap)) - RowGap;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _progress;
    private readonly List<DataRow> _lessons = [];
    private readonly Button _practiceButton;
    private readonly Button _exitButton;

    public GuiTutorialIndex(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _progress = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ProgressHeight)
        };

        for (int i = 0; i < LessonCount; i++)
        {
            var row = new DataRow
            {
                IsHighlighted = i == InProgressIndex,
                Bounds = new Rectangle(ContentX, top + RowsTop + (i * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };
            _lessons.Add(row);
            Register(row);
        }

        _practiceButton = CreatePrimaryButton(true);
        _exitButton = CreateSecondaryButton();
        _practiceButton.Clicked += OnPracticeClicked;
        _exitButton.Clicked += OnExitClicked;

        Register(_progress);
        Register(_practiceButton);
        Register(_exitButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.TutorialIndexSubtitle;
    }

    private void OnPracticeClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AIDifficulty);
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Placeholder progress: which lessons are done comes from the server.
    protected override void ApplyTexts()
    {
        _progress.Text = string.Format(TextCatalog.TutorialIndexProgressFormat, DoneCount, LessonCount);

        string[] names =
        [
            TextCatalog.LessonBoardAndGoal,
            TextCatalog.LessonMovePawn,
            TextCatalog.LessonJumpOpponent,
            TextCatalog.LessonPlaceWalls,
            TextCatalog.LessonWallsNoTrap,
            TextCatalog.LessonClock,
            TextCatalog.LessonFourPlayers
        ];

        for (int i = 0; i < _lessons.Count; i++)
        {
            _lessons[i].Title = names[i];
            _lessons[i].Value = GetStateTag(i);
        }

        _practiceButton.Title = TextCatalog.TutorialIndexPracticeButton;
        _exitButton.Title = TextCatalog.TutorialIndexExitButton;
    }

    private static string GetStateTag(int index)
    {
        if (index < DoneCount)
        {
            return TextCatalog.TutorialIndexDoneTag;
        }

        return index == InProgressIndex ? TextCatalog.TutorialIndexInProgressTag : string.Empty;
    }
}
