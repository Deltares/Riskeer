using System;
using System.Globalization;
using System.Threading;

namespace Core.Common.Utils.Globalization
{
    /// <summary>
    /// TODO: extend with DateTime formatting
    /// TODO: extend with IntegerNumber formatting
    /// TODO: extend with custom formatters (e.g. variable specific)
    /// ...
    /// </summary>
    public static class RegionalSettingsManager
    {
        public static event Action FormatChanged;

        private static string realNumberFormat = "G5";

        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
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
                if (FormatChanged != null)
                {
                    FormatChanged();
                }
            }
        }

        public static string ConvertToString(object value, bool truncateNumbers = true)
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