using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace TaskTracker
{
    public class TruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                int maxLength = 20;

                // Убираем переводы строк и лишние пробелы
                string singleLineText = text.Replace("\n", " ").Replace("\r", " ").Trim();

                if (singleLineText.Length > maxLength)
                    return singleLineText.Substring(0, maxLength) + "...";

                return singleLineText;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
