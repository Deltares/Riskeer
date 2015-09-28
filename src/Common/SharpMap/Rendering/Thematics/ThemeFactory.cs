using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    public abstract class ThemeFactory
    {
        /// <summary>
        /// Creates a <see cref="GradientTheme"/>
        /// </summary>
        /// <param name="attribute">Name of the feature attribute</param>
        /// <param name="defaultStyle">Default <see cref="VectorStyle"/> to base this theme on</param>
        /// <param name="blend"><see cref="ColorBlend"/> 
        ///   defining the min and max colors
        ///   Note: Silently assumes 2 Colors defined
        /// </param>
        /// <param name="minValue">Minimum value of the feature attribute values</param>
        /// <param name="maxValue">Maximum value of the feature attribute values</param>
        /// <param name="sizeMin">Minimum line/point size in pixels</param>
        /// <param name="sizeMax">Maximum line/point size in pixels</param>
        /// <param name="skipColors">Use the min and max colors (false) or the defaultStyle fill color (true)</param>
        /// <param name="skipSizes">Let the size of a point depend on the value (false) or use the defaultStyle size (true)</param>
        /// <param name="numberOfClasses">The number of classes (ThemeItems) to generate (default = 8)</param>
        /// <returns>A new <see cref="GradientTheme"/></returns>
        public static GradientTheme CreateGradientTheme(string attribute, VectorStyle defaultStyle, ColorBlend blend,
            double minValue, double maxValue, int sizeMin, int sizeMax, bool skipColors, bool skipSizes, int numberOfClasses = 8)
        {
            if (defaultStyle == null)
            {
                defaultStyle = new VectorStyle { GeometryType = typeof(IPolygon) };
            }

            Color minColor = (skipColors) ? ((SolidBrush)defaultStyle.Fill).Color : blend.GetColor(0);
            Color maxColor = (skipColors) ? ((SolidBrush)defaultStyle.Fill).Color : blend.GetColor(1);

            var deltaWidth = (defaultStyle.Outline.Width - defaultStyle.Line.Width);

            float minOutlineSize = deltaWidth + sizeMin;
            float maxOutlineSize = deltaWidth + sizeMax;

            // Use default styles if not working with VectorLayers (i.e. RegularGridCoverageLayers)
            var minStyle = (VectorStyle)defaultStyle.Clone();
            var maxStyle = (VectorStyle)defaultStyle.Clone();

            minStyle.GeometryType = defaultStyle.GeometryType;
            maxStyle.GeometryType = defaultStyle.GeometryType;

            if (defaultStyle.GeometryType == typeof(IPoint))
            {
                UpdateMinMaxForPoints(defaultStyle, sizeMin, sizeMax, minStyle, maxStyle, minColor, maxColor, skipSizes);
            }
            else if ((defaultStyle.GeometryType == typeof(IPolygon)) || (defaultStyle.GeometryType == typeof(IMultiPolygon)))
            {
                UpdateMinMaxForPolygons(defaultStyle, minStyle, maxStyle, minColor, maxColor, minOutlineSize, maxOutlineSize);
            }
            else if ((defaultStyle.GeometryType == typeof(ILineString)) || (defaultStyle.GeometryType == typeof(IMultiLineString)))
            {
                UpdateMinMaxForLineStrings(defaultStyle, sizeMin, sizeMax, minStyle, maxStyle, minColor, maxColor, minOutlineSize, maxOutlineSize, skipSizes);
            }
            else
            {
                //use for unknown geometry..
                minStyle.Fill = new SolidBrush(minColor);
                maxStyle.Fill = new SolidBrush(maxColor);
                minStyle.Outline = CreatePen(minColor, minOutlineSize, defaultStyle.Outline);
                maxStyle.Outline = CreatePen(maxColor, maxOutlineSize, defaultStyle.Outline);
            }

            return new GradientTheme(attribute, minValue, maxValue, minStyle, maxStyle, blend, blend, null, numberOfClasses);
        }

        private static void UpdateMinMaxForLineStrings(VectorStyle defaultStyle, int sizeMin, int sizeMax, VectorStyle minStyle, VectorStyle maxStyle, Color minColor, Color maxColor, float minOutlineSize, float maxOutlineSize, bool skipSizes)
        {
            minStyle.Line = CreatePen(minColor, skipSizes ? 4 : sizeMin, defaultStyle.Line);
            maxStyle.Line = CreatePen(maxColor, skipSizes ? 12 : sizeMax, defaultStyle.Line);
            minStyle.Outline = CreatePen(defaultStyle.Outline.Color, minOutlineSize, defaultStyle.Outline);
            maxStyle.Outline = CreatePen(defaultStyle.Outline.Color, maxOutlineSize, defaultStyle.Outline);
        }

        private static void UpdateMinMaxForPolygons(VectorStyle defaultStyle, VectorStyle minStyle, VectorStyle maxStyle, Color minColor, Color maxColor, float minOutlineSize, float maxOutlineSize)
        {
            minStyle.Fill = new SolidBrush(minColor);
            maxStyle.Fill = new SolidBrush(maxColor);
            minStyle.Outline = CreatePen(defaultStyle.Outline.Color, minOutlineSize, defaultStyle.Outline);
            maxStyle.Outline = CreatePen(defaultStyle.Outline.Color, maxOutlineSize, defaultStyle.Outline);
        }

        private static void UpdateMinMaxForPoints(VectorStyle defaultStyle, int sizeMin, int sizeMax, VectorStyle minStyle, VectorStyle maxStyle, Color minColor, Color maxColor, bool skipSizes)
        {
            minStyle.Fill = new SolidBrush(minColor);
            maxStyle.Fill = new SolidBrush(maxColor);
            minStyle.Shape = defaultStyle.Shape;
            maxStyle.Shape = defaultStyle.Shape;
            if (!skipSizes)
            {
                minStyle.Line.Width = sizeMin;
                maxStyle.Line.Width = sizeMax;
                minStyle.ShapeSize = sizeMin;
                maxStyle.ShapeSize = sizeMax;
            }
        }

        public static CategorialTheme CreateCategorialTheme(string attribute, VectorStyle defaultStyle, ColorBlend blend, 
            int numberOfClasses, IList<IComparable> values, List<string> categories, int sizeMin, int sizeMax)
        {
            if (defaultStyle == null)
            {
                defaultStyle = new VectorStyle
                                   {
                                       GeometryType = typeof (IPolygon)
                                   };
            }

            var categorialTheme = new CategorialTheme(attribute, defaultStyle);

            for (int i = 0; i < numberOfClasses; i++)
            {
                string label = (categories != null)
                                   ? categories[i]
                                   : values[i].ToString();

                Color color = (numberOfClasses > 1)
                                  ? blend.GetColor((float) i/(numberOfClasses - 1))
                                  : ((SolidBrush) defaultStyle.Fill).Color;
                
                var vectorStyle = (VectorStyle) defaultStyle.Clone();

                var size = sizeMin + (sizeMax - sizeMin)*i/(float) numberOfClasses;

                if (defaultStyle.GeometryType == typeof(IPoint))
                {
                    vectorStyle.Fill = new SolidBrush(color);
                    vectorStyle.Line.Width = 16;
                    vectorStyle.Shape = defaultStyle.Shape;
                }
                else if ((defaultStyle.GeometryType == typeof(IPolygon)) || (defaultStyle.GeometryType == typeof(IMultiPolygon)))
                {
                    vectorStyle.Fill = new SolidBrush(color);
                }
                else if ((defaultStyle.GeometryType == typeof(ILineString)) || (defaultStyle.GeometryType == typeof(IMultiLineString)))
                {
                    vectorStyle.Line = CreatePen(color, size, defaultStyle.Line);
                }
                else
                {
                    vectorStyle.Fill = new SolidBrush(color);
                }

                CategorialThemeItem categorialThemeItem = (values[i] != null)
                                                              ? new CategorialThemeItem(label, vectorStyle, vectorStyle.LegendSymbol, values[i])
                                                              : new CategorialThemeItem(label, vectorStyle, vectorStyle.LegendSymbol);

                
                categorialTheme.AddThemeItem(categorialThemeItem);
            }

            return categorialTheme;
        }

        public static QuantityTheme CreateQuantityTheme(string attribute, VectorStyle defaultStyle, ColorBlend blend, 
            int numberOfClasses, IList<Interval> intervals)
        {
            float minSize = defaultStyle.Line.Width;
            float maxSize = defaultStyle.Line.Width;

            return CreateQuantityTheme(attribute, defaultStyle, blend, numberOfClasses, intervals, minSize, maxSize, false, false);
        }

        public static QuantityTheme CreateQuantityTheme(string attribute, VectorStyle defaultStyle, ColorBlend blend,
            int numberOfClasses, IList<IComparable> values, float minSize, float maxSize, bool skipColors, bool skipSizes, QuantityThemeIntervalType intervalType)
        {
            var intervals = ThemeFactoryHelper.GetIntervalsForNumberOfClasses(values.Select(i=>Convert.ToSingle(i)).ToList(),
                                                                              intervalType, numberOfClasses);
            return CreateQuantityTheme(attribute, defaultStyle, blend, numberOfClasses, intervals,minSize,maxSize,skipColors,skipSizes);
        }
        
        public static QuantityTheme CreateQuantityTheme(string attribute, VectorStyle defaultStyle, ColorBlend blend, 
            int numberOfClasses, IList<Interval> intervals, float minSize, float maxSize, bool skipColors, bool skipSizes)
        {
            if (defaultStyle == null)
            {
                defaultStyle = new VectorStyle();
                defaultStyle.GeometryType = typeof(IPolygon);
            }

            var quantityTheme = new QuantityTheme(attribute, defaultStyle);
            
            var totalMinValue = (float) intervals[0].Min;
            var totalMaxValue = (float) intervals[intervals.Count - 1].Max;
            
            if (totalMinValue == totalMaxValue)
            {
                return null;
            }

            for (int i = 0; i < numberOfClasses; i++)
            {
                Color color = numberOfClasses > 1
                                  ? blend.GetColor(1 - (float) i/(numberOfClasses - 1))
                                  : ((SolidBrush) defaultStyle.Fill).Color;

                float size = defaultStyle.Line.Width;

                if (!skipSizes)
                {
                    var minValue = (float) intervals[i].Min;
                    var maxValue = (float) intervals[i].Max;
                    
                    float width = maxValue - minValue;
                    float mean = minValue + 0.5f * width;

                    float fraction = (mean - totalMinValue) / (totalMaxValue - totalMinValue);

                    size = minSize + fraction * (maxSize - minSize);
                }

                var vectorStyle = new VectorStyle
                                      {
                                          GeometryType = defaultStyle.GeometryType
                                      };

                if (defaultStyle.GeometryType == typeof(IPoint))
                {
                    if (skipColors)
                    {
                        color = ((SolidBrush)defaultStyle.Fill).Color;
                    }

                    vectorStyle.Fill = new SolidBrush(color);
                    vectorStyle.Shape = defaultStyle.Shape;

                    if (!skipSizes)
                    {
                        vectorStyle.ShapeSize = Convert.ToInt32(size);
                        vectorStyle.Line.Width = size;
                    }

                }
                else if ((defaultStyle.GeometryType == typeof(IPolygon)) || (defaultStyle.GeometryType == typeof(IMultiPolygon)))
                {
                    if (skipColors)
                    {
                        color = ((SolidBrush)defaultStyle.Fill).Color;
                    }
                    vectorStyle.Fill = new SolidBrush(color);
                    vectorStyle.Line = CreatePen(color, size, defaultStyle.Line);
                    vectorStyle.Outline.Width = (defaultStyle.Outline.Width - defaultStyle.Line.Width) + size;
                }
                else if ((defaultStyle.GeometryType == typeof(ILineString)) || (defaultStyle.GeometryType == typeof(IMultiLineString)))
                {
                    if (skipColors)
                    {
                        color = defaultStyle.Line.Color;
                    }
                    vectorStyle.Line = CreatePen(color, size, defaultStyle.Line);
                    vectorStyle.Outline.Width = (defaultStyle.Outline.Width - defaultStyle.Line.Width) + size;
                }
                else
                {
                    vectorStyle.Fill = new SolidBrush(color);
                }
              
                quantityTheme.AddStyle(vectorStyle, intervals[i]);
            }

            return quantityTheme;
        }

        public static CustomTheme CreateSingleFeatureTheme(Type geometryType, Color color, float width)
        {
            var vectorStyle = new VectorStyle {GeometryType = geometryType};

            if (geometryType == typeof(IPoint))
            {
                vectorStyle.Fill = new SolidBrush(color); // also used for updating symbol
                vectorStyle.Line = CreatePen(color, width, vectorStyle.Line);
            }
            else if ((geometryType == typeof(IPolygon)) || (geometryType == typeof(IMultiPolygon)))
            {
                vectorStyle.Fill = new SolidBrush(color);
            }
            else if ((geometryType == typeof(ILineString)) || (geometryType == typeof(IMultiLineString)))
            {
                vectorStyle.Line = CreatePen(color, width, vectorStyle.Line);
            }
            else
            {
                vectorStyle.Fill = new SolidBrush(color);
            }
            vectorStyle.Shape = ShapeType.Diamond;

            return new CustomTheme(null) { DefaultStyle = vectorStyle }; 
        }

        public static Pen CreatePen(Color color, Pen lineStyle)
        {
            return CreatePen(color, lineStyle.Width, lineStyle);
        }

        public static Pen CreatePen(float width, Pen lineStyle)
        {
            return CreatePen(lineStyle.Color, width, lineStyle);
        }

        private static Pen CreatePen(Color color, float width, Pen lineStyle)
        {
            var newPen = new Pen(color, width)
            {
                StartCap = lineStyle.StartCap,
                EndCap = lineStyle.EndCap,
                DashStyle = lineStyle.DashStyle
            };

            if (lineStyle.StartCap == LineCap.Custom)
            {
                newPen.CustomStartCap = lineStyle.CustomStartCap;
            }

            if (lineStyle.EndCap == LineCap.Custom)
            {
                newPen.CustomEndCap = lineStyle.CustomEndCap;
            }

            return newPen;
        }
    }
}
