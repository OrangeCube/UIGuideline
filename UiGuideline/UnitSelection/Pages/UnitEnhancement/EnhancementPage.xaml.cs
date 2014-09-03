using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Pages.UnitEnhancement
{
    /// <summary>
    /// 強化合成の確認・実行画面。
    /// 実際は「実行」を押した後にもう数画面、合成演出 → 合成結果画面と遷移するけども、サンプルだし1画面にまとめた。
    /// </summary>
    public partial class EnhancementPage
    {
        public EnhancementPage()
        {
            InitializeComponent();
        }

        public Task AwaitOk(CancellationToken ct)
        {
            return Observable.FromEventPattern(OkButton, "Click")
                .FirstAsync()
                .ToTask(ct);
        }

        public Task AwaitCancel(CancellationToken ct)
        {
            return Observable.FromEventPattern(CancelButton, "Click")
                .FirstAsync()
                .ToTask(ct);
        }
    }
}
