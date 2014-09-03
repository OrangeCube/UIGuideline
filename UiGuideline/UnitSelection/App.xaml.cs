using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UiGuidelineUnitSelection.Models;

namespace UiGuidelineUnitSelection
{
    public partial class App
    {
        public static new App Current { get { return Application.Current as App; } }

        /// <summary>
        /// 画像ID → 画像のファイル名変換用のコンバーター。
        /// {Binding Converter={x:Static}} で使うために static にコンバーターを持つ。
        /// </summary>
        public static IValueConverter ImageConverter { get { var c = Current; return c != null ? c._imageConverter : null; } }
        private Common.ImageConverter _imageConverter;

        /// <summary>
        /// ゲームのマスターデータ。
        /// </summary>
        public MasterData Masters { get; private set; }

        /// <summary>
        /// アプリ設定。
        /// </summary>
        public AppSettings Settings { get; internal set; }

        /// <summary>
        /// 設定とかマスターデータとかいろいろ読み込み。
        /// </summary>
        public async Task LoadAsync()
        {
            Settings = AppSettings.Load() ?? new AppSettings();

            Settings.PropertyChanged += Settings_PropertyChanged;

            Masters = await Task.Run(() => LoadMaster());

            _imageConverter = new Common.ImageConverter();
            UpdateResourceFolderPath();
        }

        /// <summary>
        /// アプリ終了時の処理。
        /// 設定の保存。
        /// </summary>
        public void Dispose()
        {
            AppSettings.Save(Settings);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResourceFolderPath")
            {
                UpdateResourceFolderPath();
            }
        }

        private void UpdateResourceFolderPath()
        {
            var path = Settings.ResourceFolderPath;
            if (string.IsNullOrWhiteSpace(path) || !System.IO.Directory.Exists(path) || !System.IO.File.Exists(System.IO.Path.Combine(path, "00000.png")))
                _imageConverter.Format = null;
            else
                _imageConverter.Format = Settings.ResourceFolderPath + @"\{0:D5}.png";
        }

        private static MasterData LoadMaster()
        {
            var master = DummyData.DummyGenerator.GetMasters(347);
            return master;
        }
    }
}
