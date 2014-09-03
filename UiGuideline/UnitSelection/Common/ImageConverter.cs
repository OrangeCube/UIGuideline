using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UiGuidelineUnitSelection.Common
{
    /// <summary>
    /// 画像ID → 画像のパス 変換。
    /// </summary>
    class ImageConverter : DependencyObject, IValueConverter
    {
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(ImageConverter), new PropertyMetadata(""));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Format == null)
                return null;

            if (!(value is int)) return null;
            var i = (int)value;

            return string.Format(Format, i);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
