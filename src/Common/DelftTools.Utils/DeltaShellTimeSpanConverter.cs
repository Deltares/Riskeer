using System;
using System.ComponentModel;
using System.Globalization;

namespace DelftTools.Utils
{
    public class DeltaShellTimeSpanConverter: TimeSpanConverter
    {
        const string format = "dd HH:mm:ss";

        /// <summary>
        /// converts datetime to format yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var timespan= (TimeSpan)value;
            var result = string.Format("{0}d {1:00}:{2:00}:{3:00}",Math.Floor(timespan.TotalDays),timespan.Hours,timespan.Minutes,timespan.Seconds);
            return result;
        }

        /// <summary>
        /// converts datetime from format yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var dateString = (string)value;
                try
                {
                    var days = Convert.ToInt32(dateString.Substring(0, dateString.IndexOf("d")));
                    dateString = dateString.Remove(0, dateString.IndexOf("d")+1).Trim();
                    var others = dateString.Split(':');
                    var hours = Convert.ToInt32(others[0]);
                    var minutes = Convert.ToInt32(others[1]);
                    var seconds = Convert.ToInt32(others[2]);
                    return new TimeSpan(days, hours, minutes, seconds);
                }
                catch
                {
                    throw new FormatException(string.Format("{0} is not a valid DateTime value. The format should be {1}",dateString ,format));
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}