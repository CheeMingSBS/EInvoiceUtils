using System.Globalization;
using System.Windows.Data;

namespace SBS.Core.EInvoiceUtilsDemo.Converters
{
    public class MainUIConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}