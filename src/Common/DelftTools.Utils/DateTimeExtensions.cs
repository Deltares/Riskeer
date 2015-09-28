using System;

namespace DelftTools.Utils
{
    public static class DateTimeExtensions
    {
        public static double ToModifiedJulianDay(this DateTime dateTime)
        {
            var jdt = ToJulianDate(dateTime);
            return Math.Truncate(jdt - 2400000.5);
        }

        public static double ToJulianDate(this DateTime dateTime)
        {
            int y = dateTime.Year;
            int m = dateTime.Month;
            int d = dateTime.Day;
            int h = dateTime.Hour;
            int mn = dateTime.Minute;
            int s = dateTime.Second;

            double jy;
            double ja;
            double jm;


            if (m > 2)
            {
                jy = y;
                jm = m + 1;
            }
            else
            {
                jy = y - 1;
                jm = m + 13;
            }

            double intgr = Math.Floor(Math.Floor(365.25 * jy) + Math.Floor(30.6001 * jm) + d + 1720995);

            //check for switch to Gregorian calendar  
            int gregcal = 15 + 31 * (10 + 12 * 1582);
            if (d + 31 * (m + 12 * y) >= gregcal)
            {
                ja = Math.Floor(0.01 * jy);
                intgr += 2 - ja + Math.Floor(0.25 * ja);
            }

            //correct for half-day offset  
            double dayfrac = h / 24.0 - 0.5;
            if (dayfrac < 0.0)
            {
                dayfrac += 1.0;
                --intgr;
            }

            //now set the fraction of a day  
            double frac = dayfrac + (mn + s / 60.0) / 60.0 / 24.0;

            //round to nearest second  
            double jd0 = (intgr + frac) * 100000;
            double jd = Math.Floor(jd0);
            if (jd0 - jd > 0.5) ++jd;

            return jd / 100000;
        }

        public static double ToDecimalYear(this DateTime date)
        {
            var year = date.Year;
            if (year > 9998)
            {
                year = year - ((int)Math.Ceiling((year - 9998.0) / 4)) * 4;
            }

            var firstJan = new DateTime(year, 1, 1);
            var nextYear = firstJan.AddYears(1);

            if (date == firstJan)
                return year;

            var fraction = (date - firstJan).TotalMilliseconds / (nextYear - firstJan).TotalMilliseconds;

            return year + fraction;
        }
    }
}