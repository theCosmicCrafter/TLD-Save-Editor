using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace The_Long_Dark_Save_Editor_2.Converters
{
    public class ConditionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Colors.Gray);

            double condition;
            if (value is double d)
                condition = d;
            else if (value is float f)
                condition = f;
            else
                return new SolidColorBrush(Colors.Gray);

            if (condition >= 0.75)
                return new SolidColorBrush(Color.FromRgb(76, 175, 80));   // green
            if (condition >= 0.40)
                return new SolidColorBrush(Color.FromRgb(255, 193, 7));    // amber
            return new SolidColorBrush(Color.FromRgb(244, 67, 54));        // red
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
