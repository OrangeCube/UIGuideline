using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UiGuidelineUnitSelection.Common.Navigation;

namespace UiGuidelineUnitSelection.Pages.MyPage
{
    /// <summary>
    /// ダミーのマイページが1枚あるだけなので、そのページを表示して終わり。
    /// </summary>
    public class MyPageNavigator : PageNavigatorBase
    {
        public MyPageNavigator(Frame frame) : base(frame) { }

        public override async Task RunAsync(int? initialStateId, CancellationToken ct)
        {
            await NavigateAsync<MyPage>(null);
            await ct.AwaitCancel();
        }
    }
}
