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
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                string enumValue = Enum.GetName(type, value);
                enumValue = enumValue.Replace('_', ' ');
                return textInfo.ToTitleCase(enumValue.ToLowerInvariant());
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
