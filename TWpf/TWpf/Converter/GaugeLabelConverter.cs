using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TWpf.Views;

namespace TWpf.Converter
{

    /// <summary>
    /// 标签位置转换器
    /// </summary>
    [ValueConversion(typeof(GaugeTick), typeof(double))]
    public class GaugeLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GaugeTick tick = (GaugeTick)value;
            double angle = tick.Angle * Math.PI / 180; // 转换为弧度
            double radius = 90; // 标签位置的半径

            if (parameter.ToString() == "X")
                return 150 + radius * Math.Sin(angle);
            else
                return 150 - radius * Math.Cos(angle);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
