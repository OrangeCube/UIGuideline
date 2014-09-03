using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.Pages.UnitSelling
{
    /// <summary>
    /// まとめて売却したいユニットを一斉選択。
    /// </summary>
    public partial class SelectionPage
    {
        public SelectionPage()
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
        /// 「売却」ボタン待ち。
        /// </summary>
        public Task<IEnumerable<UnitWithMaster>> AwaitSell(CancellationToken ct)
        {
            return Observable.FromEventPattern(SellButton, "Click")
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
    }
}
