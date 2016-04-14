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

using System;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// Class responsible for piping calculation scenarios in sync.
    /// </summary>
    public static class PipingCalculationScenarioService
    {
        /// <summary>
        /// Sets the <paramref name="calculationScenario"/> to the corresponding <see cref="FailureMechanismSectionResult"/> for the <paramref name="oldSurfaceLine"/>.
        /// </summary>
        /// <param name="calculationScenario">The calculation scenario to set containing the new surface line.</param>
        /// <param name="failureMechanism">The failure mechanism containing the <see cref="FailureMechanismSectionResult"/>.</param>
        /// <param name="oldSurfaceLine">The old surface line for the calculation scenario.</param>
        public static void SyncCalculationScenarioWithNewSurfaceLine(PipingCalculationScenario calculationScenario, PipingFailureMechanism failureMechanism, RingtoetsPipingSurfaceLine oldSurfaceLine)
        {
            if (calculationScenario == null)
            {
                throw new ArgumentNullException("calculationScenario");
            }
            
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            if (oldSurfaceLine == null)
            {
                throw new ArgumentNullException("oldSurfaceLine");
            }

            if (RemoveScenarioFromOldSectionResult(calculationScenario, failureMechanism, oldSurfaceLine))
            {
                AddScenarioToNewSectionResult(calculationScenario, failureMechanism);
            }
        }

        private static void AddScenarioToNewSectionResult(PipingCalculationScenario calculationScenario, PipingFailureMechanism failureMechanism)
        {
            foreach (var failureMechanismSectionResult in failureMechanism.SectionResults)
            {
                var lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSectionResult.Section.Points);

                if (calculationScenario.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments))
                {
                    failureMechanismSectionResult.CalculationScenarios.Add(calculationScenario);
                    break;
                }
            }
        }

        private static bool RemoveScenarioFromOldSectionResult(PipingCalculationScenario calculationScenario, PipingFailureMechanism failureMechanism, RingtoetsPipingSurfaceLine oldSurfaceLine)
        {
            for (int i = 0; i < failureMechanism.SectionResults.Count(); i++)
            {
                var sectionResult = failureMechanism.SectionResults.ElementAt(i);

                for (int j = 0; j < sectionResult.CalculationScenarios.Count; j++)
                {
                    var pipingCalculation = (PipingCalculation)sectionResult.CalculationScenarios[j];

                    if (pipingCalculation.Equals(calculationScenario) && !calculationScenario.InputParameters.SurfaceLine.Equals(oldSurfaceLine))
                    {
                        sectionResult.CalculationScenarios.Remove(calculationScenario);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
