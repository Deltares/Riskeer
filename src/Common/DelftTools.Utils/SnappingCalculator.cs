using System;
using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils
{
    public static class SnappingCalculator // TODO: the class has very bad name, move it next to TimeSeriesNavigator
    {
        /// <summary>
        /// Get nearest value in times for value.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetNearestDefinedTime(IEnumerable<DateTime> times, DateTime value)
        {
            if (!times.Any())
            {
                return default(DateTime);
            }

            //minimun
            var minDuration = times.Select(t=>(t - value).Duration()).Min();
            //selection the time with this minimum
            var q = from t in times
                    where (t - value).Duration()== minDuration
                    select t;
            
            return q.FirstOrDefault();
        }

        public static DateTime? GetLastTimeInRange(IEnumerable<DateTime> times, DateTime? start, DateTime? end)
        {
            var time = times.OrderByDescending(t=>t).FirstOrDefault(t => t >= start && t<=end);
            return time == default(DateTime) ? (DateTime?)null : time;
        }

        public static DateTime? GetFirstTimeInRange(IEnumerable<DateTime> times, DateTime? start, DateTime? end)
        {
            var time = times.OrderBy(t => t).FirstOrDefault(t => t >= start && t <= end);
            return time == default(DateTime) ? (DateTime?) null : time;
        }

        public static DateTime? GetFirstTimeLeftOfValue(IEnumerable<DateTime> times, DateTime? start)
        {
            var time = times.OrderByDescending(t => t).FirstOrDefault(t => t <= start);
            return time == default(DateTime) ? (DateTime?)null : time;
        }
    }
}