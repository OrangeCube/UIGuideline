using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection.Pages.UnitEnhancement
{
    /// <summary>
    /// 強化合成でのベースユニットの選択画面。
    /// </summary>
    public partial class BaseSelectionPage
    {
        public BaseSelectionPage()
        {
            InitializeComponent();

            DataContextChanged += FrameworkElement_DataContextChanged;
        }

        private void FrameworkElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as ViewModels.UnionPageModel;

            var unitList = List.UnitList;

            unitList.SelectionMode = SelectionMode.Single;
            unitList.SelectedItem = vm.BaseUnit;
            unitList.ScrollIntoView(vm.BaseUnit);
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
        /// ベースユニット選択待ち。
        /// ユニット一覧画面のアイコンを左クリックでユニット選択。
        /// タッチデバイスだとシングルタップを想定。
        /// </summary>
        public Task<UnitWithMaster> AwaitUnitSelect(CancellationToken ct)
        {
            return List.UnitList.AwaitMouse("MouseUp", ct);
        }

        /// <summary>
        /// ユニット詳細を開く操作待ち。
        /// ユニット一覧画面のアイコンを右クリックでユニット詳細を開く。
        /// タッチデバイスだとホールドを想定。
        /// </summary>
        public Task<UnitWithMaster> AwaitUnitDetail(CancellationToken ct)
        {
            return List.UnitList.AwaitMouse("MouseRightButtonUp", ct);
        }
    }

    static class EventExtensions
    {
        /// <summary>
        /// ListBox に <see cref="UnitWithMaster"/> のコレクションがバインドされてる想定の元、
        /// その項目のどれかがタップされるのを待つ。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="eventName">左クリック/右クリック(タップ/ホールド)を同一ロジックで待つために、イベント名を外からもらう。</param>
        /// <param name="ct"></param>
        /// <returns>タップされた項目にバインドされているユニット。</returns>
        public static Task<UnitWithMaster> AwaitMouse(this ListBox list, string eventName, CancellationToken ct)
        {
            return Observable.FromEventPattern<MouseButtonEventArgs>(list, eventName)
            .Select(x => GetUnitFromDataContext(x.EventArgs))
                .Where(x => x != null)
                .FirstAsync()
                .ToTask(ct);
        }

        /// <summary>
        /// <see cref="FrameworkElement.DataContext"/> の中身を取得。
        /// <see cref="UnitWithMaster"/> でないものが入ってたら null を返す。
        /// </summary>
        /// <param name="e"></param>
        /// <returns>バインドされているユニット。</returns>
        private static UnitWithMaster GetUnitFromDataContext(RoutedEventArgs e)
        {
            var elem = e.OriginalSource as FrameworkElement;
            var dc = elem.DataContext;
            var u = dc as UnitWithMaster;
            return u;
        }
    }
}
