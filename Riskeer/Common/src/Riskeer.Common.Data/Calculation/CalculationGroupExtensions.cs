// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Data.Calculation
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="CalculationGroup"/> instances.
    /// </summary>
    public static class CalculationGroupExtensions
    {
        /// <summary>
        /// Recursively enumerates across the contents of a calculation group,
        /// returning all children found.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to be evaluated.</param>
        /// <returns>Returns all contained children as an enumerable result.</returns>
        public static IEnumerable<ICalculationBase> GetAllChildrenRecursive(this CalculationGroup calculationGroup)
        {
            var children = new List<ICalculationBase>();
            foreach (ICalculationBase calculationItem in calculationGroup.Children)
            {
                children.Add(calculationItem);

                if (calculationItem is CalculationGroup nestedCalculationGroup)
                {
                    children.AddRange(GetAllChildrenRecursive(nestedCalculationGroup));
                }
            }

            return children;
        }

        /// <summary>
        /// Recursively enumerates across the contents of a calculation group,
        /// returning all calculations found.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to be evaluated.</param>
        /// <returns>Returns all contained calculations as an enumerable result.</returns>
        public static IEnumerable<ICalculation> GetCalculations(this CalculationGroup calculationGroup)
        {
            var calculations = new List<ICalculation>();
            foreach (ICalculationBase calculationItem in calculationGroup.Children)
            {
                if (calculationItem is ICalculation calculation)
                {
                    calculations.Add(calculation);
                }

                if (calculationItem is CalculationGroup nestedCalculationGroup)
                {
                    calculations.AddRange(GetCalculations(nestedCalculationGroup));
                }
            }

            return calculations;
        }

        /// <summary>
        /// Method for determining if one or more calculations in a calculation group have output.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to check the output for.</param>
        /// <returns><c>true</c> if one or more calculations in the calculation group have output, <c>false</c> otherwise.</returns>
        /// <remarks>The calculation group is enumerated recursively, also taking into account nested calculations.</remarks>
        public static bool HasOutput(this CalculationGroup calculationGroup)
        {
            return calculationGroup.GetCalculations().Any(c => c.HasOutput);
        }
    }
}