using System;
using System.Drawing;
using System.Reflection;
using Core.Common.Controls.Swf.Properties;
using Core.GIS.NetTopologySuite.Utilities;
using Steema.TeeChart.Tools;

namespace Core.Common.Controls.Swf.Charting
{
    ///<summary>
    /// Utility class for common functions used by chart tools
    ///</summary>
    public class TeeChartHelper
    {
        public static int GetNearestPoint(Steema.TeeChart.Styles.Series series, Point p, double tolerance)
        {
            int tmpMin = 0;
            int tmpMax = 0;

            int result = -1;
            int Dif = 10000;

            // Tool.GetFirstLastSeries is changed from public to internal and we do not want to modify TeeChart
            object[] args = new object[]
            {
                series,
                tmpMin,
                tmpMax
            };
            MethodInfo getFirstLastSeries = typeof(Tool).GetMethod("GetFirstLastSeries", BindingFlags.Static | BindingFlags.NonPublic);
            bool getFirstLastSeriesResult = (bool) getFirstLastSeries.Invoke(null, args);
            // retrieve the out parameters
            tmpMin = (int) args[1];
            tmpMax = (int) args[2];
            if (getFirstLastSeriesResult)
            {
                for (int t = tmpMin; t <= tmpMax; t++)
                {
                    int tmpX = series.CalcXPos(t);
                    int tmpY = series.CalcYPos(t);
                    Rectangle r = series.Chart.ChartRect;

                    if (r.Contains(tmpX, tmpY))
                    {
                        int Dist =
                            Steema.TeeChart.Utils.Round(
                                Math.Sqrt(Steema.TeeChart.Utils.Sqr(p.X - tmpX) + Steema.TeeChart.Utils.Sqr(p.Y - tmpY)));

                        if (Dist < Dif && Dist < tolerance)
                        {
                            Dif = Dist;
                            result = t;
                        }
                    }
                }
            }
            //THIS is done because Tool.GetFirstLastSeries(series, out tmpMin, out tmpMax)) is not documented (unable to find)
            Assert.IsTrue(result >= -1, Resources.TeeChartHelper_GetNearestPoint_Should_not_return_indexes_below__1__);
            return result;
        }

        public static Color DarkenColor(Color color, int percentage)
        {
            return Steema.TeeChart.Utils.DarkenColor(color, percentage);
        }
    }
}