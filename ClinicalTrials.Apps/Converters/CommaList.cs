using System.Collections;
using System.Globalization;

namespace ClinicalTrials.Apps
{
    class CommaList : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? commaList = null;
            var list = value as IEnumerable;
            if (list != null)
            {
                foreach (var item in list)
                {
                    var itemStr = item as string;
                    if (itemStr == null)
                    {
                        var type = item.GetType();
                        if (type.IsEnum)
                        {
                            itemStr = Enum.GetName(type, item)?.ToLowerInvariant();
                            if (itemStr != null)
                            {
                                itemStr = TextInfo.ToTitleCase(itemStr);
                            }
                        }
                    }

                    if (commaList == null)
                    {
                        commaList = itemStr;
                    }
                    else
                    {
                        commaList += ", " + itemStr;
                    }
                }
            }

            return commaList;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;
    }
}
