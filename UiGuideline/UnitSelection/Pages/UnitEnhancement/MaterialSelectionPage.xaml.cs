using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.Pages.UnitEnhancement
{
    /// <summary>
    /// 強化合成での素材ユニットの選択画面。
    /// </summary>
    public partial class MaterialSelectionPage
    {
        public MaterialSelectionPage()
        {
            InitializeComponent();

            DataContextChanged += FrameworkElement_DataContextChanged;
        }

        private void FrameworkElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as ViewModels.UnionPageModel;

            var unitList = List.UnitList;

            unitList.SelectionMode = SelectionMode.Extended;
        }

        /// <summary>
        /// 強化合成「実行」ボタン待ち。
        /// </summary>
        public Task<IEnumerable<UnitWithMaster>> AwaitEnhance(CancellationToken ct)
        {
            return Observable.FromEventPattern(EnhanceButton, "Click")
                .Select(x => List.UnitList.SelectedItems.OfType<UnitWithMaster>().ToArray().AsEnumerable())
                .FirstAsync()
                .ToTask(ct);
        }

        /// <summary>
        /// 「戻る」ボタン待ち。
        /// </summary>
        public Task AwaitReturn(CancellationToken ct)
        {
            return Observable.FromEventPattern(ReturnButton, "Click")
                .FirstAsync()
                .ToTask(ct);
        }

        /// <summary>
        /// ユニット詳細を開く操作待ち。
        /// <see cref="BaseSelectionPage.AwaitUnitDetail(CancellationToken)"/> と同じ。
        /// </summary>
        public Task<UnitWithMaster> AwaitUnitDetail(CancellationToken ct)
        {
            return List.UnitList.AwaitMouse("MouseRightButtonUp", ct);
        }
    }
}
