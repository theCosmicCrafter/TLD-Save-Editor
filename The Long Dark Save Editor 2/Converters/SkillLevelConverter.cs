using System;
using System.Globalization;
using System.Windows.Data;

namespace The_Long_Dark_Save_Editor_2.Converters
{
    public class SkillLevelConverter : IValueConverter
    {
        private static readonly int[] thresholds = { 0, 10, 25, 50, 100 };
        private static readonly string[] names = { "Beginner", "Novice", "Apprentice", "Journeyman", "Expert" };

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

            int index = Math.Min(level - 1, names.Length - 1);
            return $"Level {level}: {names[index]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
