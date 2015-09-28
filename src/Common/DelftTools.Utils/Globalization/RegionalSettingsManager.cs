using System;
using System.Globalization;
using System.Threading;

namespace DelftTools.Utils.Globalization
{
    /// <summary>
    /// TODO: extend with DateTime formatting
    /// TODO: extend with IntegerNumber formatting
    /// TODO: extend with custom formatters (e.g. variable specific)
    /// ...
    /// </summary>
    public static class RegionalSettingsManager
    {
        private static readonly CustomFormatProvider customFormatProvider = new CustomFormatProvider();

        public static event Action LanguageChanged;
        public static event Action FormatChanged;

        private static string realNumberFormat = "G5";
        
        /// <summary>
        /// Language in the form of standard cultures "en-US", "ru-RU" ...
        /// </summary>
        public static string Language { 
            set
            {
                var ci = new CultureInfo(value)
                {
                    NumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat,
                    DateTimeFormat = CreateDateTimeFormatFromSystemSettingsWithoutNameLocalization(),
                };

                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = ci;

                if(LanguageChanged != null)
                {
                    LanguageChanged();
                }
            }

            get
            {
                return Thread.CurrentThread.CurrentCulture.Name;
            }
        }

        private static DateTimeFormatInfo CreateDateTimeFormatFromSystemSettingsWithoutNameLocalization()
        {
            var systemCulture = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            var ci = CultureInfo.InvariantCulture;

            var localMachineDateTimeFormat = (DateTimeFormatInfo)systemCulture.Clone();
            //don't take the localized names!
            localMachineDateTimeFormat.DayNames = ci.DateTimeFormat.DayNames; 
            localMachineDateTimeFormat.MonthNames = ci.DateTimeFormat.MonthNames;
            localMachineDateTimeFormat.AbbreviatedDayNames = ci.DateTimeFormat.AbbreviatedDayNames;
            localMachineDateTimeFormat.AbbreviatedMonthGenitiveNames = ci.DateTimeFormat.AbbreviatedMonthGenitiveNames;
            localMachineDateTimeFormat.AbbreviatedMonthNames = ci.DateTimeFormat.AbbreviatedMonthNames;
            return localMachineDateTimeFormat;
        }

        public static CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public static IFormatProvider GetCustomFormatProvider()
        {
            return customFormatProvider;
        }

        /// <summary>
        /// TODO: make it configurable
        /// </summary>
        public static string DateTimeFormat
        {
            get
            {
                return String.Format("yyyy{0}MM{0}dd HH{1}mm{1}ss",
                                     CurrentCulture.DateTimeFormat.DateSeparator,
                                     CurrentCulture.DateTimeFormat.TimeSeparator);
            }
        }

        /// <summary>
        /// Set formatting for real numbers (double, float). See standard .NET formatting strings for more info: http://msdn.microsoft.com/en-us/library/0c899ak8.aspx
        /// http://www.csharp-examples.net/string-format-double/
        /// </summary>
        public static string RealNumberFormat
        {
            get 
            {
                return realNumberFormat;
            }
            set
            {
                realNumberFormat = value;
                if(FormatChanged != null)
                {
                    FormatChanged();
                }
            }
        }

        /// <summary>
        /// TODO: how to make .NET use this FormatProvider instead of CurrentCulture.NumberInfo? Sealed class problem
        /// </summary>
        private class CustomFormatProvider : IFormatProvider, ICustomFormatter
        {
            public object GetFormat(Type formatType)
            {
                return (formatType == typeof(ICustomFormatter)) ? this : null;
            }

            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                var argType = arg.GetType();
                if ((argType == typeof(double) || argType == typeof(float)))
                {
                    return string.Format(CurrentCulture, "{" + realNumberFormat + "}", arg);
                }

                return string.Format(CurrentCulture, format, arg);
            }
        }

        public static string ConvertToString(object value, bool truncateNumbers=true)
        {
            if (value is DateTime)
            {
                return ((DateTime) value).ToString(DateTimeFormat);
            }
            if (truncateNumbers && (value is double || value is float || value is decimal))
            {
                return ((double) value).ToString(RealNumberFormat);
            }
            return Convert.ToString(value, CurrentCulture);
        }
    }
}