using DelftTools.Controls.Swf.Charting;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property.Charting
{
    [TestFixture]
    public class ChartPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowChartPropertiesInPropertyGrid()
        {
            WindowsFormsTestHelper.ShowPropertyGridForObject(new ChartProperties { Data = new Chart() });
        }
    }
}