using System.Drawing;
using System.Text.RegularExpressions;
using DelftTools.Utils.RegularExpressions;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis
{
    public static class LegFileConverter
    {
        public static QuantityTheme Convert(string legFileText, string attributeName)
        {
            const string valueGroup = "ValueGroup";
            const string colorGroup = "ColorGroup";
            const string descrGroup = "DescrGroup";
            const string regExpRange = "Range[0-9-]{1,2}=" +
                                       @"(?<" + valueGroup + @">[0-9.-]*)," +
                                       "(?<" + colorGroup + ">[0-9a-fA-F-]*),\"" +
                                       "(?<" + descrGroup + ">" + RegularExpression.Characters + ")\"";

            var generatedTheme = new QuantityTheme(attributeName, new VectorStyle());
            var regex = new Regex(regExpRange, RegexOptions.Singleline);

            var matches = regex.Matches(legFileText);

            for (int i = 1; i < matches.Count; i++)
            {
                var match = matches[i];
                var previousMatch = matches[i - 1];

                var color = GetColor(match.Groups[colorGroup].Captures[0].Value);
                var value = match.Groups[valueGroup].Captures[0].Value;
                var label = match.Groups[descrGroup].Captures[0].Value;

                var previousValue = (previousMatch.Groups[valueGroup].Captures[0].Value);

                var quantityThemeItem = new QuantityThemeItem(new Interval
                {
                    Min = System.Convert.ToDouble(previousValue),
                    Max = System.Convert.ToDouble(value)
                }
                                                              , new VectorStyle
                                                              {
                                                                  Line = new Pen(color),
                                                                  Fill = new SolidBrush(color)
                                                              })
                {
                    Label = label
                };

                ((VectorStyle) quantityThemeItem.Style).GeometryType = typeof(IPolygon);
                generatedTheme.ThemeItems.Add(quantityThemeItem);
            }

            return generatedTheme;
        }

        private static Color GetColor(string colorValue)
        {
            if (colorValue.Length < 6)
            {
                colorValue = colorValue.PadLeft(6, '0');
            }

            return Color.FromArgb(255,
                                  HexToInt(colorValue.Substring(4, 2)),
                                  HexToInt(colorValue.Substring(2, 2)),
                                  HexToInt(colorValue.Substring(0, 2)));
        }

        private static int HexToInt(string hexValue)
        {
            return System.Convert.ToInt32(hexValue, 16);
        }
    }
}