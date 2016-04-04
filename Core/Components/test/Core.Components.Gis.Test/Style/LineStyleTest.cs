using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class LineStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            var color = Color.AliceBlue;
            var width = 3;
            var style = DashStyle.Solid;

            // Call
            var lineStyle = new LineStyle(color, width, style);

            // Assert
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        } 
    }
}