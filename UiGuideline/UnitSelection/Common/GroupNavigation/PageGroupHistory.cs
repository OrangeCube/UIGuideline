namespace UiGuidelineUnitSelection.Common.GroupNavigation
{
    /// <summary>
    /// ページ群間の遷移履歴。
    /// </summary>
    class PageGroupHistory
    {
        public string Key { get; private set; }
        public int FinalStateId { get; private set; }
        public object ViewModel { get; private set; }

        public PageGroupHistory(string key, int finalStateId, object viewModel)
        {
            Key = key;
            FinalStateId = finalStateId;
            ViewModel = viewModel;
        }
    }
}
