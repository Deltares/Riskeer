using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property.Charting
{
    [TestFixture]
    public class ChartSeriesPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowChartSeriesPropertiesInPropertyGrid()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new ChartSeriesProperties<BarSeries> { Data = ChartSeriesFactory.CreateBarSeries() });
        }
    }
}