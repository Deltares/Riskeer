using System;
using System.Globalization;

namespace DelftTools.Utils
{
    /// <summary>
    /// Provides culture invariant string conversion. YAGNI - just inline InvariantCulture use when required.
    /// </summary>
    public static class ConversionHelper
    {
        /// <summary>
        /// Converts a string to single with a decimal separator '.'
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float ToSingle(string s)
        {
            return Convert.ToSingle(s, CultureInfo.InvariantCulture);
        }

        public static double ToDouble(string s)
        {
            return Convert.ToDouble(s, CultureInfo.InvariantCulture);
        }
    }
}
