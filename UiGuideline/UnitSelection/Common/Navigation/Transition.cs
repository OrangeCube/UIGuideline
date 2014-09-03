using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Common.Navigation
{
    /// <summary>
    /// 遷移設定。
    /// { 遷移のトリガー, 次状態 } × n。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    public class Transition<TState, TOption> : IEnumerable
    {
        private readonly List<ITransitionItem<TState, TOption>> _list = new List<ITransitionItem<TState, TOption>>();

        /// <summary>
        /// 状態繊維を起こす。
        /// </summary>
        /// <returns>次状態。</returns>
        public async Task<TransitionResult<TState, TOption>> Transit()
        {
            var cts = new CancellationTokenSource();
            var t = await Task.WhenAny(_list.Select(x => x.Await(cts.Token)));
            cts.Cancel();

            return t.Result.Result;
        }

        #region 初期化子用

        public void Add(ITransitionItem<TState, TOption> item)
        {
            _list.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
