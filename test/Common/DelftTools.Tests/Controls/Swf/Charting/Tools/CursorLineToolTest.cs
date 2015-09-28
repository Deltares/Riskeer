using DelftTools.Controls.Swf.Charting;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting.Tools
{
    [TestFixture]
    public class CursorLineToolTest
    {
        [Test]
        public void SettingXValueShouldWork()
        {
            var chartView = new ChartView();
            var tool = chartView.NewCursorLineTool(CursorLineToolStyles.Horizontal);
            const double value = 10.0;
            tool.XValue = value;
            Assert.AreEqual(value,tool.XValue);
        }
    }
}