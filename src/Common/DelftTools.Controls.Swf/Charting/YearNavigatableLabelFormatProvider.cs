using System;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf.Charting
{
    /// <summary>
    /// Basically (partial) example code for extending TimeNavigatableLabelFormatProvider.
    /// </summary>
    public class YearNavigatableLabelFormatProvider : TimeNavigatableLabelFormatProvider
    {
        public override string GetLabel(DateTime labelValue, TimeSpan duration)
        {
            return labelValue.Year.ToString();  //always show just years, independent of duration
        }

        public override string GetUnits(TimeSpan duration)
        {
            return "yyyy";
        }

        public override string GetRangeLabel(DateTime min, DateTime max)
        {
            if (min.Year == max.Year)
            {
                return min.Year.ToString();
            }
            return min.Year + " till " + max.Year;
        }
    }
}
