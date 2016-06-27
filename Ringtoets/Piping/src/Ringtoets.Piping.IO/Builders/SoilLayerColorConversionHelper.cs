using System;
using System.Drawing;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class provides helpers for converting double values from the DSoilModel database into
    /// <see cref="Color"/>.
    /// </summary>
    public static class SoilLayerColorConversionHelper
    {
        /// <summary>
        /// Converts a nullable <see cref="double"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="colorValue">The value to convert.</param>
        /// <returns>A <see cref="Color"/> instance based on the <paramref name="colorValue"/>.</returns>
        public static Color ColorFromNullableDouble(double? colorValue)
        {
            if (colorValue == null)
            {
                return Color.Empty;
            }
            return Color.FromArgb(Convert.ToInt32(colorValue));
        }
    }
}