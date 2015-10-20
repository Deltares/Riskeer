using System.Drawing;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    public static class ThemeHelper
    {
        public static Color ExtractFillColorFromThemeItem(IThemeItem item)
        {
            Color c = Color.Empty;

            if (item == null)
            {
                return c;
            }

            VectorStyle vectorStyle;
            SolidBrush solidBrush;

            if ((vectorStyle = item.Style as VectorStyle) != null && (solidBrush = vectorStyle.Fill as SolidBrush) != null)
            {
                c = solidBrush.Color;
            }

            return c;
        }
    }
}