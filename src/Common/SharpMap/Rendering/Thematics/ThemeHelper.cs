using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    public class ThemeHelper
    {
        /// <summary>
        /// Used for caching, optimization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="theme"></param>
        /// <param name="colors"></param>
        /// <param name="values"></param>
        public static void RefreshCachedFillColorsAndValues<T>(ITheme theme, List<Color> colors, ArrayList values)
        {
            //use sorted dictionary to make sure values and colors are in the correct order
            var themeItemsDictionary = new SortedDictionary<IThemeItem, object>();
            var categorialTheme = theme as CategorialTheme;
            var gradientTheme = theme as GradientTheme;
            var quantityTheme = theme as QuantityTheme;

            if (categorialTheme != null)
            {
                foreach (CategorialThemeItem item in categorialTheme.ThemeItems)
                {
                    themeItemsDictionary.Add(item, item.Value);
                }
            }
            else if (quantityTheme != null)
            {
                if (values.Count > 0)
                {
                    colors.Add(ExtractFillColorFromThemeItem(quantityTheme.ThemeItems[0]));
                    values.Add(Convert.ChangeType(((QuantityThemeItem) quantityTheme.ThemeItems[0]).Interval.Min,
                                                  typeof (T)));
                }
                foreach (QuantityThemeItem item in quantityTheme.ThemeItems)
                {
                    themeItemsDictionary[item] = item.Interval.Max;
                }
            }
            else if (gradientTheme != null)
            {
                double currentMax = gradientTheme.Min;
                double totalWidth = gradientTheme.Max - gradientTheme.Min;
                double widthPerClass = totalWidth/values.Count;

                if (values.Count > 0)
                {
                    colors.Add(ExtractFillColorFromThemeItem(gradientTheme.ThemeItems[0]));
                    values.Add(Convert.ChangeType(gradientTheme.Min, typeof (T)));
                }
                for (int i = 0; i < values.Count; i++)
                {
                    currentMax += widthPerClass;
                    //VectorStyle style = (VectorStyle) gradientTheme.GetStyle(currentMax);

                    themeItemsDictionary.Add(gradientTheme.ThemeItems[i], Convert.ChangeType(currentMax, values[0].GetType()));
                }
            }

            foreach (IThemeItem item in themeItemsDictionary.Keys)
            {
                Color color = ExtractFillColorFromThemeItem(item);
                if (!colors.Contains(color))
                {
                    colors.Add(color);
                    values.Add(System.Convert.ChangeType(themeItemsDictionary[item], typeof (T)));
                }
            }
        }

        public static Color ExtractFillColorFromThemeItem(IThemeItem item)
        {
            Color c = Color.Empty;

            if (item == null) return c;

            VectorStyle vectorStyle;
            SolidBrush solidBrush;

            if ((vectorStyle=item.Style as VectorStyle) != null && (solidBrush = vectorStyle.Fill as SolidBrush)!=null)
            {
                c = solidBrush.Color;
            }


            return c;
        }
    }
}