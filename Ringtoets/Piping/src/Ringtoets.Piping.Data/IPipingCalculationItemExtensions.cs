using System.Collections.Generic;
using System.Linq;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Defines extension methods dealing with with <see cref="IPipingCalculationItem"/> instances.
    /// </summary>
    public static class IPipingCalculationItemExtensions
    {
        /// <summary>
        /// Recursively enumerates across over the contents of the piping calculation item, 
        /// yielding the piping calculations found within the calculation item.
        /// </summary>
        /// <param name="pipingCalculationItem">The calculation item to be evaluated.</param>
        /// <returns>Returns all contained piping calculations as an enumerable result.</returns>
        public static IEnumerable<PipingCalculation> GetPipingCalculations(this IPipingCalculationItem pipingCalculationItem)
        {
            var calculation = pipingCalculationItem as PipingCalculation;
            if (calculation != null)
            {
                yield return calculation;
            }
            var group = pipingCalculationItem as PipingCalculationGroup;
            if (group != null)
            {
                foreach (PipingCalculation calculationInGroup in group.Children.GetPipingCalculations())
                {
                    yield return calculationInGroup;
                }
            }
        }

        /// <summary>
        /// Recursively enumerates across over the contents of all the piping calculation 
        /// items, yielding the piping calculations found within those calculation items.
        /// </summary>
        /// <param name="pipingCalculationItems">The calculation items to be evaluated.</param>
        /// <returns>Returns all contained piping calculations as an enumerable result.</returns>
        public static IEnumerable<PipingCalculation> GetPipingCalculations(this IEnumerable<IPipingCalculationItem> pipingCalculationItems)
        {
            return pipingCalculationItems.SelectMany(GetPipingCalculations);
        }
    }
}