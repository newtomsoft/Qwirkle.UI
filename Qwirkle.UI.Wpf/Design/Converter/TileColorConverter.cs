using Qwirkle.Core.Enums;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Qwirkle.UI.Wpf.Design.Converter
{
    public class TileColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TileColor.Green => new SolidColorBrush(ApplicationColor.StatusGreen),
                TileColor.Orange => new SolidColorBrush(ApplicationColor.Orange),
                TileColor.Blue => new SolidColorBrush(ApplicationColor.Orange),
                TileColor.Purple => new SolidColorBrush(ApplicationColor.ButtonDarkGray),
                TileColor.Yellow => new SolidColorBrush(ApplicationColor.ButtonGray),
                TileColor.Red => new SolidColorBrush(ApplicationColor.StatusRed),
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
