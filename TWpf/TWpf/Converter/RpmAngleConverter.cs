using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TWpf.Converter
{


    /// <summary>
    /// 角度转换器 - 转速表
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class RpmAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double rpm = (double)value;
            // 将转速值(0-8)映射到角度值(-120-120)
            return rpm * 240 / 8 - 120;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
