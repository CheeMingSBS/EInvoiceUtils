using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SBS.Core.EInvoiceUtilsDemo.Converters
{
    public class LoginUIConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return values.Clone();

            if (parameter as string == "LoginPage" && string.IsNullOrWhiteSpace(values[0] as string))
                return Visibility.Visible;
            else if (parameter as string == "MainPage" && !string.IsNullOrWhiteSpace(values[0] as string))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}