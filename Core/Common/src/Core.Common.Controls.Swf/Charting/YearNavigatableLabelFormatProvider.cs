using System;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf.Charting
{
    /// <summary>
    /// Basically (partial) example code for extending TimeNavigatableLabelFormatProvider.
    /// </summary>
    public class YearNavigatableLabelFormatProvider : TimeNavigatableLabelFormatProvider
    {
        public override string GetLabel(DateTime labelValue, TimeSpan duration)
        {
            return labelValue.Year.ToString(); //always show just years, independent of duration
        }

        public override string GetUnits(TimeSpan duration)
        {
            return Resources.YearNavigatableLabelFormatProvider_GetUnits_yyyy;
        }

        public override string GetRangeLabel(DateTime min, DateTime max)
        {
            if (min.Year == max.Year)
            {
                return min.Year.ToString();
            }
            return string.Format(Resources.RangeLabel__0__till__1_, min.Year, max.Year);
        }
    }
}