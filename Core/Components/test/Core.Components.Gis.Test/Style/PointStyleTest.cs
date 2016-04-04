using System.Drawing;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class PointStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            var color = Color.AliceBlue;
            var width = 3;
            var style = PointSymbol.Square;

            // Call
            var pointStyle = new PointStyle(color, width, style);

            // Assert
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(style, pointStyle.Symbol);
        }  
    }
}