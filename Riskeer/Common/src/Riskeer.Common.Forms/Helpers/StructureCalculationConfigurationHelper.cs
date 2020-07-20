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

using System;
using System.Collections.Generic;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="StructureBase"/>.
    /// </summary>
    public static class StructureCalculationConfigurationHelper
    {
        /// <summary>
        /// Configures calculations and adds them to the calculation group.
        /// </summary>
        /// <param name="calculationGroup">The calculation group.</param>
        /// <param name="structures">The collection of structures.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void GenerateCalculations<TStructureBase, TInputBase>(CalculationGroup calculationGroup, IEnumerable<TStructureBase> structures) where TStructureBase : StructureBase where TInputBase : StructuresInputBase<TStructureBase>, new()
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }

            foreach (TStructureBase structure in structures)
            {
                var calculation = new StructuresCalculationScenario<TInputBase>
                {
                    Name = NamingHelper.GetUniqueName(calculationGroup.Children, structure.Name, c => c.Name),
                    InputParameters =
                    {
                        Structure = structure
                    }
                };
                calculationGroup.Children.Add(calculation);
            }
        }
    }
}