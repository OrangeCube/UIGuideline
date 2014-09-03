using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Common.Navigation
{
    /// <summary>
    /// 遷移に関する詳細。
    /// どういうトリガーが来た時に、次状態をどうするかとかを決める主体。
    /// 複数の TransitionItem.Await を WhenAny して、最初に完了した1個だけを採用して次状態に遷移。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    public interface ITransitionItem<TState, TOption>
    {
        /// <summary>
        /// トリガー待ち。
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<ITransitionItem<TState, TOption>> Await(CancellationToken ct);

        /// <summary>
        /// 次状態への遷移情報。
        /// </summary>
        /// <returns></returns>
        TransitionResult<TState, TOption> Result { get; }
    }

    /// <summary>
    /// <see cref="ITransitionItem{TState, TOption}"/> の実装。
    /// トリガーと結果取得の間に中間状態 <see cref="TTemporary"/> が挟まるタイプ。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    /// <typeparam name="TTemporary">中間状態の型。</typeparam>
    public class TransitionItem<TState, TOption, TTemporary> : ITransitionItem<TState, TOption>
    {
        public delegate Task<TTemporary> TransitionTrigger(CancellationToken ct);
        public delegate TransitionResult<TState, TOption> TransitionResult(TTemporary x);

        private readonly TransitionTrigger _awaitTrigger;
        private readonly TransitionResult _getResult;
        private TTemporary _tamporaryState;

        public TransitionItem(TransitionTrigger awaitTrigger, TransitionResult getResult) { _awaitTrigger = awaitTrigger; _getResult = getResult; }

        public async Task<ITransitionItem<TState, TOption>> Await(CancellationToken ct)
        {
            _tamporaryState = await _awaitTrigger(ct);
            return this;
        }

        public TransitionResult<TState, TOption> Result { get { return _getResult(_tamporaryState); } }
    }

    /// <summary>
    /// <see cref="ITransitionItem{TState, TOption}"/> の実装。
    /// 中間状態がないタイプ。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    public class TransitionItem<TState, TOption> : ITransitionItem<TState, TOption>
    {
        public delegate Task TransitionTrigger(CancellationToken ct);
        public delegate TransitionResult<TState, TOption> TransitionResult();

        private readonly TransitionTrigger _awaitTrigger;
        private readonly TransitionResult _getResult;

        public TransitionItem(TransitionTrigger awaitTrigger, TransitionResult getResult) { _awaitTrigger = awaitTrigger; _getResult = getResult; }

        public async Task<ITransitionItem<TState, TOption>> Await(CancellationToken ct)
        {
            await _awaitTrigger(ct);
            return this;
        }

        public TransitionResult<TState, TOption> Result { get { return _getResult(); } }
    }

    public partial class Transition
    {
        /// <summary>
        /// <see cref="TransitionItem{TState, TOption, TTemporary}"/> に対する型推論用ファクトリメソッド。
        /// </summary>
        public static TransitionItem<TState, TOption, TTemporary> Item<TState, TOption, TTemporary>(
            TransitionItem<TState, TOption, TTemporary>.TransitionTrigger awaitTrigger,
            TransitionItem<TState, TOption, TTemporary>.TransitionResult getResult)
        {
            return new TransitionItem<TState, TOption, TTemporary>(awaitTrigger, getResult);
        }

        /// <summary>
        /// <see cref="TransitionItem{TState, TOption}"/> に対する型推論用ファクトリメソッド。
        /// </summary>
        public static TransitionItem<TState, TOption> Item<TState, TOption>(
            TransitionItem<TState, TOption>.TransitionTrigger awaitTrigger,
            TransitionItem<TState, TOption>.TransitionResult getResult)
        {
            return new TransitionItem<TState, TOption>(awaitTrigger, getResult);
        }
    }
}
