using System;

using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="PipingOutput"/> related to creating an <see cref="PipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class PipingOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingCalculationOutputEntity"/> based on the information
        /// of the <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for piping failure mechanism to 
        /// create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="PipingCalculationOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static PipingCalculationOutputEntity Create(this PipingOutput output, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new PipingCalculationOutputEntity
            {
                HeaveFactorOfSafety = GetNullableDecimal(output.HeaveFactorOfSafety),
                HeaveZValue = GetNullableDecimal(output.HeaveZValue),
                SellmeijerFactorOfSafety = GetNullableDecimal(output.SellmeijerFactorOfSafety),
                SellmeijerZValue = GetNullableDecimal(output.SellmeijerZValue),
                UpliftFactorOfSafety = GetNullableDecimal(output.UpliftFactorOfSafety),
                UpliftZValue = GetNullableDecimal(output.UpliftZValue)
            };
            return entity;
        }

        private static decimal? GetNullableDecimal(double parameterValue)
        {
            if (double.IsNaN(parameterValue))
            {
                return null;
            }
            return Convert.ToDecimal(parameterValue);
        }
    }
}