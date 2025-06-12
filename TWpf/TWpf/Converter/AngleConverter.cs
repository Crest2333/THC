using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TWpf.Converter
{
    /// <summary>
    /// 角度转换器 - 速度表
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class AngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double speed = (double)value;
            // 将速度值(0-220)映射到角度值(-120-120)
            return speed * 240 / 220 - 120;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
