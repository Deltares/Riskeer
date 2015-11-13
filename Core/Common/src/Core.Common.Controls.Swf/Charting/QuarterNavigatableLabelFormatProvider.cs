using System;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf.Charting
{
    /// <summary>
    /// Basically (partial) example code for extending TimeNavigatableLabelFormatProvider.
    /// </summary>
    public class QuarterNavigatableLabelFormatProvider : TimeNavigatableLabelFormatProvider
    {
        public override string GetLabel(DateTime labelValue, TimeSpan duration)
        {
            return GetQuarterStringForDateTime(labelValue); //always show just quarters, independent of duration
        }

        public override string GetRangeLabel(DateTime min, DateTime max)
        {
            if (min.Year == max.Year && GetQuarterNumber(min) == GetQuarterNumber(max))
            {
                return GetQuarterStringForDateTime(min);
            }
            return string.Format(Resources.RangeLabel__0__till__1_, GetQuarterStringForDateTime(min), GetQuarterStringForDateTime(max));
        }

        public override string GetUnits(TimeSpan duration)
        {
            return Resources.QuarterNavigatableLabelFormatProvider_GetUnits_qtr_yyyy;
        }

        private static int GetQuarterNumber(DateTime min)
        {
            return ((min.Month - 1)/3) + 1;
        }

        private static string GetQuarterStringForDateTime(DateTime labelValue)
        {
            switch (GetQuarterNumber(labelValue)) //localization is your own responsibility here
            {
                case 1:
                    return string.Format(Resources.QuarterNavigatableLabelFormatProvider_GetQuarterStringForDateTime__1st_Qtr_, labelValue.Year);
                case 2:
                    return string.Format(Resources.QuarterNavigatableLabelFormatProvider_GetQuarterStringForDateTime__2nd_Qtr_, labelValue.Year);
                case 3:
                    return string.Format(Resources.QuarterNavigatableLabelFormatProvider_GetQuarterStringForDateTime__3rd_Qtr_, labelValue.Year);
                case 4:
                    return string.Format(Resources.QuarterNavigatableLabelFormatProvider_GetQuarterStringForDateTime__4th_Qtr_, labelValue.Year);
                default:
                    return Resources.QuarterNavigatableLabelFormatProvider_GetQuarterStringForDateTime__Unknown_quarter_;
            }
        }
    }
}