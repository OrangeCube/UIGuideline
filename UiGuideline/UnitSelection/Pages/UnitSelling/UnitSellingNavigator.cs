using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.Pages.UnitSelling
{
    using S = UnitSellingVisualState;
    using StateMachine = Common.Navigation.StateMachine<UnitSellingVisualState, object>;
    using T = Common.Navigation.Transition;
    using TArg = Common.Navigation.TransitionResult<UnitSellingVisualState, object>;
    using Transition = Common.Navigation.Transition<UnitSellingVisualState, object>;

    public enum UnitSellingVisualState
    {
        Selection,
        Confirmation,
        Close,
    }

    /// <summary>
    /// ユニット売却関連ページ群。
    /// </summary>
    public class UnitSellingNavigator : Common.Navigation.PageNavigatorBase
    {
        private ViewModels.UnitSellingPageModel _vm;

        public UnitSellingNavigator(Frame frame, ViewModels.UnitSellingPageModel vm)
            : base(frame)
        {
            _vm = vm;
        }

        public override async Task RunAsync(int? initialStateId, CancellationToken ct)
        {
            var s = new StateMachine()
            {
                { S.Selection, RunSelection },
                { S.Confirmation, RunConfirmation },
            };

            await s.RunAsync(ct, S.Selection, S.Close);
        }

        private async Task<TArg> RunSelection(object _, CancellationToken ct)
        {
            var page = await NavigateToSelection();

            var tr = new Transition
            {
                T.Item(page.AwaitSell, x => { ResetUnits(x); return T.Result(S.Confirmation); }),
                T.Item(page.AwaitReturn, () => T.Result(S.Close)),
            };

            return await tr.Transit();
        }

        private async Task<TArg> RunConfirmation(object _, CancellationToken ct)
        {
            var page = await NavigateToConfirmation();

            var tr = new Transition
            {
                T.Item(page.AwaitOk, () => { _vm.Sell(); return T.Result(S.Selection); }),
                T.Item(page.AwaitCancel, () => T.Result(S.Selection)),
            };

            return await tr.Transit();
        }

        private void ResetUnits(IEnumerable<UnitWithMaster> units)
        {
            _vm.Units.Clear();
            foreach (var x in units)
            {
                _vm.Units.Add(x);
            }
        }

        #region ページのロード

        public Task<SelectionPage> NavigateToSelection()
        {
            return NavigateAsync<SelectionPage>(_vm);
        }

        public Task<ConfirmationPage> NavigateToConfirmation()
        {
            return NavigateAsync<ConfirmationPage>(_vm);
        }

        #endregion
    }
}
