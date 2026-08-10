using System;
using System.Globalization;
using System.Windows.Data;

namespace The_Long_Dark_Save_Editor_2.Converters
{
    public class SkillProgressConverter : IValueConverter
    {
        private static readonly int[] thresholds = { 0, 10, 25, 50, 100 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int points = 0;
            if (value is int)
                points = (int)value;
            else if (value is double)
                points = (int)(double)value;
            else if (value is float)
                points = (int)(float)value;
            else if (value != null && int.TryParse(value.ToString(), out int parsed))
                points = parsed;

            int level = 1;
            for (int i = 1; i < thresholds.Length; i++)
            {
                if (points >= thresholds[i])
                    level = i + 1;
                else
                    break;
            }

            if (level >= thresholds.Length)
                return 1.0;

            int currentThreshold = thresholds[level - 1];
            int nextThreshold = thresholds[level];

            if (nextThreshold == currentThreshold)
                return 0.0;

            double progress = (double)(points - currentThreshold) / (nextThreshold - currentThreshold);
            return Math.Max(0, Math.Min(1, progress));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
