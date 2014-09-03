using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UiGuidelineUnitSelection.Pages.Gacha
{
    using S = GachaVisualState;
    using StateMachine = Common.Navigation.StateMachine<GachaVisualState, object>;
    using T = Common.Navigation.Transition;
    using TArg = Common.Navigation.TransitionResult<GachaVisualState, object>;
    using Transition = Common.Navigation.Transition<GachaVisualState, object>;

    public enum GachaVisualState
    {
        Top,
        NormalResult,
        PremiumResult,
        Close,
    }

    /// <summary>
    /// ガチャ関連ページ群。
    /// </summary>
    class GachaNavigator : Common.Navigation.PageNavigatorBase
    {
        private ViewModels.GachaPageModel _vm;

        public GachaNavigator(Frame frame, ViewModels.GachaPageModel vm)
            : base(frame)
        {
            _vm = vm;
        }

        public override async Task RunAsync(int? initialStateId, CancellationToken ct)
        {
            var s = new StateMachine()
            {
                { S.Top, RunGachaTop },
                { S.NormalResult, (_, ct1) => RunGachaResult(false, _, ct1) },
                { S.PremiumResult, (_, ct1) => RunGachaResult(true, _, ct1) },
            };

            await s.RunAsync(ct, S.Top, S.Close);
        }

        private async Task<TArg> RunGachaTop(object option, CancellationToken ct)
        {
            var page = await NavigateToGachaTop();

            var tr = new Transition
            {
                T.Item(page.AwaitReturn, () => { return T.Result(S.Close); }),
                T.Item(page.AwaitNormalGacha, () => { return T.Result(S.NormalResult); }),
                T.Item(page.AwaitPremiumGacha, () => { return T.Result(S.PremiumResult); }),
            };

            return await tr.Transit();
        }

        private async Task<TArg> RunGachaResult(bool isPremium, object x, CancellationToken ct)
        {
            _vm.Draw(isPremium);

            var page = await NavigateToGachaResult();

            var tr = new Transition
            {
                T.Item(page.AwaitClose, () => { return T.Result(S.Top); }),
                T.Item(page.AwaitOnceMoreGacha, () => { return T.Result(isPremium ? S.PremiumResult : S.NormalResult); }),
            };

            return await tr.Transit();
        }

        #region ページのロード

        public Task<GachaTopPage> NavigateToGachaTop()
        {
            return NavigateAsync<GachaTopPage>(_vm);
        }

        public Task<GachaResultPage> NavigateToGachaResult()
        {
            return NavigateAsync<GachaResultPage>(_vm);
        }

        #endregion
    }
}
