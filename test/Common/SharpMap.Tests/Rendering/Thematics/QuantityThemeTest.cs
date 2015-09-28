using System.Collections.Generic;
using System.Drawing;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using NUnit.Framework;
using SharpMap.Api;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class QuantityThemeTest
    {
        // Some test objects the tests can use


        private static IStyle GenerateVectorStyle(Brush brush)
        {
            return new VectorStyle
                       {
                           Line = new Pen(Color.Red, 2),
                           Outline = new Pen(Color.Blue, 3),
                           EnableOutline = true,
                           Fill = brush
                       };
        }

        [Test]
        public void QuantityThemeColorTest()
        {
            var defaultStyle = GenerateVectorStyle(Brushes.Black);
            var quantityTheme = new QuantityTheme("test", defaultStyle);

            var redStyle = GenerateVectorStyle(Brushes.Red);
            var whiteStyle = GenerateVectorStyle(Brushes.White);
            var blueStyle = GenerateVectorStyle(Brushes.Blue);

            //add styles in random order
            quantityTheme.AddStyle(redStyle, new Interval(0.0, 1.0));
            quantityTheme.AddStyle(blueStyle, new Interval(2.0, 3.0));
            quantityTheme.AddStyle(whiteStyle, new Interval(1.0, 2.0));
            quantityTheme.NoDataValues = new[] {-999.0};


            //value within interval
            Assert.IsTrue(CompareRGBColor(Color.Red, quantityTheme.GetFillColor(0.5)));
            Assert.IsTrue(CompareRGBColor(Color.White, quantityTheme.GetFillColor(1.5)));
            Assert.IsTrue(CompareRGBColor(Color.Blue, quantityTheme.GetFillColor(2.5)));


            //no data value
            Assert.IsTrue(CompareRGBColor(Pens.Transparent.Color, quantityTheme.GetFillColor(-999.0)));
        }

        private static bool CompareRGBColor(Color color, Color fillColor)
        {
            return color.R == fillColor.R && color.G == fillColor.G && color.B == fillColor.B;
        }

        [Test]
        public void CloneQuantityThemeWithNoDataValues()
        {
            var quantityTheme = new QuantityTheme("aa",new VectorStyle()){ NoDataValues = new List<double> { -9999 } };
            var quantityThemeClone = quantityTheme.Clone();

            Assert.AreEqual(quantityTheme.NoDataValues, ((QuantityTheme)quantityThemeClone).NoDataValues);
        }
    }
}