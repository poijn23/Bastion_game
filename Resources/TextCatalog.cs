using System.Globalization;
using System.Resources;

namespace Bastion.Resources;

// Written by hand on purpose: the Visual Studio generator does not run under
// dotnet build, and the team works on Rider and VS Code over macOS and Linux.
public static class TextCatalog
{
    private static readonly ResourceManager _manager =
        new("Bastion.Resources.TextCatalog", typeof(TextCatalog).Assembly);

    public static string WindowTitle => GetText(nameof(WindowTitle));

    public static string GameTitle => GetText(nameof(GameTitle));

    public static string InterfaceLanguageLabel => GetText(nameof(InterfaceLanguageLabel));

    public static string SpanishLanguageCode => GetText(nameof(SpanishLanguageCode));

    public static string EnglishLanguageCode => GetText(nameof(EnglishLanguageCode));

    public static string SpanishMexicoLanguageName => GetText(nameof(SpanishMexicoLanguageName));

    public static string EnglishLanguageName => GetText(nameof(EnglishLanguageName));

    public static string RegisterSubtitle => GetText(nameof(RegisterSubtitle));

    public static string RegisterNicknameLabel => GetText(nameof(RegisterNicknameLabel));

    public static string RegisterNicknamePlaceholder => GetText(nameof(RegisterNicknamePlaceholder));

    public static string RegisterEmailLabel => GetText(nameof(RegisterEmailLabel));

    public static string RegisterEmailPlaceholder => GetText(nameof(RegisterEmailPlaceholder));

    public static string RegisterPasswordLabel => GetText(nameof(RegisterPasswordLabel));

    public static string RegisterPasswordPlaceholder => GetText(nameof(RegisterPasswordPlaceholder));

    public static string RegisterConfirmationLabel => GetText(nameof(RegisterConfirmationLabel));

    public static string RegisterConfirmationPlaceholder => GetText(nameof(RegisterConfirmationPlaceholder));

    public static string RegisterBirthDateLabel => GetText(nameof(RegisterBirthDateLabel));

    public static string RegisterDayPlaceholder => GetText(nameof(RegisterDayPlaceholder));

    public static string RegisterMonthPlaceholder => GetText(nameof(RegisterMonthPlaceholder));

    public static string RegisterYearPlaceholder => GetText(nameof(RegisterYearPlaceholder));

    public static string RegisterAccountLanguageLabel => GetText(nameof(RegisterAccountLanguageLabel));

    public static string RegisterTermsText => GetText(nameof(RegisterTermsText));

    public static string RegisterTermsLinkText => GetText(nameof(RegisterTermsLinkText));

    public static string RegisterCreateButton => GetText(nameof(RegisterCreateButton));

    public static string RegisterCreateButtonDetail => GetText(nameof(RegisterCreateButtonDetail));

    public static string RegisterCancelButton => GetText(nameof(RegisterCancelButton));

    public static string DialogErrorTitle => GetText(nameof(DialogErrorTitle));

    public static string DialogWarningTitle => GetText(nameof(DialogWarningTitle));

    public static string DialogConfirmTitle => GetText(nameof(DialogConfirmTitle));

    public static string DialogSuccessTitle => GetText(nameof(DialogSuccessTitle));

    public static string DialogAcceptButton => GetText(nameof(DialogAcceptButton));

    public static string DialogRetryButton => GetText(nameof(DialogRetryButton));

    public static string DialogConfirmButton => GetText(nameof(DialogConfirmButton));

    public static string DialogCancelButton => GetText(nameof(DialogCancelButton));

    public static string DialogSignInButton => GetText(nameof(DialogSignInButton));

    public static string CommonSaveButton => GetText(nameof(CommonSaveButton));

    public static string CommonCancelButton => GetText(nameof(CommonCancelButton));

    public static string CommonBackButton => GetText(nameof(CommonBackButton));

    public static string LoginSubtitle => GetText(nameof(LoginSubtitle));

    public static string LoginIdentifierLabel => GetText(nameof(LoginIdentifierLabel));

    public static string LoginIdentifierPlaceholder => GetText(nameof(LoginIdentifierPlaceholder));

    public static string LoginPasswordLabel => GetText(nameof(LoginPasswordLabel));

    public static string LoginPasswordPlaceholder => GetText(nameof(LoginPasswordPlaceholder));

    public static string LoginForgotLink => GetText(nameof(LoginForgotLink));

    public static string LoginSignInButton => GetText(nameof(LoginSignInButton));

    public static string LoginSignInDetail => GetText(nameof(LoginSignInDetail));

    public static string LoginCreateAccountButton => GetText(nameof(LoginCreateAccountButton));

    public static string LoginGuestButton => GetText(nameof(LoginGuestButton));

    public static string ForgotPasswordSubtitle => GetText(nameof(ForgotPasswordSubtitle));

    public static string ForgotPasswordNotice => GetText(nameof(ForgotPasswordNotice));

    public static string ForgotPasswordEmailLabel => GetText(nameof(ForgotPasswordEmailLabel));

    public static string ForgotPasswordEmailPlaceholder => GetText(nameof(ForgotPasswordEmailPlaceholder));

    public static string ForgotPasswordSendButton => GetText(nameof(ForgotPasswordSendButton));

    public static string ResetPasswordSubtitle => GetText(nameof(ResetPasswordSubtitle));

    public static string ResetPasswordCodeLabel => GetText(nameof(ResetPasswordCodeLabel));

    public static string ResetPasswordCodePlaceholder => GetText(nameof(ResetPasswordCodePlaceholder));

    public static string ResetPasswordNewLabel => GetText(nameof(ResetPasswordNewLabel));

    public static string ResetPasswordNewPlaceholder => GetText(nameof(ResetPasswordNewPlaceholder));

    public static string ResetPasswordConfirmLabel => GetText(nameof(ResetPasswordConfirmLabel));

    public static string ResetPasswordConfirmPlaceholder => GetText(nameof(ResetPasswordConfirmPlaceholder));

    public static string ChangePasswordSubtitle => GetText(nameof(ChangePasswordSubtitle));

    public static string ChangePasswordCurrentLabel => GetText(nameof(ChangePasswordCurrentLabel));

    public static string ChangePasswordCurrentPlaceholder => GetText(nameof(ChangePasswordCurrentPlaceholder));

    public static string ChangePasswordNewLabel => GetText(nameof(ChangePasswordNewLabel));

    public static string ChangePasswordNewPlaceholder => GetText(nameof(ChangePasswordNewPlaceholder));

    public static string ChangePasswordConfirmLabel => GetText(nameof(ChangePasswordConfirmLabel));

    public static string ChangePasswordConfirmPlaceholder => GetText(nameof(ChangePasswordConfirmPlaceholder));

    public static string ChangeEmailSubtitle => GetText(nameof(ChangeEmailSubtitle));

    public static string ChangeEmailNewLabel => GetText(nameof(ChangeEmailNewLabel));

    public static string ChangeEmailNewPlaceholder => GetText(nameof(ChangeEmailNewPlaceholder));

    public static string ChangeEmailPasswordLabel => GetText(nameof(ChangeEmailPasswordLabel));

    public static string ChangeEmailPasswordPlaceholder => GetText(nameof(ChangeEmailPasswordPlaceholder));

    public static string ChangeEmailNotice => GetText(nameof(ChangeEmailNotice));

    public static string ChangeNicknameSubtitle => GetText(nameof(ChangeNicknameSubtitle));

    public static string ChangeNicknameNotice => GetText(nameof(ChangeNicknameNotice));

    public static string ChangeNicknameCurrentLabel => GetText(nameof(ChangeNicknameCurrentLabel));

    public static string ChangeNicknameNewLabel => GetText(nameof(ChangeNicknameNewLabel));

    public static string ChangeNicknameNewPlaceholder => GetText(nameof(ChangeNicknameNewPlaceholder));

    public static string ChangeNicknameSaveButton => GetText(nameof(ChangeNicknameSaveButton));

    public static string DeleteAccountSubtitle => GetText(nameof(DeleteAccountSubtitle));

    public static string DeleteAccountNotice => GetText(nameof(DeleteAccountNotice));

    public static string DeleteAccountPasswordLabel => GetText(nameof(DeleteAccountPasswordLabel));

    public static string DeleteAccountPasswordPlaceholder => GetText(nameof(DeleteAccountPasswordPlaceholder));

    public static string DeleteAccountContinueButton => GetText(nameof(DeleteAccountContinueButton));

    public static string DeleteAccountConfirmSubtitle => GetText(nameof(DeleteAccountConfirmSubtitle));

    public static string DeleteAccountConfirmNotice => GetText(nameof(DeleteAccountConfirmNotice));

    public static string DeleteAccountWordLabel => GetText(nameof(DeleteAccountWordLabel));

    public static string DeleteAccountWordPlaceholder => GetText(nameof(DeleteAccountWordPlaceholder));

    public static string DeleteAccountDeleteButton => GetText(nameof(DeleteAccountDeleteButton));

    public static string ActiveSessionsSubtitle => GetText(nameof(ActiveSessionsSubtitle));

    public static string ActiveSessionsCurrentBadge => GetText(nameof(ActiveSessionsCurrentBadge));

    public static string ActiveSessionsCloseButton => GetText(nameof(ActiveSessionsCloseButton));

    public static string ActiveSessionsCloseAllButton => GetText(nameof(ActiveSessionsCloseAllButton));

    public static string ActiveSessionsDeviceSample => GetText(nameof(ActiveSessionsDeviceSample));

    public static string ActiveSessionsLastUseSample => GetText(nameof(ActiveSessionsLastUseSample));

    public static string AccountSettingsSubtitle => GetText(nameof(AccountSettingsSubtitle));

    public static string AccountSettingsNicknameEntry => GetText(nameof(AccountSettingsNicknameEntry));

    public static string AccountSettingsPasswordEntry => GetText(nameof(AccountSettingsPasswordEntry));

    public static string AccountSettingsEmailEntry => GetText(nameof(AccountSettingsEmailEntry));

    public static string AccountSettingsSessionsEntry => GetText(nameof(AccountSettingsSessionsEntry));

    public static string AccountSettingsDeleteEntry => GetText(nameof(AccountSettingsDeleteEntry));

    public static string RegisterDiscardBody => GetText(nameof(RegisterDiscardBody));

    public static string RegisterDiscardButton => GetText(nameof(RegisterDiscardButton));

    public static string RegisterKeepEditingButton => GetText(nameof(RegisterKeepEditingButton));

    public static string ProfileSubtitle => GetText(nameof(ProfileSubtitle));

    public static string ProfileAvatarPlaceholder => GetText(nameof(ProfileAvatarPlaceholder));

    public static string ProfileMatchesCaption => GetText(nameof(ProfileMatchesCaption));

    public static string ProfileWinRateCaption => GetText(nameof(ProfileWinRateCaption));

    public static string ProfileStreakCaption => GetText(nameof(ProfileStreakCaption));

    public static string ProfileEloCaption => GetText(nameof(ProfileEloCaption));

    public static string ProfileEditButton => GetText(nameof(ProfileEditButton));

    public static string EditProfileSubtitle => GetText(nameof(EditProfileSubtitle));

    public static string EditProfileNicknameLabel => GetText(nameof(EditProfileNicknameLabel));

    public static string EditProfileLinkLabel => GetText(nameof(EditProfileLinkLabel));

    public static string EditProfileSecondLinkLabel => GetText(nameof(EditProfileSecondLinkLabel));

    public static string EditProfileLinkPlaceholder => GetText(nameof(EditProfileLinkPlaceholder));

    public static string EditProfileSpectatorsLabel => GetText(nameof(EditProfileSpectatorsLabel));

    public static string EditProfileDiscardBody => GetText(nameof(EditProfileDiscardBody));

    public static string PlayerCardSubtitle => GetText(nameof(PlayerCardSubtitle));

    public static string PlayerCardHeadToHeadCaption => GetText(nameof(PlayerCardHeadToHeadCaption));

    public static string PlayerCardHeadToHeadSample => GetText(nameof(PlayerCardHeadToHeadSample));

    public static string PlayerCardMatchSample => GetText(nameof(PlayerCardMatchSample));

    public static string PlayerCardAddFriendButton => GetText(nameof(PlayerCardAddFriendButton));

    public static string AddFriendSubtitle => GetText(nameof(AddFriendSubtitle));

    public static string AddFriendSearchLabel => GetText(nameof(AddFriendSearchLabel));

    public static string AddFriendSearchPlaceholder => GetText(nameof(AddFriendSearchPlaceholder));

    public static string AddFriendSearchButton => GetText(nameof(AddFriendSearchButton));

    public static string AddFriendSuggestionSample => GetText(nameof(AddFriendSuggestionSample));

    public static string AddFriendSuggestionReason => GetText(nameof(AddFriendSuggestionReason));

    public static string FriendsSubtitle => GetText(nameof(FriendsSubtitle));

    public static string FriendsNameSample => GetText(nameof(FriendsNameSample));

    public static string FriendsOnlineLabel => GetText(nameof(FriendsOnlineLabel));

    public static string FriendsOfflineLabel => GetText(nameof(FriendsOfflineLabel));

    public static string FriendsAddButton => GetText(nameof(FriendsAddButton));

    public static string MatchHistorySubtitle => GetText(nameof(MatchHistorySubtitle));

    public static string MatchHistoryWin => GetText(nameof(MatchHistoryWin));

    public static string MatchHistoryLoss => GetText(nameof(MatchHistoryLoss));

    public static string MatchHistoryRowSample => GetText(nameof(MatchHistoryRowSample));

    public static string MatchHistoryGainSample => GetText(nameof(MatchHistoryGainSample));

    public static string MatchHistoryLossSample => GetText(nameof(MatchHistoryLossSample));

    public static string CoinHistorySubtitle => GetText(nameof(CoinHistorySubtitle));

    public static string CoinHistoryBalanceCaption => GetText(nameof(CoinHistoryBalanceCaption));

    public static string CoinHistoryBalanceSample => GetText(nameof(CoinHistoryBalanceSample));

    public static string CoinHistoryGainSample => GetText(nameof(CoinHistoryGainSample));

    public static string CoinHistorySpendSample => GetText(nameof(CoinHistorySpendSample));

    public static string CoinHistoryDateSample => GetText(nameof(CoinHistoryDateSample));

    public static string CoinHistoryGainAmountSample => GetText(nameof(CoinHistoryGainAmountSample));

    public static string CoinHistorySpendAmountSample => GetText(nameof(CoinHistorySpendAmountSample));

    public static string RankingSubtitle => GetText(nameof(RankingSubtitle));

    public static string RankingPlayerSample => GetText(nameof(RankingPlayerSample));

    public static string RankingDivisionSample => GetText(nameof(RankingDivisionSample));

    public static string RankingEloSample => GetText(nameof(RankingEloSample));

    public static string RankingOwnPositionSample => GetText(nameof(RankingOwnPositionSample));

    public static string RankingYouLabel => GetText(nameof(RankingYouLabel));

    public static string ProfileNicknameSample => GetText(nameof(ProfileNicknameSample));

    public static string ProfileTitleSample => GetText(nameof(ProfileTitleSample));

    public static string ProfileLevelSample => GetText(nameof(ProfileLevelSample));

    public static string ProfileMatchesSample => GetText(nameof(ProfileMatchesSample));

    public static string ProfileWinRateSample => GetText(nameof(ProfileWinRateSample));

    public static string ProfileStreakSample => GetText(nameof(ProfileStreakSample));

    public static string ReportSubtitle => GetText(nameof(ReportSubtitle));

    public static string ReportReasonCheating => GetText(nameof(ReportReasonCheating));

    public static string ReportReasonAbuse => GetText(nameof(ReportReasonAbuse));

    public static string ReportReasonName => GetText(nameof(ReportReasonName));

    public static string ReportReasonLeaving => GetText(nameof(ReportReasonLeaving));

    public static string ReportReasonOther => GetText(nameof(ReportReasonOther));

    public static string ReportDescriptionLabel => GetText(nameof(ReportDescriptionLabel));

    public static string ReportDescriptionPlaceholder => GetText(nameof(ReportDescriptionPlaceholder));

    public static string ReportEvidenceNotice => GetText(nameof(ReportEvidenceNotice));

    public static string ReportSendButton => GetText(nameof(ReportSendButton));

    public static string ModerationQueueSubtitle => GetText(nameof(ModerationQueueSubtitle));

    public static string ModerationQueueReasonSample => GetText(nameof(ModerationQueueReasonSample));

    public static string ModerationQueueAgeSample => GetText(nameof(ModerationQueueAgeSample));

    public static string ModerationQueueCountSample => GetText(nameof(ModerationQueueCountSample));

    public static string ModerationQueueReviewButton => GetText(nameof(ModerationQueueReviewButton));

    public static string ReportReviewSubtitle => GetText(nameof(ReportReviewSubtitle));

    public static string ReportReviewReasonLabel => GetText(nameof(ReportReviewReasonLabel));

    public static string ReportReviewDescriptionSample => GetText(nameof(ReportReviewDescriptionSample));

    public static string ReportReviewMessageSample => GetText(nameof(ReportReviewMessageSample));

    public static string ReportReviewAuthorSample => GetText(nameof(ReportReviewAuthorSample));

    public static string ReportReviewSanctionButton => GetText(nameof(ReportReviewSanctionButton));

    public static string ReportReviewDismissButton => GetText(nameof(ReportReviewDismissButton));

    public static string ApplySanctionSubtitle => GetText(nameof(ApplySanctionSubtitle));

    public static string ApplySanctionPlayerLabel => GetText(nameof(ApplySanctionPlayerLabel));

    public static string ApplySanctionScopeLabel => GetText(nameof(ApplySanctionScopeLabel));

    public static string ApplySanctionScopeChat => GetText(nameof(ApplySanctionScopeChat));

    public static string ApplySanctionScopeAccount => GetText(nameof(ApplySanctionScopeAccount));

    public static string ApplySanctionTypeLabel => GetText(nameof(ApplySanctionTypeLabel));

    public static string ApplySanctionTypeTemporary => GetText(nameof(ApplySanctionTypeTemporary));

    public static string ApplySanctionTypePermanent => GetText(nameof(ApplySanctionTypePermanent));

    public static string ApplySanctionDurationLabel => GetText(nameof(ApplySanctionDurationLabel));

    public static string ApplySanctionDurationDay => GetText(nameof(ApplySanctionDurationDay));

    public static string ApplySanctionDurationWeek => GetText(nameof(ApplySanctionDurationWeek));

    public static string ApplySanctionHistorySample => GetText(nameof(ApplySanctionHistorySample));

    public static string ApplySanctionHistoryDateSample => GetText(nameof(ApplySanctionHistoryDateSample));

    public static string ApplySanctionConfirmBody => GetText(nameof(ApplySanctionConfirmBody));

    public static string ApplySanctionApplyButton => GetText(nameof(ApplySanctionApplyButton));

    public static string AppealSubtitle => GetText(nameof(AppealSubtitle));

    public static string AppealDetailSample => GetText(nameof(AppealDetailSample));

    public static string AppealTextLabel => GetText(nameof(AppealTextLabel));

    public static string AppealTextPlaceholder => GetText(nameof(AppealTextPlaceholder));

    public static string AppealStillActiveNotice => GetText(nameof(AppealStillActiveNotice));

    public static string AppealSendButton => GetText(nameof(AppealSendButton));

    public static string AdminPanelSubtitle => GetText(nameof(AdminPanelSubtitle));

    public static string AdminPanelNicknameLabel => GetText(nameof(AdminPanelNicknameLabel));

    public static string AdminPanelRegisteredLabel => GetText(nameof(AdminPanelRegisteredLabel));

    public static string AdminPanelRegisteredSample => GetText(nameof(AdminPanelRegisteredSample));

    public static string AdminPanelStateLabel => GetText(nameof(AdminPanelStateLabel));

    public static string AdminPanelStateSample => GetText(nameof(AdminPanelStateSample));

    public static string AdminPanelTypeLabel => GetText(nameof(AdminPanelTypeLabel));

    public static string AdminPanelTypeSample => GetText(nameof(AdminPanelTypeSample));

    public static string AdminPanelHistorySample => GetText(nameof(AdminPanelHistorySample));

    public static string AdminPanelGrantConfirmBody => GetText(nameof(AdminPanelGrantConfirmBody));

    public static string AdminPanelGrantButton => GetText(nameof(AdminPanelGrantButton));

    public static string LogsSubtitle => GetText(nameof(LogsSubtitle));

    public static string LogsWhichLabel => GetText(nameof(LogsWhichLabel));

    public static string LogsAccessOption => GetText(nameof(LogsAccessOption));

    public static string LogsModerationOption => GetText(nameof(LogsModerationOption));

    public static string LogsFromLabel => GetText(nameof(LogsFromLabel));

    public static string LogsToLabel => GetText(nameof(LogsToLabel));

    public static string LogsDatePlaceholder => GetText(nameof(LogsDatePlaceholder));

    public static string LogsRecordSample => GetText(nameof(LogsRecordSample));

    public static string LogsRecordDetailSample => GetText(nameof(LogsRecordDetailSample));

    public static string LogsResultSample => GetText(nameof(LogsResultSample));

    public static string LogsSearchButton => GetText(nameof(LogsSearchButton));

    public static string ShopSubtitle => GetText(nameof(ShopSubtitle));

    public static string ShopItemSample => GetText(nameof(ShopItemSample));

    public static string ShopBuyButton => GetText(nameof(ShopBuyButton));

    public static string PurchaseConfirmSubtitle => GetText(nameof(PurchaseConfirmSubtitle));

    public static string PurchaseConfirmItemLabel => GetText(nameof(PurchaseConfirmItemLabel));

    public static string PurchaseConfirmPriceLabel => GetText(nameof(PurchaseConfirmPriceLabel));

    public static string PurchaseConfirmPriceSample => GetText(nameof(PurchaseConfirmPriceSample));

    public static string PurchaseConfirmBalanceLabel => GetText(nameof(PurchaseConfirmBalanceLabel));

    public static string PurchaseConfirmRemainingLabel => GetText(nameof(PurchaseConfirmRemainingLabel));

    public static string PurchaseConfirmRemainingSample => GetText(nameof(PurchaseConfirmRemainingSample));

    public static string PurchaseConfirmButton => GetText(nameof(PurchaseConfirmButton));

    public static string BoxPurchaseSubtitle => GetText(nameof(BoxPurchaseSubtitle));

    public static string BoxPurchasePriceSample => GetText(nameof(BoxPurchasePriceSample));

    public static string BoxPurchaseRemainingSample => GetText(nameof(BoxPurchaseRemainingSample));

    public static string BoxPurchaseGuaranteeNotice => GetText(nameof(BoxPurchaseGuaranteeNotice));

    public static string BoxPurchaseConfirmButton => GetText(nameof(BoxPurchaseConfirmButton));

    public static string CustomizeSubtitle => GetText(nameof(CustomizeSubtitle));

    public static string CustomizeSlotPawn => GetText(nameof(CustomizeSlotPawn));

    public static string CustomizeSlotWalls => GetText(nameof(CustomizeSlotWalls));

    public static string CustomizeSlotBoard => GetText(nameof(CustomizeSlotBoard));

    public static string CustomizeSlotFrame => GetText(nameof(CustomizeSlotFrame));

    public static string CustomizeSlotEmotes => GetText(nameof(CustomizeSlotEmotes));

    public static string CustomizeSlotTitle => GetText(nameof(CustomizeSlotTitle));

    public static string CustomizePreviewPlaceholder => GetText(nameof(CustomizePreviewPlaceholder));

    public static string CustomizeEquipButton => GetText(nameof(CustomizeEquipButton));

    public static string ReportReviewOtherAuthorSample => GetText(nameof(ReportReviewOtherAuthorSample));

    // Returning the key when it is missing keeps a typo visible on screen
    // instead of throwing while drawing a frame.
    private static string GetText(string key)
    {
        return _manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
