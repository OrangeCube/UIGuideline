using System;
using System.Threading.Tasks;
using System.Windows.Input;
using UiGuidelineUnitSelection.Common.Navigation;

namespace UiGuidelineUnitSelection.Common.GroupNavigation
{
    /// <summary>
    /// ページ群間遷移用の設定。
    /// 実体としてはジェネリック版の方を使う。<see cref="PageGroupNavigator"/> の方で共通処理するためにあえて object 版を用意。
    /// </summary>
    public abstract class PageGroupItem
    {
        /// <summary>
        /// ページ遷移用のキー。
        /// <see cref="PageGroupNavigator.NavigateAsync(string)"/> に渡すと、このページ群に遷移できる。
        /// </summary>
        public string Key { get; private set; }

        protected PageGroupItem(string key) { Key = key; }

        /// <summary>
        /// 「戻る」ボタンがなくて、これ以上履歴をたどる必要がないページに対して立てておくフラグ。
        /// このフラグが true の時、<see cref="PageGroupNavigator"/> の履歴スタックを空にする。
        /// </summary>
        internal bool HasNoHistory { get; private set; }

        /// <summary>
        /// fluent に <see cref="HasNoHistory"/> フラグを立てるメソッド。
        /// </summary>
        /// <returns>自分自身。</returns>
        public PageGroupItem WithoutHistory()
        {
            HasNoHistory = true;
            return this;
        }

        /// <summary>
        /// このページ群用の ViewModel を新規作成。
        /// 新規遷移の時だけ呼ばれる。
        /// </summary>
        /// <returns>新しいインスタンス。</returns>
        public abstract object CreateViewModel();

        /// <summary>
        /// ページ群に遷移してきたときに呼ぶ、ページ群内Navigator作成用関数。
        /// <see cref="CreateViewModel"/>とわかれてるのは、履歴経由(「戻る」ボタン経由)で来た時には新規作成じゃなくて、履歴に残してるViewModelを渡すから。
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public abstract PageNavigatorBase CreateNavigator(object viewModel);

        internal PageGroupNavigator Navigator { get; set; }

        /// <summary>
        /// このページ群に遷移させる。
        /// UI上にデータバインドして、<see cref="NavigateCommand"/> コマンド経由でページ群遷移したい時用に、ここにもメソッド置いとく。
        /// このためだけに<see cref="Navigator"/>をinternal公開してsetしてもらってる。
        /// </summary>
        public Task NavigateAsync() { return Navigator.NavigateAsync(this); }

        public ICommand NavigateCommand { get { return _nav ?? (_nav = new DelegateCommand(async () => await NavigateAsync())); } }
        private ICommand _nav;

        /// <summary>
        /// 型推論用。
        /// </summary>
        public static PageGroupItem<TViewModel, TNavigator> Item<TViewModel, TNavigator>(Func<TViewModel> createViewModel, Func<TViewModel, TNavigator> createNavigator)
            where TNavigator : PageNavigatorBase
        {
            return new PageGroupItem<TViewModel, TNavigator>(createViewModel, createNavigator);
        }

        /// <summary>
        /// 型推論用。
        /// </summary>
        public static PageGroupItem<TViewModel, TNavigator> Item<TViewModel, TNavigator>(string key, Func<TViewModel> createViewModel, Func<TViewModel, TNavigator> createNavigator)
            where TNavigator : PageNavigatorBase
        {
            return new PageGroupItem<TViewModel, TNavigator>(key, createViewModel, createNavigator);
        }
    }

    /// <summary>
    /// ページ群間遷移用の設定。
    /// こっちが本体。
    /// </summary>
    public class PageGroupItem<TViewModel, TNavigator> : PageGroupItem
        where TNavigator : PageNavigatorBase
    {
        private readonly Func<TViewModel> _createViewModel;
        private readonly Func<TViewModel, TNavigator> _createNavigator;

        public PageGroupItem(string key, Func<TViewModel> createViewModel, Func<TViewModel, TNavigator> createNavigator)
            : base(key)
        {
            _createViewModel = createViewModel;
            _createNavigator = createNavigator;
        }

        public PageGroupItem(Func<TViewModel> createViewModel, Func<TViewModel, TNavigator> createNavigator)
            : this(typeof(TNavigator).Name, createViewModel, createNavigator)
        { }

        public override object CreateViewModel()
        {
            return _createViewModel();
        }

        public override PageNavigatorBase CreateNavigator(object viewModel)
        {
            var vm = (TViewModel)viewModel;
            return _createNavigator(vm);
        }
    }
}
