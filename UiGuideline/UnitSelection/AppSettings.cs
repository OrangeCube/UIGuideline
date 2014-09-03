using Newtonsoft.Json;
using System;
using System.IO;
using UiGuidelineUnitSelection.Common;

namespace UiGuidelineUnitSelection
{
    /// <summary>
    /// 永続化しておきたいアプリ設定。
    /// </summary>
    public class AppSettings : BindableBase
    {
        /// <summary>
        /// 画像が入っているフォルダーのフルパス。
        /// </summary>
        public string ResourceFolderPath { get { return _ResourceFolderPath; } set { SetProperty(ref _ResourceFolderPath, value); } }
        private string _ResourceFolderPath;

        /// <summary>
        /// 読み込み。
        /// </summary>
        public static AppSettings Load()
        {
            var settingPath = GetSettingPath();

            if (!File.Exists(settingPath))
                return null;

            try
            {
                var json = File.ReadAllText(settingPath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                return settings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 保存。
        /// </summary>
        public static void Save(AppSettings settings)
        {
            var settingPath = GetSettingPath();
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(settingPath, json);
        }

        private static string GetSettingPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folder = Path.Combine(appData, "UiGuideline", "UnitSelection");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, "settings.txt");
        }
    }
}
