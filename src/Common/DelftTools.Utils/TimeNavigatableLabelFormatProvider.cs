using System;
using System.Globalization;
using DelftTools.Utils.Properties;

namespace DelftTools.Utils
{
    /// <summary>
    /// Provides custom DateTime formatting (e.g. for use on the chart axes based on the zoom level). More detail (eg time) when zoomed in etc. 
    /// Also sets the axis title with the current zoom range. This class contains two methods that can be overwritten to implement
    /// custom DateTime formatting behavior (eg, show Quarters instead of Months, etc).
    /// </summary>
    public class TimeNavigatableLabelFormatProvider
    {
        private static readonly string strTill = Resource.strTill;

        /// <summary>
        /// Initializes the provider, sets the default CustomDateTimeFormatInfo.
        /// </summary>
        public TimeNavigatableLabelFormatProvider()
        {
            ShowRangeLabel = true; //default
            ShowUnits = true;
            CustomDateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
        }

        /// <summary>
        /// The culture-specific CustomDateTimeFormatInfo to use when rendering the date times. 
        /// Default is the CurrentCulture.CustomDateTimeFormatInfo.
        /// </summary>
        public DateTimeFormatInfo CustomDateTimeFormatInfo { get; set; }

        /// <summary>
        /// Indicates whether the range label (typically with the date/time range) is shown. Default true.
        /// </summary>
        public virtual bool ShowRangeLabel { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether units are shown for time.
        /// TODO: move it to FunctionBindingList (presentation facade)
        /// </summary>
        public bool ShowUnits { get; set; }

        /// <summary>
        /// Should be overwritten to implement custom label text for the labelValue.
        /// </summary>
        /// <param name="labelValue">The datetime value to provide a formatted string for.</param>
        /// <param name="duration">The current (zoomed) axis range. Typically if the range is large, 
        /// you can omit details such as hours and seconds.</param>
        /// <returns></returns>
        public virtual string GetLabel(DateTime labelValue, TimeSpan duration)
        {
            return labelValue.ToString(GetUnits(duration));
        }

        /// <summary>
        /// Units of measure for a given duration. Not included in labels! Client code should show this units depending on ShowUnits flag.
        /// </summary>
        /// <param name="duration">The current (zoomed) axis range. Typically if the range is large, 
        /// you can omit details such as hours and seconds.</param>
        /// <returns></returns>
        public virtual string GetUnits(TimeSpan duration)
        {
            string format = "";

            if (duration.TotalHours < 1)
            {
                format = CustomDateTimeFormatInfo.LongTimePattern;
            }
            else if (duration.TotalDays < 1)
            {
                format = CustomDateTimeFormatInfo.ShortTimePattern;
            }
            else if (duration.TotalDays < 5)
            {
                format = CustomDateTimeFormatInfo.ShortDatePattern + " " + CustomDateTimeFormatInfo.ShortTimePattern;
            }
            else if (duration.TotalDays < 30)
            {
                format = CustomDateTimeFormatInfo.ShortDatePattern;
            }
            else
            {
                format = CustomDateTimeFormatInfo.YearMonthPattern;
            }

            return format;
        }

        /// <summary>
        /// Should be overwritten to implement a custom axis title text. Requires ShowRangeLabel to be true. 
        /// If you return null, the original title is kept. Default is "date / time ([long date min] till [long date max])"
        /// </summary>
        /// <param name="min">Minimum value of axis</param>
        /// <param name="max">Maximum value of axis</param>
        /// <returns></returns>
        public virtual string GetRangeLabel(DateTime min, DateTime max)
        {
            TimeSpan span = max - min;

            if (max.DayOfYear == min.DayOfYear && max.Year == min.Year) //same day: show only one day
            {
                return "(" + max.ToString(CustomDateTimeFormatInfo.LongDatePattern) + ")";
            }
            else
            {
                return "(" + min.ToString(CustomDateTimeFormatInfo.LongDatePattern) + " " + strTill + " " +
                       max.ToString(CustomDateTimeFormatInfo.LongDatePattern) + ")";
            }
        }
    }
}