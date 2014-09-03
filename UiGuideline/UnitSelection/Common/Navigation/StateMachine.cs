using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Common.Navigation
{
    /// <summary>
    /// 状態遷移マシン。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    public class StateMachine<TState, TOption> : IEnumerable
    {
        public delegate Task<TransitionResult<TState, TOption>> TransitionFunc(TOption option, CancellationToken ct);

        private readonly IDictionary<TState, TransitionFunc> _table = new Dictionary<TState, TransitionFunc>();

        /// <summary>
        /// 実行。
        /// </summary>
        /// <param name="initialState">初期状態。</param>
        /// <param name="terminalState">終了状態。</param>
        /// <param name="initialOption">初期状態に遷移する際に渡すオプション。</param>
        /// <returns>実行完了待ちTask。</returns>
        public async Task RunAsync(CancellationToken ct, TState initialState, TState terminalState, TOption initialOption = default(TOption))
        {
            var arg = Transition.Result(initialState, initialOption);

            while (!arg.NextState.Equals(terminalState))
            {
                TransitionFunc f;
                if (!_table.TryGetValue(arg.NextState, out f)) break;

                CurrentState = arg.NextState;

                var t = f(arg.Option, ct);
                await Task.WhenAny(t, ct.AwaitCancel());

                if (ct.IsCancellationRequested)
                    break;

                arg = await t;
            }
        }

        public TState CurrentState { get; private set; }

        #region 初期化子用

        public IEnumerator GetEnumerator()
        {
            return _table.GetEnumerator();
        }

        public void Add(TState state, TransitionFunc transition)
        {
            _table.Add(state, transition);
        }

        #endregion
    }
}
