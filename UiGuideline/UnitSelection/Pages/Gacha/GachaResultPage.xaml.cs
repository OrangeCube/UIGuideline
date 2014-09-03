using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Pages.Gacha
{
    public partial class GachaResultPage
    {
        public GachaResultPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「戻る」ボタン待ち。
        /// </summary>
        public Task AwaitClose(CancellationToken ct)
        {
            return Observable.FromEventPattern(CloseButton, "Click").FirstAsync().ToTask(ct);
        }

        /// <summary>
        /// 「もう1度引く」ボタン待ち。
        /// </summary>
        public Task AwaitOnceMoreGacha(CancellationToken ct)
        {
            return Observable.FromEventPattern(OnceMoreButton, "Click").FirstAsync().ToTask(ct);
        }
    }
}
