using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property.Charting
{
    [TestFixture]
    public class PointChartSeriesPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowPointChartSeriesPropertiesInPropertyGrid()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new PointChartSeriesProperties { Data = ChartSeriesFactory.CreatePointSeries() });
        }
    }
}