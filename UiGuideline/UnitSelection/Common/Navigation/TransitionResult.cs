namespace UiGuidelineUnitSelection.Common.Navigation
{
    /// <summary>
    /// 遷移結果。
    /// この情報に基づいて、<see cref="StateMachine{TState, TOption}"/> が次状態に遷移する。
    /// </summary>
    /// <typeparam name="TState">状態を表す型。enumとかで管理する想定。</typeparam>
    /// <typeparam name="TOption">次状態に対して渡すオプション。</typeparam>
    public class TransitionResult<TState, TOption>
    {
        /// <summary>
        /// 次状態。
        /// </summary>
        public TState NextState { get; private set; }

        /// <summary>
        /// 次状態に対して渡すオプション。
        /// </summary>
        public TOption Option { get; private set; }

        public TransitionResult(TState state, TOption option) { NextState = state; Option = option; }
    }

    public partial class Transition
    {
        /// <summary>
        /// <see cref="TransitionResult{TState, TOption}"/> に対する型推論用ファクトリメソッド。
        /// </summary>
        public static TransitionResult<TState, TOption> Result<TState, TOption>(TState state, TOption option)
        {
            return new TransitionResult<TState, TOption>(state, option);
        }

        /// <summary>
        /// <see cref="TransitionResult{TState, TOption}"/> に対する型推論用ファクトリメソッド。
        /// オプションなし版。
        /// オプションないときはとりあえず、object 型扱いで。
        /// </summary>
        public static TransitionResult<TState, object> Result<TState>(TState state)
        {
            return new TransitionResult<TState, object>(state, null);
        }
    }
}
