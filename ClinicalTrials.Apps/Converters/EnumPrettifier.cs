using System.Globalization;

namespace ClinicalTrials.Apps
{
    class EnumPrettifier : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var type = value.GetType();
            if (type.IsEnum)
            {
                string? enumValue = Enum.GetName(type, value);
                if (enumValue != null)
                {
                    enumValue = enumValue.Replace('_', ' ');
                    return TextInfo.ToTitleCase(enumValue.ToLowerInvariant());
                }
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
    }
}
