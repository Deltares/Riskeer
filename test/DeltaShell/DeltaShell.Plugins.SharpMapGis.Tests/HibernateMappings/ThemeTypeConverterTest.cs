using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using DeltaShell.Plugins.SharpMapGis.HibernateMappings;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using NUnit.Framework;
using SharpMap.Api;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.HibernateMappings
{
    [TestFixture]
    public class ThemeTypeConverterTest
    {
        // Some test objects the tests can use
        private static readonly VectorStyle vectorStyle = new VectorStyle();
        private static readonly TypeConverter themeTC;
        private static readonly TypeConverter styleTC;

        /// <summary>
        /// Tests the functioning of the StyleTypeConverter
        /// </summary>
        [Test]
        public void ConvertStyleToCssAndBack()
        {
            DelftTools.Utils.TypeConverter.RegisterTypeConverter<ITheme, ThemeTypeConverter>();
            DelftTools.Utils.TypeConverter.RegisterTypeConverter<IStyle, StyleTypeConverter>();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // Convert the vectorstyle to css and back
            string css = styleTC.ConvertToString(vectorStyle);
            IStyle result = (IStyle) styleTC.ConvertFromString(css);

            Assert.IsNotNull(result as VectorStyle);

            // Check if all properties were restored to the original values
            VectorStyle vResult = (VectorStyle) result;
            Assert.IsTrue(CheckVectorStyleIsEqual(vectorStyle, vResult));
        }

        /// <summary>
        /// Test the basic class-to-xml capabilities of the ThemeTypeConverter
        /// </summary>
        [Test]
        public void ConvertThemeToXml()
        {
            ITheme theme = new QuantityTheme("column1", vectorStyle);
            string themeXml = (string) themeTC.ConvertTo(theme, typeof(string));

            Assert.IsTrue(themeXml.StartsWith("<?xml"));
        }

        [Test]
        public void CheckValuesAreBeingPersistedForThemes()
        {
            // Convert a test theme to xml and back
            var theme = new CategorialTheme
            {
                AttributeName = "", DefaultStyle = vectorStyle
            };
            var item1 = new CategorialThemeItem("0", vectorStyle, vectorStyle.Symbol, 0.0);
            var item2 = new CategorialThemeItem("1", vectorStyle, vectorStyle.Symbol, 1.0);
            var item3 = new CategorialThemeItem("2", vectorStyle, vectorStyle.Symbol, 2.0);
            var item4 = new CategorialThemeItem("3", vectorStyle, vectorStyle.Symbol, 3.0);

            theme.AddThemeItem(item1);
            theme.AddThemeItem(item2);
            theme.AddThemeItem(item3);
            theme.AddThemeItem(item4);

            string xml = themeTC.ConvertToString(theme);
            var retrievedTheme = (CategorialTheme) themeTC.ConvertFromString(xml);
            var retrievedItem1 = retrievedTheme.ThemeItems[0] as CategorialThemeItem;
            var retrievedItem2 = retrievedTheme.ThemeItems[1] as CategorialThemeItem;
            var retrievedItem3 = retrievedTheme.ThemeItems[2] as CategorialThemeItem;
            var retrievedItem4 = retrievedTheme.ThemeItems[3] as CategorialThemeItem;

            Assert.AreEqual(item1.Value, retrievedItem1.Value);
            Assert.AreEqual(item2.Value, retrievedItem2.Value);
            Assert.AreEqual(item3.Value, retrievedItem3.Value);
            Assert.AreEqual(item4.Value, retrievedItem4.Value);
        }

        [Test]
        public void ConvertThemeToXmlAndBack()
        {
            // Convert a test theme to xml and back
            QuantityTheme theme = new QuantityTheme("column1", vectorStyle);
            theme.AddStyle(vectorStyle, new Interval(0, 100));
            theme.AddStyle(vectorStyle, new Interval(101, 200));
            theme.NoDataValues = new[]
            {
                -999.0
            };

            string xml = themeTC.ConvertToString(theme);
            QuantityTheme retrievedTheme = (QuantityTheme) themeTC.ConvertFromString(xml);

            // Check if all properties were restored to the original values
            Assert.AreEqual(theme.AttributeName, retrievedTheme.AttributeName);
            Assert.IsTrue(CheckVectorStyleIsEqual((VectorStyle) theme.DefaultStyle,
                                                  (VectorStyle) retrievedTheme.DefaultStyle));

            Assert.IsTrue(CheckVectorStyleIsEqual((VectorStyle) theme.ThemeItems[0].Style,
                                                  (VectorStyle) retrievedTheme.ThemeItems[0].Style));
            Assert.IsTrue(CheckVectorStyleIsEqual((VectorStyle) theme.ThemeItems[1].Style,
                                                  (VectorStyle) retrievedTheme.ThemeItems[1].Style));
            Assert.AreEqual(theme.ThemeItems[0].Label, retrievedTheme.ThemeItems[0].Label);
            Assert.AreEqual(((QuantityThemeItem) theme.ThemeItems[0]).Interval.Min,
                            ((QuantityThemeItem) retrievedTheme.ThemeItems[0]).Interval.Min);
            Assert.AreEqual(((QuantityThemeItem) theme.ThemeItems[0]).Interval.Max,
                            ((QuantityThemeItem) retrievedTheme.ThemeItems[0]).Interval.Max);
            Assert.AreEqual(((QuantityThemeItem) theme.ThemeItems[1]).Interval.Min,
                            ((QuantityThemeItem) retrievedTheme.ThemeItems[1]).Interval.Min);
            Assert.AreEqual(((QuantityThemeItem) theme.ThemeItems[1]).Interval.Max,
                            ((QuantityThemeItem) retrievedTheme.ThemeItems[1]).Interval.Max);
            Assert.AreEqual(((QuantityThemeItem) theme.ThemeItems[1]).Interval.Max,
                            ((QuantityThemeItem) retrievedTheme.ThemeItems[1]).Interval.Max);
            Assert.AreEqual(new[]
            {
                -999.0
            }, retrievedTheme.NoDataValues);
            Assert.AreEqual(-999.0, retrievedTheme.NoDataValues[0]);
            Assert.AreEqual(typeof(double), retrievedTheme.NoDataValues[0].GetType());
        }

        /// <summary>
        /// JIRA TOOLS-1097: After a save load the NoDataValues in the theme seem lost.
        /// Actually they have changed type to double and effectively are ignored.
        /// 2 reasons to store NoDataValues in theme and not always copy from component in function.
        /// - Layers Datasource is not required to be a function, thus changing code must also 
        ///   account for this 
        /// - We want the user be able to use his/her own nodatavalues disconnected from the 
        ///   backing data only for presentation.
        /// </summary>
        [Test]
        public void ConvertQuantityThemeToXmlAndBackIntNoDataValues()
        {
            QuantityTheme theme = new QuantityTheme("column1", vectorStyle);
            theme.AddStyle(vectorStyle, new Interval(0, 100));
            theme.AddStyle(vectorStyle, new Interval(101, 200));
            theme.NoDataValues = new[]
            {
                -999
            };

            string xml = themeTC.ConvertToString(theme);
            QuantityTheme retrievedTheme = (QuantityTheme) themeTC.ConvertFromString(xml);
            Assert.AreEqual(new[]
            {
                -999
            }, retrievedTheme.NoDataValues);
            Assert.AreEqual(typeof(int), retrievedTheme.NoDataValues[0].GetType());
        }

        [Test]
        public void ConvertQuantityThemeToXmlAndBackFloatNoDataValues()
        {
            QuantityTheme theme = new QuantityTheme("column1", vectorStyle);
            theme.AddStyle(vectorStyle, new Interval(0, 100));
            theme.AddStyle(vectorStyle, new Interval(101, 200));
            theme.NoDataValues = new[]
            {
                -999.0F
            };

            string xml = themeTC.ConvertToString(theme);
            QuantityTheme retrievedTheme = (QuantityTheme) themeTC.ConvertFromString(xml);
            Assert.AreEqual(new[]
            {
                -999.0F
            }, retrievedTheme.NoDataValues);
            Assert.AreEqual(typeof(float), retrievedTheme.NoDataValues[0].GetType());
        }

        [Test]
        public void ConvertCategorialThemeWithEnumToXmlAndBackCheckValue()
        {
            var theme = new CategorialTheme();
            theme.AttributeName = "DataType";
            foreach (var shapeType in Enum.GetValues(typeof(ShapeType)))
            {
                theme.AddThemeItem(new CategorialThemeItem
                {
                    Value = shapeType,
                    Label = "<label>",
                    Style = new VectorStyle()
                });
            }

            string xml = themeTC.ConvertToString(theme);
            var retrievedTheme = (CategorialTheme) themeTC.ConvertFromString(xml);
            Assert.AreEqual(Enum.GetValues(typeof(ShapeType)).GetValue(0),
                            ((CategorialThemeItem) retrievedTheme.ThemeItems[0]).Value);
        }

        [Test]
        public void ConvertCategorialThemeWithBooleanToXmlAndBackCheckValue()
        {
            var theme = new CategorialTheme();
            theme.AttributeName = "IsTrue";
            theme.AddThemeItem(new CategorialThemeItem
            {
                Value = true,
                Label = "<label>",
                Style = new VectorStyle()
            });
            theme.AddThemeItem(new CategorialThemeItem
            {
                Value = false,
                Label = "<label>",
                Style = new VectorStyle()
            });

            string xml = themeTC.ConvertToString(theme);
            var retrievedTheme = (CategorialTheme) themeTC.ConvertFromString(xml);
            Assert.IsTrue((bool) ((CategorialThemeItem) retrievedTheme.ThemeItems[0]).Value);
        }

        /// <summary>
        /// Tests multiple themeitems and check if they are not default items.
        /// </summary>
        [Test]
        public void ConvertQuantityThemeToXmlAndBackThemeItems()
        {
            // Convert a test theme to xml and back
            QuantityTheme theme = new QuantityTheme("column1", vectorStyle);
            VectorStyle vectorStyleDiamond = new VectorStyle
            {
                Shape = ShapeType.Diamond, GeometryType = typeof(IPolygon)
            };
            VectorStyle vectorStyleEllipse = new VectorStyle
            {
                Shape = ShapeType.Ellipse, GeometryType = typeof(IPoint)
            };
            VectorStyle vectorStyleTriangle = new VectorStyle
            {
                Shape = ShapeType.Triangle, GeometryType = typeof(ILineString)
            };
            theme.AddStyle(vectorStyleDiamond, new Interval(0, 100));
            theme.AddStyle(vectorStyleEllipse, new Interval(101, 200));
            theme.AddStyle(vectorStyleTriangle, new Interval(201, 300));
            theme.NoDataValues = new[]
            {
                -999.0
            };

            string xml = themeTC.ConvertToString(theme);
            QuantityTheme retrievedTheme = (QuantityTheme) themeTC.ConvertFromString(xml);
            Assert.AreEqual(3, retrievedTheme.ThemeItems.Count);

            VectorStyle retrievedvectorStyleDiamond = (VectorStyle) retrievedTheme.ThemeItems[0].Style;
            Assert.AreEqual(vectorStyleDiamond.HasCustomSymbol, retrievedvectorStyleDiamond.HasCustomSymbol);
            Assert.AreEqual(vectorStyleDiamond.Shape, retrievedvectorStyleDiamond.Shape);
            Assert.AreEqual(vectorStyleDiamond.GeometryType, retrievedvectorStyleDiamond.GeometryType);

            VectorStyle retrievedvectorStyleEllipse = (VectorStyle) retrievedTheme.ThemeItems[1].Style;
            Assert.AreEqual(vectorStyleEllipse.Shape, retrievedvectorStyleEllipse.Shape);
            Assert.AreEqual(vectorStyleEllipse.GeometryType, retrievedvectorStyleEllipse.GeometryType);

            VectorStyle retrievedvectorStyleTriangle = (VectorStyle) retrievedTheme.ThemeItems[2].Style;
            Assert.AreEqual(vectorStyleTriangle.Shape, retrievedvectorStyleTriangle.Shape);
            Assert.AreEqual(vectorStyleTriangle.GeometryType, retrievedvectorStyleTriangle.GeometryType);
        }

        [Test]
        public void SaveLoadLayerWithGradientTheme()
        {
            var theme = new GradientTheme("aap", 5, 15, vectorStyle, vectorStyle, ColorBlend.BlueToRed,
                                          ColorBlend.GreenToBlue, ColorBlend.Rainbow5, 5)
            {
                UseCustomRange = true
            };
            string xml = themeTC.ConvertToString(theme);
            var retrievedTheme = (GradientTheme) themeTC.ConvertFromString(xml);

            Assert.AreEqual(theme.Max, retrievedTheme.Max);
            Assert.AreEqual(theme.Min, retrievedTheme.Min);
            Assert.AreEqual(true, retrievedTheme.UseCustomRange);
            Assert.AreEqual(theme.NumberOfClasses, retrievedTheme.NumberOfClasses);
        }

        static ThemeTypeConverterTest()
        {
            // Set up a IStyle objects
            vectorStyle.Line = new Pen(Color.Red, 2);
            vectorStyle.Outline = new Pen(Color.Blue, 3);
            vectorStyle.EnableOutline = true;
            vectorStyle.Fill = Brushes.Yellow;

            DelftTools.Utils.TypeConverter.RegisterTypeConverter<ITheme, ThemeTypeConverter>();
            DelftTools.Utils.TypeConverter.RegisterTypeConverter<IStyle, StyleTypeConverter>();
            themeTC = TypeDescriptor.GetConverter(typeof(ITheme));
            styleTC = TypeDescriptor.GetConverter(typeof(IStyle));
        }

        /// <summary>
        /// Compare the two VectorStyle objects on all CSS-serializable properties
        /// </summary>
        /// <param name="t1">The first <see cref="VectorStyle"/> object (for example the original)</param>
        /// <param name="t2">The second <see cref="VectorStyle"/> object (for example one that was created from CSS)</param>
        /// <returns>If both VectorStyle's have all CSS-serializable properties set to equal values</returns>
        private static bool CheckVectorStyleIsEqual(VectorStyle t1, VectorStyle t2)
        {
            Assert.AreEqual(t1.Line.Color, t2.Line.Color);
            Assert.AreEqual(t1.Line.Width, t2.Line.Width);
            Assert.AreEqual(t1.Outline.Color, t2.Outline.Color);
            Assert.AreEqual(t1.Outline.Width, t2.Outline.Width);
            Assert.AreEqual(((SolidBrush) t1.Fill).Color.ToArgb(), ((SolidBrush) t2.Fill).Color.ToArgb());
            return true;
        }
    }
}