using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataHelperTest
    {
        [Test]
        public void Convert_Circle_ReturnDefault()
        {
            // Call
            var symbol = MapDataHelper.Convert(PointSymbol.Circle);

            // Assert
            Assert.AreEqual(PointShape.Undefined, symbol);
        } 

        [Test]
        public void Convert_Square_ReturnRectangle()
        {
            // Call
            var symbol = MapDataHelper.Convert(PointSymbol.Square);

            // Assert
            Assert.AreEqual(PointShape.Rectangle, symbol);
        } 

        [Test]
        public void Convert_Triangle_ReturnTriangle()
        {
            // Call
            var symbol = MapDataHelper.Convert(PointSymbol.Triangle);

            // Assert
            Assert.AreEqual(PointShape.Triangle, symbol);
        } 
    }
}