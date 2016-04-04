using System.Drawing;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class PolygonStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            var fillColor = Color.AliceBlue;
            var strokeColor = Color.Gainsboro;
            var width = 3;

            // Call
            var polygonStyle = new PolygonStyle(fillColor, strokeColor, width);

            // Assert
            Assert.AreEqual(fillColor, polygonStyle.FillColor);
            Assert.AreEqual(strokeColor, polygonStyle.StrokeColor);
            Assert.AreEqual(width, polygonStyle.Width);
        }  
    }
}