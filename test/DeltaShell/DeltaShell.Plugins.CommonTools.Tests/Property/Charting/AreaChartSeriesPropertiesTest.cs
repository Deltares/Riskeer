using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property.Charting
{
    [TestFixture]
    public class AreaChartSeriesPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowAreaChartSeriesPropertiesInPropertyGrid()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new AreaChartSeriesProperties { Data = ChartSeriesFactory.CreateAreaSeries() });
        }
    }
}