using Normalizing.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Normalizing.Helpers
{
    // 状态到颜色转换器
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeviceStatus status)
            {
                switch (status)
                {
                    case DeviceStatus.Normal:
                        return HexToBrush("#10B981");
                    case DeviceStatus.Abnormal:
                        return HexToBrush("#ef4444");
                    default:
                        return Brushes.Gray;
                }
            }
            return Brushes.Gray;
        }
        private SolidColorBrush HexToBrush(string hex)
        {
            // 直接转换HEX字符串为Color
            Color color = (Color)ColorConverter.ConvertFromString(hex);
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
