using System;
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
            return GetQuarterStringForDateTime(min) + " till " + GetQuarterStringForDateTime(max);
        }

        public override string GetUnits(TimeSpan duration)
        {
            return "qtr yyyy";
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
                    return "1st Qtr " + labelValue.Year;
                case 2:
                    return "2nd Qtr " + labelValue.Year;
                case 3:
                    return "3rd Qtr " + labelValue.Year;
                case 4:
                    return "4th Qtr " + labelValue.Year;
                default:
                    return "<Unknown quarter>";
            }
        }
    }
}