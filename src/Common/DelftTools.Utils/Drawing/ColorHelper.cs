using System.Drawing;

namespace DelftTools.Utils.Drawing
{
    public static class ColorHelper
    {
        private static readonly Color[] StandardColors = new[]
                                                     {
                                                         Color.BlueViolet, Color.Crimson, Color.Blue, Color.Green,
                                                         Color.Magenta, Color.DarkOrange, Color.DodgerBlue, Color.Gold,
                                                         Color.Indigo, Color.DarkGray, Color.DarkCyan, Color.Lime
                                                     };

        public static Color GetIndexedColor(int index)
        {
            return StandardColors[index % StandardColors.Length];
        }

        public static Color GetIndexedColor(int alpha, int index)
        {
            return Color.FromArgb(alpha, StandardColors[index % StandardColors.Length]);
        }

        /// <summary>
        /// Used only scripting until System.Drawing can be imported into a script
        /// </summary>
        public static Color Transparent { get { return Color.Transparent; } }
    }
}