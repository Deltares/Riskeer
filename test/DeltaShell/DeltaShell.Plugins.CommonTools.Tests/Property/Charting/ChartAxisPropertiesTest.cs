using DelftTools.Controls.Swf.Charting;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Property.Charting
{
    [TestFixture]
    public class ChartAxisPropertiesTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowAxisProperties()
        {
            var chart = new Chart();
            WindowsFormsTestHelper.ShowPropertyGridForObject(new ChartAxisDoubleProperties(chart.LeftAxis));
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void ShowTimeAxisProperties()
        {
            var chart = new Chart();
            WindowsFormsTestHelper.ShowPropertyGridForObject(new ChartAxisDateTimeProperties(chart.LeftAxis));
        }
    }
}