using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TWpf.Converter
{
    public class ParityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "无":
                    return Parity.None;
                case "奇校验":
                    return Parity.Odd;
                case "偶校验":
                    return Parity.Even;
                default:
                    return Parity.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Parity)value)
            {
                case Parity.None:
                    return "无";
                case Parity.Odd:
                    return "奇校验";
                case Parity.Even:
                    return "偶校验";
                default:
                    return "无";
            }
        }
    }
}
