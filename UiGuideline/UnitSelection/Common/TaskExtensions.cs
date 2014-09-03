using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection
{
    public static class TaskExtensions
    {
        public static Task AwaitCancel(this CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<bool>();
            ct.Register(() => tcs.TrySetResult(false));
            return tcs.Task;
        }
    }
}
