using System.Drawing;
using NUnit.Framework;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests
{
    [TestFixture]
    public class LegFileConverterTest
    {
        [Test]
        public void ConvertToQuantityTheme()
        {
            const string legFileText = "[Z-data]\n" +
                                       "Version=3\n" +
                                       "ClassificationType=7\n" +
                                       "ClassificationFlipped=0\n" +
                                       "ColorScheme=-999\n" +
                                       "ColorSchemeFlipped=0\n" +
                                       "MinValue=-437.5\n" +
                                       "MaxValue=3500\n" +
                                       "StepValue=393.75\n" +
                                       "MissingValueEnabled=-1\n" +
                                       "MissingValueColor=FFFFFF\n" +
                                       "MissingValueInLegend=-1\n" +
                                       "MissingValueMin=-1601\n" +
                                       "MissingValueMax=-1601\n" +
                                       "MissingValueTransparent=-1\n" +
                                       "NoDataValueEnabled=-1\n" +
                                       "NoDataValueColor=FFFFFF\n" +
                                       "NoDataValueInLegend=-1\n" +
                                       "NoDataValueMin=-999\n" +
                                       "NoDataValueMax=-999\n" +
                                       "NoDataValueTransparent=-1\n" +
                                       "ZeroValueEnabled=0\n" +
                                       "ZeroValueColor=80FFFF\n" +
                                       "ZeroValueInLegend=-1\n" +
                                       "ZeroValueMin=0\n" +
                                       "ZeroValueMax=0\n" +
                                       "ZeroValueTransparent=0\n" +
                                       "ValueFormat=\"0.00\"\n" +
                                       "RangeFormat=0\n" +
                                       "RangeSizeFirst=2\n" +
                                       "RangeSizeLast=40\n" +
                                       "Transparency=0\n" +
                                       "Ranges=10\n" +
                                       "Range0=-1601,FFFFFF,\"Missing value: -1601\"\n" +
                                       "Range1=-437.5,800000,\"< -10 m\"\n" +
                                       "Range2=-100,FF8000,\"-10 - 1 m\"\n" +
                                       "Range3=0,FFFF80,\"-1 - 0 m\"\n" +
                                       "Range4=50,00FF00,\"0 - 0.5 m\"\n" +
                                       "Range5=100,80FF80,\"0.5 - 1 m\"\n" +
                                       "Range6=150,80FFFF,\"1 - 1.5 m\"\n" +
                                       "Range7=200,0080FF,\"1.5 - 2 m\"\n" +
                                       "Range8=300,0000FF,\"2 - 3 m\"\n" +
                                       "Range9=400,0000A0,\"3 - 4 m\"\n" +
                                       "Range10=3500,000080,\"> 4 m\"\n" +
                                       "RangesFixed=0\n" +
                                       "ColorsFixed=0";

            var theme = LegFileConverter.Convert(legFileText, "attribute");

            Assert.IsNotNull(theme);
            Assert.IsNotNull(theme.ThemeItems);
            Assert.AreEqual(10, theme.ThemeItems.Count);
            
            var quantityThemeItem = ((QuantityThemeItem)theme.ThemeItems[3]);

            Assert.AreEqual(0, quantityThemeItem.Min);
            Assert.AreEqual(50, quantityThemeItem.Max);
            Assert.AreEqual("0 - 0.5 m",quantityThemeItem.Label);
            Assert.AreEqual(Color.FromArgb(0,255,0), ((SolidBrush)((VectorStyle)quantityThemeItem.Style).Fill).Color);
        }
    }
}