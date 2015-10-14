using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using GeoAPI.Geometries;
using SharpMap.Api;
using SharpMap.Api.Enums;
using SharpMap.Styles;
using SharpVectors.Dom.Css;

namespace DeltaShell.Plugins.SharpMapGis.HibernateMappings
{
    /// <summary>
    /// Converter to serialize an <see cref="SharpMap.Api.IStyle"/> object into a <see cref="CssStyleDeclaration"/> object or CSS declaration string
    /// </summary>
    public class StyleTypeConverter : TypeConverter
    {
        private const string endcapName = "line-endcap";

        /// <summary>
        /// Converts a CSS-like string to a <see cref="SharpMap.Api.IStyle"/> object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            CssStyleDeclaration csd = null;
            // Try to parse the object as string or CssStyleDeclaration
            if (value is string)
            {
                // Parse the string as css style declaration
                csd = new CssStyleDeclaration((string) value, null, false, CssStyleSheetType.Author);
            }
            else if (value is CssStyleDeclaration)
            {
                csd = (CssStyleDeclaration) value;
            }

            if (csd != null)
            {
                // Copy properties to a new Style object
                IStyle style;

                // See what kind of style this is
                string styleType = csd.GetPropertyValue("display-style");
                if (styleType == "label")
                {
                    style = GetLabelStyle(csd);
                }
                else
                {
                    style = GetVectorStyle(csd);
                }

                // Generic style properties assignment
                GetGeneralProperties(csd, style);

                return style;
            }
            // Use the base converter if the target type is unsupported
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a <see cref="SharpMap.Api.IStyle"/> object to a CSS-like string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is IStyle && (destinationType.Equals(typeof(CssStyleDeclaration)) || destinationType.Equals(typeof(string))))
            {
                // Get the IStyle to convert from
                IStyle from = (IStyle) value;
                CssStyleDeclaration csd = new CssStyleDeclaration(string.Empty, null, false, CssStyleSheetType.Author);

                // Copy IStyle/Style properties to the CSS declaration
                SetGeneralProperties(from, csd);

                if (from is VectorStyle)
                {
                    SetVectorStyleProperties(from as VectorStyle, csd);
                }
                else if (from is LabelStyle)
                {
                    SetLabelStyleProperties(from as LabelStyle, csd);
                }

                // Return as CssStyleDeclaration
                if (destinationType.Equals(typeof(CssStyleDeclaration)))
                {
                    return csd;
                }

                // Else return as string
                return csd.CssText;
            }
            // Use the base converter if the value was of an unsupported type
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Can convert from string or CssDeclarationObject
            return sourceType.Equals(typeof(CssStyleDeclaration)) || sourceType.Equals(typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            // Can convert from string or CssDeclarationObject
            return destinationType.Equals(typeof(CssStyleDeclaration)) || destinationType.Equals(typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        private static void GetGeneralProperties(ICssStyleDeclaration csd, IStyle style)
        {
            if (csd.GetPropertyValue("zoom-min-visible") != string.Empty)
            {
                style.MinVisible = double.Parse(csd.GetPropertyValue("zoom-min-visible"));
            }
            if (csd.GetPropertyValue("zoom-max-visible") != string.Empty)
            {
                style.MaxVisible = double.Parse(csd.GetPropertyValue("zoom-max-visible"));
            }
        }

        private static IStyle GetVectorStyle(ICssStyleDeclaration csd)
        {
            // VectorStyle object rebuild. Deserializes:
            //   border-color     Line.Color
            //   border-width     Line.Width
            //   outline-color    Outline.Color
            //   outline-width    Outline.Width
            //   outline-style    EnableOutline
            //   background-color Fill
            VectorStyle vStyle = new VectorStyle();
            if (csd.GetPropertyValue("border-color") != string.Empty)
            {
                vStyle.Line.Color = GetColorFromCss(csd, "border-color");
            }
            if (csd.GetPropertyValue("border-width") != string.Empty)
            {
                vStyle.Line.Width = float.Parse(csd.GetPropertyValue("border-width"));
            }
            if (csd.GetPropertyValue("outline-color") != string.Empty)
            {
                vStyle.Outline.Color = GetColorFromCss(csd, "outline-color");
            }
            if (csd.GetPropertyValue("outline-width") != string.Empty)
            {
                vStyle.Outline.Width = float.Parse(csd.GetPropertyValue("outline-width"));
            }
            if (csd.GetPropertyValue("outline-style") != string.Empty)
            {
                vStyle.EnableOutline = Convert.ToBoolean(csd.GetPropertyValue("outline-style"));
            }
            if (csd.GetPropertyValue("background-color") != string.Empty)
            {
                vStyle.Fill = new SolidBrush(GetColorFromCss(csd, "background-color"));
            }
            if (csd.GetPropertyValue("line-dash") != string.Empty)
            {
                vStyle.Line.DashStyle = (DashStyle) Enum.Parse(typeof(DashStyle), csd.GetPropertyValue("line-dash"));
            }

            if (csd.GetPropertyValue("outline-dash") != string.Empty)
            {
                vStyle.Outline.DashStyle = (DashStyle) Enum.Parse(typeof(DashStyle), csd.GetPropertyValue("outline-dash"));
            }

            if (csd.GetPropertyValue(endcapName) != string.Empty)
            {
                vStyle.Line.EndCap = (LineCap) Enum.Parse(typeof(LineCap), csd.GetPropertyValue(endcapName));
            }

            var customEndCap = csd.GetPropertyValue("custom-line-end-cap");
            if (customEndCap != null && customEndCap.StartsWith("AdjustableArrowCap"))
            {
                var values = customEndCap.Split(',');
                if (values.Length == 5)
                {
                    vStyle.Line.CustomEndCap = new AdjustableArrowCap(Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToBoolean(values[3]))
                    {
                        BaseCap = (LineCap) Enum.Parse(typeof(LineCap), values[4])
                    };
                }
            }

            var customStartCap = csd.GetPropertyValue("custom-line-start-cap");
            if (customStartCap != null && customStartCap.StartsWith("AdjustableArrowCap"))
            {
                var values = customStartCap.Split(',');
                if (values.Length == 5)
                {
                    vStyle.Line.CustomStartCap = new AdjustableArrowCap(Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToBoolean(values[3]))
                    {
                        BaseCap = (LineCap) Enum.Parse(typeof(LineCap), values[4])
                    };
                }
            }

            if (csd.GetPropertyValue("geometry-type") != string.Empty)
            {
                vStyle.GeometryType = GetGeometryTypeFromCssString(csd);
            }

            if (csd.GetPropertyValue("symbol-shape") != string.Empty)
            {
                vStyle.Shape = (ShapeType) Enum.Parse(typeof(ShapeType), csd.GetPropertyValue("symbol-shape"));
            }
            if (csd.GetPropertyValue("symbol") != string.Empty)
            {
                // From a Codepage 1251-encoded string, convert to bytes and next to a Bitmap representing the symbol
                byte[] bytes = Convert.FromBase64String(csd.GetPropertyValue("symbol"));
                vStyle.Symbol = (Bitmap) TypeDescriptor.GetConverter(typeof(Bitmap)).ConvertFrom(bytes);
            }
            return vStyle;
        }

        private IStyle GetLabelStyle(ICssStyleDeclaration csd)
        {
            //   LabelStyle object rebuild. Deserializes:
            //   font-family        Font.Family
            //   font-size          Font.Size
            //   font-color         ForeColor
            //   background-color   BackColor
            //   border-color       Halo.Color
            //   border-width       Halo.Width
            //   padding-horizontal Offset.X
            //   padding-vertical   Offset.Y
            //   text-align         HorizontalAlignment
            //   vertical-align     VerticalAlignment
            LabelStyle lStyle = new LabelStyle();
            string fontFamily = lStyle.Font.FontFamily.Name;
            float fontSize = lStyle.Font.Size;
            if (csd.GetPropertyValue("font-family") != string.Empty)
            {
                fontFamily = csd.GetPropertyValue("font-family");
            }
            if (csd.GetPropertyValue("font-size") != string.Empty)
            {
                fontSize = float.Parse(csd.GetPropertyValue("font-size"));
            }
            lStyle.Font = new Font(fontFamily, fontSize);
            if (csd.GetPropertyValue("font-color") != string.Empty)
            {
                lStyle.ForeColor = GetColorFromCss(csd, "font-color");
            }
            if (csd.GetPropertyValue("background-color") != string.Empty)
            {
                lStyle.BackColor = new SolidBrush(GetColorFromCss(csd, "background-color"));
            }

            if (csd.GetPropertyValue("collision-detection") != string.Empty)
            {
                bool collision;
                bool.TryParse(csd.GetPropertyValue("collision-detection"), out collision);
                lStyle.CollisionDetection = collision;
            }

            Color haloColor = Color.Empty;
            float haloWidth = 1.0f;

            if (lStyle.Halo != null)
            {
                haloColor = lStyle.Halo.Color;
                haloWidth = lStyle.Halo.Width;
            }

            if (csd.GetPropertyValue("border-color") != string.Empty)
            {
                haloColor = GetColorFromCss(csd, "border-color");
            }
            if (csd.GetPropertyValue("border-width") != string.Empty)
            {
                haloWidth = float.Parse(csd.GetPropertyValue("border-width"));
            }
            lStyle.Halo = new Pen(haloColor, haloWidth);

            float offsetX = lStyle.Offset.X;
            float offsetY = lStyle.Offset.Y;
            if (csd.GetPropertyValue("padding-horizontal") != string.Empty)
            {
                offsetX = float.Parse(csd.GetPropertyValue("padding-horizontal"));
            }
            if (csd.GetPropertyValue("padding-vertical") != string.Empty)
            {
                offsetY = float.Parse(csd.GetPropertyValue("padding-vertical"));
            }
            lStyle.Offset = new PointF(offsetX, offsetY);
            if (csd.GetPropertyValue("text-align") != string.Empty)
            {
                lStyle.HorizontalAlignment = (HorizontalAlignmentEnum) Enum.Parse(typeof(HorizontalAlignmentEnum),
                                                                                  csd.GetPropertyValue("text-align"));
            }
            if (csd.GetPropertyValue("vertical-align") != string.Empty)
            {
                lStyle.VerticalAlignment = (VerticalAlignmentEnum) Enum.Parse(typeof(VerticalAlignmentEnum),
                                                                              csd.GetPropertyValue("vertical-align"));
            }
            return lStyle;
        }

        private static void SetLabelStyleProperties(LabelStyle labelStyle, ICssStyleDeclaration csd)
        {
            // Copy LabelStyle properties to the CSS declaration. Serializes:
            //   font-family        Font.Family
            //   font-size          Font.Size
            //   font-color         ForeColor
            //   background-color   BackColor
            //   border-color       Halo.Color
            //   border-width       Halo.Width
            //   padding-horizontal Offset.X
            //   padding-vertical   Offset.Y
            //   text-align         HorizontalAlignment
            //   vertical-align     VerticalAlignment
            LabelStyle labelDefaults = new LabelStyle();

            csd.SetProperty("display-style", "label", String.Empty);

            if (labelStyle.Font.FontFamily != labelDefaults.Font.FontFamily)
            {
                csd.SetProperty("font-family", labelStyle.Font.FontFamily.Name, string.Empty);
            }
            if (labelStyle.Font.Size != labelDefaults.Font.Size)
            {
                csd.SetProperty("font-size", labelStyle.Font.Size.ToString("F0"), string.Empty);
            }
            SetColorStyleProperty(csd, "font-color", labelStyle.ForeColor, labelDefaults.ForeColor);
            SetBrushStyleProperty(csd, "background-color", labelStyle.BackColor, labelDefaults.BackColor);

            csd.SetProperty("collision-detection", labelStyle.CollisionDetection.ToString(), string.Empty);

            Color haloValueColor = Color.Empty;
            Color haloOriginalColor = Color.Empty;
            if (labelStyle.Halo != null)
            {
                haloValueColor = labelStyle.Halo.Color;
            }
            if (labelDefaults.Halo != null)
            {
                haloOriginalColor = labelDefaults.Halo.Color;
            }
            SetColorStyleProperty(csd, "border-color", haloValueColor, haloOriginalColor);

            if (labelStyle.Halo != null
                && (labelDefaults.Halo == null
                    || (labelStyle.Halo.Width != labelDefaults.Halo.Width)))
            {
                csd.SetProperty("border-width", labelStyle.Halo.Width.ToString("F0"), string.Empty);
            }

            if (labelStyle.Offset.X != labelDefaults.Offset.X)
            {
                csd.SetProperty("padding-horizontal", labelStyle.Offset.X.ToString("F0"), string.Empty);
            }
            if (labelStyle.Offset.Y != labelDefaults.Offset.Y)
            {
                csd.SetProperty("padding-vertical", labelStyle.Offset.Y.ToString("F0"), string.Empty);
            }
            if (labelStyle.HorizontalAlignment != labelDefaults.HorizontalAlignment)
            {
                csd.SetProperty("text-align", labelStyle.HorizontalAlignment.ToString(), string.Empty);
            }
            if (labelStyle.VerticalAlignment != labelDefaults.VerticalAlignment)
            {
                csd.SetProperty("vertical-align", labelStyle.VerticalAlignment.ToString(), string.Empty);
            }
        }

        private static void SetVectorStyleProperties(VectorStyle vectorStyle, ICssStyleDeclaration csd)
        {
            // Copy VectorStyle properties to the CSS declaration. Serializes:
            //   border-color     Line.Color
            //   border-width     Line.Width
            //   outline-color    Outline.Color
            //   outline-width    Outline.Width
            //   outline-style    EnableOutline
            //   background-color Fill
            //   endCap           Line.EndCap
            VectorStyle vectorDefaults = new VectorStyle();
            if (vectorStyle != null)
            {
                csd.SetProperty("display-style", "vector", String.Empty);

                SetBrushStyleProperty(csd, "background-color", vectorStyle.Fill, vectorDefaults.Fill);
                SetColorStyleProperty(csd, "border-color", vectorStyle.Line.Color, vectorDefaults.Line.Color);
                SetColorStyleProperty(csd, "outline-color", vectorStyle.Outline.Color, vectorDefaults.Outline.Color);

                if (vectorStyle.Line.Width != vectorDefaults.Line.Width)
                {
                    csd.SetProperty("border-width", vectorStyle.Line.Width.ToString("F0"), string.Empty);
                }

                if (vectorStyle.Outline.Width != vectorDefaults.Outline.Width)
                {
                    csd.SetProperty("outline-width", vectorStyle.Outline.Width.ToString("F0"), string.Empty);
                }

                if (vectorStyle.Line.DashStyle != vectorDefaults.Line.DashStyle)
                {
                    csd.SetProperty("line-dash", vectorStyle.Line.DashStyle.ToString(), string.Empty);
                }

                if (vectorStyle.Outline.Width != vectorDefaults.Outline.Width)
                {
                    csd.SetProperty("outline-dash", vectorStyle.Outline.DashStyle.ToString(), string.Empty);
                }

                csd.SetProperty("outline-style", vectorStyle.EnableOutline.ToString(), string.Empty);

                csd.SetProperty(endcapName, vectorStyle.Line.EndCap.ToString(), string.Empty);

                var arrowEndCap = vectorStyle.Line.EndCap != LineCap.Custom ? null : vectorStyle.Line.CustomEndCap as AdjustableArrowCap;
                if (arrowEndCap != null)
                {
                    var customEndCapString = "AdjustableArrowCap," + arrowEndCap.Width + "," + arrowEndCap.Height + "," + arrowEndCap.Filled + "," + arrowEndCap.BaseCap;
                    csd.SetProperty("custom-line-end-cap", customEndCapString, string.Empty);
                }

                var arrowStartCap = vectorStyle.Line.StartCap != LineCap.Custom ? null : vectorStyle.Line.CustomStartCap as AdjustableArrowCap;
                if (arrowStartCap != null)
                {
                    var customEndCapString = "AdjustableArrowCap," + arrowStartCap.Width + "," + arrowStartCap.Height + "," + arrowStartCap.Filled + "," + arrowStartCap.BaseCap;
                    csd.SetProperty("custom-line-start-cap", customEndCapString, string.Empty);
                }

                if (vectorStyle.GeometryType != null)
                {
                    csd.SetProperty("geometry-type", GeometryType2CssString(vectorStyle), string.Empty);
                }

                // vectorStyle.Shape has got a default value and cannot be null (no check required)
                csd.SetProperty("symbol-shape", vectorStyle.Shape.ToString(), string.Empty);
                

                if ((vectorStyle.Symbol != null) && (vectorStyle.HasCustomSymbol))
                {
                    // Encode a Bitmap symbol as bytes that can be included in the css as string using Codepage 1251 encoding
                    byte[] bytes = (byte[]) TypeDescriptor.GetConverter(typeof(Bitmap)).ConvertTo(vectorStyle.Symbol, typeof(byte[]));
                    csd.SetProperty("symbol", Convert.ToBase64String(bytes), string.Empty);
                }
            }
        }

        private static string GeometryType2CssString(VectorStyle vectorStyle)
        {
            if (vectorStyle.GeometryType == typeof(ILineString))
            {
                return "LineString";
            }
            if (vectorStyle.GeometryType == typeof(IPoint))
            {
                return "Point";
            }
            if (vectorStyle.GeometryType == typeof(IPolygon))
            {
                return "Polygon";
            }
            return "LineString";
        }

        private static Type GetGeometryTypeFromCssString(ICssStyleDeclaration csd)
        {
            string property = csd.GetPropertyValue("geometry-type");
            switch (property) {
                case "LineString":
                    return typeof(ILineString);
                case "Polygon":
                    return typeof(IPolygon);
                case "Point":
                    return typeof(IPoint);
                default:
                    return new VectorStyle().GeometryType;
            }
            
        }

        /// <summary>
        /// Sets general properties of the style in the csd.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="csd"></param>
        private static void SetGeneralProperties(IStyle from, ICssStyleDeclaration csd)
        {
            if (from.MinVisible > 0)
            {
                csd.SetProperty("zoom-min-visible", from.MinVisible.ToString(), string.Empty);
            }
            if (from.MaxVisible < double.MaxValue)
            {
                csd.SetProperty("zoom-max-visible", from.MaxVisible.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Sets the CSS property of a brush if it was different from the original
        /// </summary>
        /// <param name="csd">The style to add the CSS property to</param>
        /// <param name="property">The property name to use</param>
        /// <param name="value">The set brush value of the object to serialize</param>
        /// <param name="original">The original non-changed object to compare changes to</param>
        private static void SetBrushStyleProperty(ICssStyleDeclaration csd, string property, Brush value, Brush original)
        {
            Color valueColor = Color.Empty;
            Color originalColor = Color.Empty;

            if (value != null)
            {
                valueColor = ((SolidBrush) value).Color;
            }

            if (original != null)
            {
                originalColor = ((SolidBrush) original).Color;
            }

            // Compare brushes based on the color values and output the brush' color value as HTML color
            SetColorStyleProperty(csd, property, valueColor, originalColor);
        }

        /// <summary>
        /// Sets the CSS property of a color if it was different from the original
        /// </summary>
        /// <param name="csd">The style to add the CSS property to</param>
        /// <param name="property">The property name to use</param>
        /// <param name="value">The set color value of the object to serialize</param>
        /// <param name="original">The original non-changed object to compare changes to</param>
        private static void SetColorStyleProperty(ICssStyleDeclaration csd, string property, Color value, Color original)
        {
            // Compare colors on their ARGB values and set it as an HTML color (hex or well-known)
            if (value.ToArgb() != original.ToArgb())
            {
                SetColorToCss(value, csd, property);
            }
        }

        private static Color GetColorFromCss(ICssStyleDeclaration csd, string propertyName)
        {
            string property = csd.GetPropertyValue(propertyName);
            string[] components = property.Split(new string[]
            {
                " "
            }, StringSplitOptions.RemoveEmptyEntries);

            var color = components[0].Contains("#") ? Color.FromArgb(int.Parse(components[1])) : Color.FromName(components[0]);

            return color;
        }

        /// <summary>
        /// Stores the color in a string using the name (for support known colors) and the 
        /// ARGB to support the alpha channel which is not supported by ColorTranslator.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="csd"></param>
        /// <param name="propertyName"></param>
        private static void SetColorToCss(Color color, ICssStyleDeclaration csd, string propertyName)
        {
            string colorString = string.Format("{0} {1}", color.IsKnownColor ? color.Name : "#", color.ToArgb());
            csd.SetProperty(propertyName, colorString, string.Empty);
        }
    }
}