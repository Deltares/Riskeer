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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="StructuresCalculationScenario{T}"/>
    /// for testing purposes.
    /// </summary>
    public static class ClosingStructuresCalculationScenarioTestFactory
    {
        /// <summary>
        /// Creates a calculated scenario for which the surface line on the input intersects with <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="StructuresCalculationScenario{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static StructuresCalculationScenario<ClosingStructuresInput> CreateClosingStructuresCalculationScenario(
            FailureMechanismSection section)
        {
            StructuresCalculationScenario<ClosingStructuresInput> scenario = CreateNotCalculatedClosingStructuresCalculationScenario(section);
            scenario.Output = new TestStructuresOutput();

            return scenario;
        }

        /// <summary>
        /// Creates a scenario for which the structure on the input intersects with <paramref name="section"/> and
        /// the calculation has not been performed.
        /// </summary>
        /// <param name="section">The section for which an intersection will be created.</param>
        /// <returns>A new <see cref="StructuresCalculationScenario{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public static StructuresCalculationScenario<ClosingStructuresInput> CreateNotCalculatedClosingStructuresCalculationScenario(
            FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            var scenario = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                IsRelevant = true,
                InputParameters =
                {
                    Structure = new TestClosingStructure(section.StartPoint)
                }
            };

            return scenario;
        }
    }
}
