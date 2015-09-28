using System.Drawing;
using System.Drawing.Drawing2D;
using DeltaShell.Plugins.SharpMapGis.HibernateMappings;
using GeoAPI.Geometries;
using NUnit.Framework;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.HibernateMappings
{
    [TestFixture]
    public class StyleTypeConverterTest
    {
        [Test]
        public void ConvertToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            //get a  style and convert it back and forth
            //styleTypeConverter.ConvertToString()
            var vectorStyle = new VectorStyle {Line = {EndCap = LineCap.ArrowAnchor}};
            //serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            //assert both are same.
            Assert.AreEqual(vectorStyle.Line.EndCap,retrievedStyle.Line.EndCap);
        }

        [Test]
        public void ColorTranslatorHtmlTest()
        {
            Color color = Color.FromArgb(11, 22, 33, 44);
            string htmlColor = ColorTranslator.ToHtml(color);
            Color convertedColor = ColorTranslator.FromHtml(htmlColor);
            Assert.AreEqual(22, convertedColor.R);
            Assert.AreEqual(33, convertedColor.G);
            Assert.AreEqual(44, convertedColor.B);
            // html does not support the alpha channel; 
            // also see ConvertColorToAndFrom
            Assert.AreNotEqual(11, convertedColor.A); // value is 255
        }

        [Test]
        public void ColorTranslatorWin32Test()
        {
            Color color = Color.FromArgb(11, 22, 33, 44);
            int win32Color = ColorTranslator.ToWin32(color);
            Color convertedColor = ColorTranslator.FromWin32(win32Color);
            Assert.AreEqual(22, convertedColor.R);
            Assert.AreEqual(33, convertedColor.G);
            Assert.AreEqual(44, convertedColor.B);
            // also ColorTranslator.@@Win32 does not support the alpha channel; 
            // also see ConvertColorToAndFrom
            Assert.AreNotEqual(11, convertedColor.A); // value is 255
        }

        [Test]
        public void ConvertColorToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            // get a style and convert it back and forth
            // styleTypeConverter.ConvertToString()
            var vectorStyle = new VectorStyle {Line = {Color = Color.FromArgb(11, 22, 33, 44)}};
            // serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            // assert both are same.
            Assert.AreEqual(22, retrievedStyle.Line.Color.R);
            Assert.AreEqual(33, retrievedStyle.Line.Color.G);
            Assert.AreEqual(44, retrievedStyle.Line.Color.B);
            Assert.AreEqual(11, retrievedStyle.Line.Color.A);
        }

        [Test]
        public void ConvertNamedColorToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            // get a style and convert it back and forth
            var vectorStyle = new VectorStyle { Line = { Color = Color.Red } };
            // serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            // assert both are same.
            Assert.AreEqual(Color.Red, retrievedStyle.Line.Color);

            vectorStyle = new VectorStyle { Line = { Color = SystemColors.ButtonFace } };
            xml = styleTypeConverter.ConvertToString(vectorStyle);
            retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            // assert both are same.
            Assert.AreEqual(SystemColors.ButtonFace, retrievedStyle.Line.Color);
        }

        [Test]
        public void ConvertDiamondShapeToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            VectorStyle vectorStyle = new VectorStyle { Shape = ShapeType.Diamond, GeometryType = typeof(IPolygon) };
            // serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            // assert both are same.
            Assert.AreEqual(ShapeType.Diamond, retrievedStyle.Shape);
            Assert.AreEqual(typeof(IPolygon), retrievedStyle.GeometryType);
        }

        [Test]
        public void ConvertEllipseShapeToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            VectorStyle vectorStyle = new VectorStyle { Shape = ShapeType.Ellipse, GeometryType = typeof(IPoint) };
            // serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            // assert both are same.
            Assert.AreEqual(ShapeType.Ellipse, retrievedStyle.Shape);
            Assert.AreEqual(typeof(IPoint), retrievedStyle.GeometryType);
        }

        [Test]
        public void ConvertSymbolToAndFromString()
        {
            StyleTypeConverter styleTypeConverter = new StyleTypeConverter();
            VectorStyle vectorStyle = new VectorStyle { Symbol = new Bitmap(10, 10) };
            // serialize to xml and back
            string xml = styleTypeConverter.ConvertToString(vectorStyle);
            VectorStyle retrievedStyle = (VectorStyle)styleTypeConverter.ConvertFrom(xml);
            Assert.AreEqual(true, retrievedStyle.HasCustomSymbol);
            Assert.IsNotNull(retrievedStyle.Symbol);
        }
    }
}