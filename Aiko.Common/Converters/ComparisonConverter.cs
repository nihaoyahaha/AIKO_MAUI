using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Aiko.Common.Converters
{
    public class ComparisonConverter : IValueConverter
    {
        // 将 ViewModel 的值转为 View 的 IsChecked (bool)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        // 当用户点击 RadioButton 时，将 parameter 传回给 ViewModel
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
