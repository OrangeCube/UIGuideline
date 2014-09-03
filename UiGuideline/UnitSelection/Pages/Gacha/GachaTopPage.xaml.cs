using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Pages.Gacha
{
    public partial class GachaTopPage
    {
        public GachaTopPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「戻る」ボタン待ち。
        /// </summary>
        public Task AwaitReturn(CancellationToken ct)
        {
            return Observable.FromEventPattern(ReturnButton, "Click").FirstAsync().ToTask(ct);
        }

        /// <summary>
        /// 「無料ガチャ」ボタン待ち。
        /// </summary>
        public Task AwaitNormalGacha(CancellationToken ct)
        {
            return Observable.FromEventPattern(NormalGacha, "Click").FirstAsync().ToTask(ct);
        }

        /// <summary>
        /// 「課金ガチャ」ボタン待ち。
        /// </summary>
        public Task AwaitPremiumGacha(CancellationToken ct)
        {
            return Observable.FromEventPattern(PremiumGacha, "Click").FirstAsync().ToTask(ct);
        }
    }
}
