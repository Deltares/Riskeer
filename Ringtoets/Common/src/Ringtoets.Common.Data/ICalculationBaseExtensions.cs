// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="ICalculationBase"/> instances.
    /// </summary>
    public static class ICalculationBaseExtensions
    {
        /// <summary>
        /// Recursively enumerates across over the contents of the piping calculation item, 
        /// yielding the piping calculations found within the calculation item.
        /// </summary>
        /// <param name="calculationItem">The calculation item to be evaluated.</param>
        /// <returns>Returns all contained piping calculations as an enumerable result.</returns>
        public static IEnumerable<ICalculationScenario> GetCalculations(this ICalculationBase calculationItem)
        {
            var calculationScenario = calculationItem as ICalculationScenario;
            if (calculationScenario != null)
            {
                yield return calculationScenario;
            }
            var group = calculationItem as CalculationGroup;
            if (group != null)
            {
                foreach (ICalculationScenario calculationInGroup in group.Children.SelectMany(g => g.GetCalculations()))
                {
                    yield return calculationInGroup;
                }
            }
        }
    }
}