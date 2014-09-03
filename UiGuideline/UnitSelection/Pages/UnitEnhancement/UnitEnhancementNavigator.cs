using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.Pages.UnitEnhancement
{
    using S = UnitEnhancementVisualState;
    using StateMachine = Common.Navigation.StateMachine<UnitEnhancementVisualState, UnitWithMaster>;
    using T = Common.Navigation.Transition;
    using TArg = Common.Navigation.TransitionResult<UnitEnhancementVisualState, UnitWithMaster>;
    using Transition = Common.Navigation.Transition<UnitEnhancementVisualState, UnitWithMaster>;

    public enum UnitEnhancementVisualState
    {
        BaseSelection,
        MaterialSelection,
        DetailFromBase,
        DetailFromMaterial,
        Enhancement,
        Close,
    }

    /// <summary>
    /// ユニット強化合成ページ群。
    /// </summary>
    class UnitEnhancementNavigator : Common.Navigation.PageNavigatorBase
    {
        private ViewModels.UnionPageModel _vm;

        public UnitEnhancementNavigator(Frame frame, ViewModels.UnionPageModel vm)
            : base(frame)
        {
            _vm = vm;
        }

        public override async Task RunAsync(int? initialStateId, CancellationToken ct)
        {
            var s = new StateMachine()
            {
                { S.BaseSelection, RunBaseSelection },
                { S.MaterialSelection, RunMaterialSelection },
                { S.DetailFromBase, (x, ct1) => RunDetail(x, S.BaseSelection, ct1) },
                { S.DetailFromMaterial, (x, ct1) => RunDetail(x, S.MaterialSelection, ct1) },
                { S.Enhancement, RunEnhancement },
            };

            S initialState = GetInitialState(initialStateId);

            await s.RunAsync(ct, initialState, S.Close);

            FinalStateId = (int)s.CurrentState;
        }

        private UnitEnhancementVisualState GetInitialState(int? initialStateId)
        {
            // 指定なし or 前回は終了状態で終わってる → 規定のベース選択ページに
            if (initialStateId == null
                || initialStateId == (int)S.Close)
                return S.BaseSelection;

            // ベースユニットが消えてたりする → ベース選択からやりなおし
            if (!_vm.Model.Units.Any(u => u == _vm.BaseUnit))
                return S.BaseSelection;

            return (S)initialStateId.Value;
        }

        private async Task<TArg> RunEnhancement(UnitWithMaster option, CancellationToken ct)
        {
            var page = await NavigateToEnhancement();

            var tr = new Transition
            {
                T.Item(page.AwaitOk, () => { _vm.Enhance(); return T.Result(S.MaterialSelection, _vm.BaseUnit); }),
                T.Item(page.AwaitCancel, () => { return T.Result(S.MaterialSelection, _vm.BaseUnit); }),
            };

            return await tr.Transit();
        }

        private async Task<TArg> RunBaseSelection(UnitWithMaster u, CancellationToken ct)
        {
            var page = await NavigateToBaseSelection();

            var tr = new Transition
            {
                T.Item(page.AwaitUnitSelect, x => { _vm.BaseUnit = x; return T.Result(S.MaterialSelection, x); }),
                T.Item(page.AwaitUnitDetail, x => { _vm.BaseUnit = x; return T.Result(S.DetailFromBase, x); }),
                T.Item(page.AwaitReturn, () => T.Result(S.Close, default(UnitWithMaster))),
            };

            return await tr.Transit();
        }

        private async Task<TArg> RunMaterialSelection(UnitWithMaster u, CancellationToken ct)
        {
            var page = await NavigateToMaterialSelection();

            var tr = new Transition
            {
                T.Item(page.AwaitEnhance, x => { ResetMaterials(x); return T.Result(S.Enhancement, _vm.BaseUnit); }),
                T.Item(page.AwaitUnitDetail, x => { _vm.BaseUnit = x; return T.Result(S.DetailFromMaterial, x); }),
                T.Item(page.AwaitReturn, () => T.Result(S.BaseSelection, default(UnitWithMaster))),
            };

            return await tr.Transit();
        }

        private void ResetMaterials(IEnumerable<UnitWithMaster> enhance)
        {
            var materials = enhance;

            _vm.MaterialUnits.Clear();
            foreach (var x in materials)
            {
                _vm.MaterialUnits.Add(x);
            }
        }

        private async Task<TArg> RunDetail(UnitWithMaster u, S backTo, CancellationToken ct)
        {
            var page = await NavigateToDetailView(u);
            await page.AwaitClose(CancellationToken.None);
            return T.Result(backTo, u);
        }

        #region ページのロード

        public Task<BaseSelectionPage> NavigateToBaseSelection()
        {
            return NavigateAsync<BaseSelectionPage>(_vm);
        }

        public Task<MaterialSelectionPage> NavigateToMaterialSelection()
        {
            return NavigateAsync<MaterialSelectionPage>(_vm);
        }

        public Task<EnhancementPage> NavigateToEnhancement()
        {
            return NavigateAsync<EnhancementPage>(_vm);
        }

        public Task<UnitDetailPage> NavigateToDetailView(UnitWithMaster u)
        {
            return NavigateAsync<UnitDetailPage>(u);
        }

        #endregion
    }
}
