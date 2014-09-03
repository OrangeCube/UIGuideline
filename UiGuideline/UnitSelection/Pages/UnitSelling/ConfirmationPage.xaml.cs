using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace UiGuidelineUnitSelection.Pages.UnitSelling
{
    public partial class ConfirmationPage
    {
        public ConfirmationPage()
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
