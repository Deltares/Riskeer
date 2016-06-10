using System;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Class that contains extension methods for <see cref="decimal"/> to convert them to
    /// other value types.
    /// </summary>
    public static class DecimalConversionExtensions
    {
        public static double ToNanableDouble(this decimal? value)
        {
            if (value.HasValue)
            {
                return Convert.ToDouble(value);
            }
            return double.NaN;
        }
    }
}