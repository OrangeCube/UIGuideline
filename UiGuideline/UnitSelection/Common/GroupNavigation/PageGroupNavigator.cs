using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UiGuidelineUnitSelection.Common.Navigation;

namespace UiGuidelineUnitSelection.Common.GroupNavigation
{
    /// <summary>
    /// ページ群をまたいだ遷移を管理するクラス。
    /// ページ群単位で ViewModel があって、[戻る]で正しく戻れるように、ページ群履歴と一緒にViewModelもスタックに記録しておく。
    /// 永久にViewModelをとっておくわけにもいかないので、一定個数でpop、かつ、[戻る]がないページに言った時点でClearを呼んでもらう想定。
    /// </summary>
    /// <remarks>
    /// 一定個数で履歴を pop は未実装。
    /// [戻る]がないページで履歴 Clear は実装済み。その場合、<see cref="PageGroupItem.WithoutHistory"/> する。
    /// </remarks>
    public class PageGroupNavigator : IEnumerable
    {
        /// <summary>
        /// 開始ページのキー。
        /// 「戻る」スタックが空の時とかに遷移するページ群のキー。
        ///
        /// 最初に Add したページのキーを set することにした。
        /// </summary>
        public string StartPageKey { get; private set; }

        /// <summary>
        /// 今いるページのキー。
        /// </summary>
        public string CurrentPageKey { get; private set; }

        private readonly Dictionary<string, PageGroupItem> _table = new Dictionary<string, PageGroupItem>();
        private readonly Stack<PageGroupHistory> _history = new Stack<PageGroupHistory>();

        private NavigationState _current;

        /// <summary>
        /// 現在表示されてるページ群のナビゲート状況。
        /// どの Navigator が実行されてるとか、ナビゲート中止用の <see cref="CancellationTokenSource"/> とか。
        /// </summary>
        private class NavigationState
        {
            private bool IsCanceled { get { return _cancel.IsCancellationRequested; } }

            private Task _task;
            private PageNavigatorBase _navigator;
            private object _viewModel;
            private CancellationTokenSource _cancel;

            public NavigationState(PageNavigatorBase navigator, object viewModel, int? stateId, Func<Task> onTaskCompleted)
            {
                _navigator = navigator;
                _viewModel = viewModel;

                _cancel = new CancellationTokenSource();
                _task = navigator.RunAsync(stateId, _cancel.Token);

                RegisterTaskCompleted(onTaskCompleted);
            }

            /// <summary>
            /// ナビゲート処理が、キャンセルではなくて自ら終了した時にコールバックを呼び出す。
            /// <see cref="PageGroupNavigator"/> の仕様的には、<see cref="PageNavigatorBase.RunAsync(int?, CancellationToken)"/> が自ら終了したときに、1つ前のページ群に戻る。
            /// 逆に、<see cref="PageGroupNavigator.NavigateAsync(string)"/> が明示的に呼ばれてページ群遷移するときはナビゲーションがキャンセルされる。
            /// </summary>
            /// <param name="onTaskCompleted"></param>
            private void RegisterTaskCompleted(Func<Task> onTaskCompleted)
            {
                var sc = SynchronizationContext.Current;
                _task.ContinueWith(async t =>
            {
                if (IsCanceled)
                    return;

                if (sc == null)
                    await onTaskCompleted();

                sc.Post(async _ => await onTaskCompleted(), null);
            });
            }

            public PageGroupHistory ToHisotry(string currentPageKey)
            {
                return new PageGroupHistory(currentPageKey, _navigator.FinalStateId, _viewModel);
            }

            public async Task Cancel()
            {
                _cancel.Cancel();
                await _task;
            }
        }

        /// <summary>
        /// ページ群ナビゲーションを開始する。
        /// コンストラクター → <see cref="PageGroupItem"/> の Add → ナビゲーション開始の順序になるので、この処理はコンストラクター内には書けない。
        /// 明示的に呼ぶ必要あり。
        /// </summary>
        public void Start()
        {
            var item = _table[StartPageKey];
            var vm = item.CreateViewModel();
            StartNavigation(item, vm, null);
        }

        /// <summary>
        /// 履歴に残ってる前のページ群に戻る。
        /// </summary>
        public async Task NavigateBackAsync()
        {
            if (_history.Count == 0)
            {
                var item = _table[StartPageKey];
                await NavigateAsync(item, false);
            }
            else
            {
                var h = _history.Pop();
                var item = _table[h.Key];
                await NavigateAsync(item, h.ViewModel, h.FinalStateId, false);
            }
        }

        /// <summary>
        /// キーを指定して明示的にページ群遷移。
        /// </summary>
        public async Task NavigateAsync(string key)
        {
            if (key == CurrentPageKey)
                return;

            var item = _table[key];

            if (item.HasNoHistory)
            {
                // マイページみたいな、「戻る」機能持たないページには HasNoHistory フラグを立てておく。
                // この場合に、履歴をクリア。
                _history.Clear();
            }

            await NavigateAsync(item);
        }

        internal Task NavigateAsync(PageGroupItem item)
        {
            return NavigateAsync(item, true);
        }

        private async Task NavigateAsync(PageGroupItem item, bool pushHistory)
        {
            var vm = item.CreateViewModel();
            await NavigateAsync(item, vm, null, pushHistory);
        }

        private async Task NavigateAsync(PageGroupItem item, object vm, int? stateId, bool pushHistory)
        {
            await CloseAsync(pushHistory);

            StartNavigation(item, vm, stateId);
        }

        private void StartNavigation(PageGroupItem item, object vm, int? stateId)
        {
            var navigator = item.CreateNavigator(vm);

            // ページ群内ナビゲーションがキャンセルじゃなくて、明示的終了した時には NavigateBack を呼ぶ。
            _current = new NavigationState(navigator, vm, stateId, NavigateBackAsync);

            CurrentPageKey = item.Key;
        }

        /// <summary>
        /// 現在のページ群のナビゲーションをキャンセルする。
        /// </summary>
        public Task CloseAsync()
        {
            return CloseAsync(true);
        }

        private async Task CloseAsync(bool pushHistory)
        {
            if (_current != null)
            {
                await _current.Cancel();

                if (pushHistory)
                {
                    // ページ群遷移する前に、現在のページ群情報を履歴に残す。
                    _history.Push(_current.ToHisotry(CurrentPageKey));
                }

                _current = null;
            }
        }

        #region 初期化子用

        public IEnumerator GetEnumerator()
        {
            return _table.Values.GetEnumerator();
        }

        public void Add(PageGroupItem item)
        {
            if (StartPageKey == null)
            {
                StartPageKey = item.Key;
            }

            item.Navigator = this;
            _table.Add(item.Key, item);
        }

        #endregion
    }
}
