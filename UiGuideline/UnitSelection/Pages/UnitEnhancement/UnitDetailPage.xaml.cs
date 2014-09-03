using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Pages.UnitEnhancement
{
    /// <summary>
    /// ユニット詳細画面。
    /// </summary>
    public partial class UnitDetailPage
    {
        public UnitDetailPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 詳細を「閉じる」ボタン待ち。
        /// </summary>
        public Task AwaitClose(CancellationToken ct)
        {
            return Observable.FromEventPattern(CloseButton, "Click").FirstAsync().ToTask(ct);
        }
    }
}
