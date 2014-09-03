using System.Threading.Tasks;
using System.Windows;
using UiGuidelineUnitSelection.GameModels;

namespace UiGuidelineUnitSelection
{
    using Common.GroupNavigation;
    using Navigator = Common.GroupNavigation.PageGroupNavigator;
    using PG = Common.GroupNavigation.PageGroupItem;

    /// <summary>
    /// とりあえずメインウィンドウは、
    /// マスターデータとかを読み込んで、
    /// 作ったフレームを <see cref="PageGroupNavigator"/> に渡して、ページ群間遷移設定をするだけ。
    /// </summary>
    public partial class MainWindow
    {
        public AppSettings Settings
        {
            get { return (AppSettings)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof(AppSettings), typeof(MainWindow), new PropertyMetadata(null));

        public PageGroupNavigator Navigator
        {
            get { return (PageGroupNavigator)GetValue(NavigatorProperty); }
            set { SetValue(NavigatorProperty, value); }
        }
        public static readonly DependencyProperty NavigatorProperty =
            DependencyProperty.Register("Navigator", typeof(PageGroupNavigator), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            Loaded += FrameworkElement_Loaded;
        }

        private async void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var app = await LoadAsync();

            Settings = app.Settings;

            Navigator = CreateNavigator(app.Masters);

            Closed += async (_, __) =>
            {
                app.Dispose();
                await Navigator.CloseAsync();
            };

            Navigator.Start();
        }

        private Navigator CreateNavigator(Models.MasterData masters)
        {
            var model = new UnitModel(masters);
            var frame = MainFrame;

            return new Navigator()
            {
                PG.Item("マイページ", () => default(object), vm => new Pages.MyPage.MyPageNavigator(frame)).WithoutHistory(),
                PG.Item("強化合成", () => new ViewModels.UnionPageModel(masters, model), vm => new Pages.UnitEnhancement.UnitEnhancementNavigator(frame, vm)),
                PG.Item("ガチャ", () => new ViewModels.GachaPageModel(masters, model), vm => new Pages.Gacha.GachaNavigator(frame, vm)),
                PG.Item("売却", () => new ViewModels.UnitSellingPageModel(masters, model), vm => new Pages.UnitSelling.UnitSellingNavigator(frame, vm)),
            };
        }

        private async Task<App> LoadAsync()
        {
            IsEnabled = false;
            Status.Text = "loading...";

            var app = App.Current;
            await app.LoadAsync();

            Status.Text = "";
            IsEnabled = true;

            return app;
        }
    }
}
