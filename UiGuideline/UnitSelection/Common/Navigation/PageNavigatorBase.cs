using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UiGuidelineUnitSelection.Common.Navigation
{
    /// <summary>
    /// フレーム中のページ ナビゲーション用の基底クラス。
    /// ページ群中での遷移用。
    /// </summary>
    /// <remarks>
    /// ページ … WPF の場合、Page を継承した、ほんと1つのページ。
    /// ページ群 … 1つのViewModel共有で作れる一連のページ。[戻る]時の挙動とかも、ViewModelかPageNavigatorで責任を持つ。
    /// ページ群をまたいだ遷移は <see cref="PageGroupNavigator"/> がやる。
    /// </remarks>
    public abstract class PageNavigatorBase
    {
        // このフレーム中でページ遷移する。
        private Frame _frame;

        // /Pages/{$pagePrefix}/{$typeName}.xaml でページ遷移する。
        string _pagePrefix;

        public PageNavigatorBase(Frame frame)
        {
            _pagePrefix = GetPrefix();

            _frame = frame;
            _frame.NavigationService.LoadCompleted += SetDataContextOnLoadCompleted;
        }

        /// <summary>
        /// ページ遷移の管理処理を開始。
        /// </summary>
        /// <param name="initialStateId">初期状態。null ならRunAsync内で規定ページから開始させる。</param>
        public abstract Task RunAsync(int? initialStateId, CancellationToken ct);

        /// <summary>
        /// 最終状態。
        /// 子クラスでは enum を使う想定だけど、汎用化のために int 化したものを返すようにする。
        /// </summary>
        public int FinalStateId { get; protected set; }

        /// <summary>
        /// 規約ベースで、ページ遷移 URI /Pages/{$pagePrefix}/{$typeName}.xaml の pagePrefix の部分を取得する。
        /// 名前空間の、クラス名直前の部分、例えば、 A.B.C.ClassName だったらCのところが page Prefix になるという規約。
        /// 実際、プロジェクト的に、/Pages/{$pagePrefix}/{$typeName}.xaml という XAML を作ると、
        /// {$solusionName}.Pages.{$pagePrefix}.{$typeName} という名前のクラスができるので、この前提。
        /// </summary>
        /// <returns></returns>
        private string GetPrefix()
        {
            var nameHierarchy = GetType().FullName.Split('.');
            var pagePrefix = nameHierarchy[nameHierarchy.Length - 2];
            return pagePrefix;
        }


        /// <summary>
        /// <see cref="Frame.Navigate(object, object)"/> の extraData にデータコンテキストが入って送られてくる想定で、
        /// <see cref="FrameworkElement.DataContext"/> にデータをセットする。
        /// </summary>
        private void SetDataContextOnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var x = e.Content as FrameworkElement;
            x.DataContext = e.ExtraData;
        }

        /// <summary>
        /// ページ遷移させる。
        /// 以下の想定で、規約ベース実装:
        /// ・関連するページは全部 Pages フォルダーの直下に入っている
        /// ・ページの型名 ＝ XAML のファイル名
        /// </summary>
        /// <typeparam name="TView">ページの型。</typeparam>
        /// <param name="dataContext">ページに対して渡したいデータコンテキスト。</param>
        /// <returns>読みこんだページのインスタンス。</returns>
        protected async Task<TView> NavigateAsync<TView>(object dataContext)
            where TView : class
        {
            _frame.IsEnabled = false;
            var t = AwaitLoadCompleted().ContinueWith(x => x.Result as TView);
            _frame.Navigate(new Uri("/Pages/" + _pagePrefix + "/" + typeof(TView).Name + ".xaml", UriKind.Relative), dataContext);
            var v = await t;
            _frame.IsEnabled = true;
            return v;
        }

        private Task<object> AwaitLoadCompleted()
        {
            return Observable.FromEventPattern<NavigationEventArgs>(_frame.NavigationService, "LoadCompleted")
                .Select(x => x.EventArgs.Content)
                .FirstAsync().ToTask();
        }
    }
}
