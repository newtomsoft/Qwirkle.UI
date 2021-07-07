using System.Windows.Media;

namespace Qwirkle.UI.Wpf.Design.Converter
{
    public class ApplicationColor
    {
        public static Color Orange { get => Color.FromRgb(0x00, 0x00, 0x00); }
        public static Color StatusRed { get => Color.FromRgb(0xE8, 0x4E, 0x0F); }
        public static Color StatusOrange { get => Color.FromRgb(0xFF, 0xD8, 0x00); }
        public static Color StatusGreen { get => Color.FromRgb(0x64, 0xB3, 0x2E); }
        public static Color ButtonGray { get => Color.FromRgb(0xC8, 0xC8, 0xC8); }
        public static Color ButtonDarkGray { get => Color.FromRgb(0xA4, 0xA4, 0xA4); }
        public static Color ButtonGreen = StatusGreen;
    }
}
