using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Common
{
    // CancellationToken 受け取る非同期メソッド。
    // 割と頻出なので標準であってくれてもいい気がする。

    public delegate Task AsyncAction(CancellationToken ct);
    public delegate Task AsyncAction<T1>(T1 x1, CancellationToken ct);
    public delegate Task AsyncAction<T1, T2>(T1 x1, T2 x2, CancellationToken ct);
    public delegate Task AsyncAction<T1, T2, T3>(T1 x1, T2 x2, T3 x3, CancellationToken ct);

    public delegate Task<TResult> AsyncFunc<TResult>(CancellationToken ct);
    public delegate Task<TResult> AsyncFunc<T1, TResult>(T1 x1, CancellationToken ct);
    public delegate Task<TResult> AsyncFunc<T1, T2, TResult>(T1 x1, T2 x2, CancellationToken ct);
}
