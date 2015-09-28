using System.Drawing;
using System.IO;
using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DeltaShell.Plugins.SharpMapGis.HibernateMappings;
using NUnit.Framework;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.HibernateMappings
{
    [TestFixture, System.ComponentModel.Category("DataAccess")]
    public class ThemeFileExporterTest
    {
        private static readonly VectorStyle vectorStyle;

        static ThemeFileExporterTest()
        {
            // Set up a IStyle objects
            vectorStyle = new VectorStyle
                              {
                                  Line = new Pen(Color.Red, 2),
                                  Outline = new Pen(Color.Blue, 3),
                                  EnableOutline = true,
                                  Fill = Brushes.Yellow
                              };

            TypeConverter.RegisterTypeConverter<ITheme, ThemeTypeConverter>();
            TypeConverter.RegisterTypeConverter<IStyle, StyleTypeConverter>();
        }

        [Test]
        public void WriteQuantityThemeToFile()
        {
            ITheme theme = new QuantityTheme("column1", vectorStyle);
            string path = TestHelper.GetCurrentMethodName() + ".dsleg";
            IFileExporter exporter = new ThemeFileExporter();
            exporter.Export(theme, path);

            string xml;
            using (TextReader reader = new StreamReader(path))
            {
                xml = reader.ReadToEnd();
            }

            Assert.IsTrue(xml.StartsWith("<?xml"));
        }
    }
}