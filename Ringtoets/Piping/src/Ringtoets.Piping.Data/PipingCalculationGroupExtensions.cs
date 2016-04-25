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

using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="CalculationGroup"/> instances.
    /// </summary>
    public static class PipingCalculationGroupExtensions
    {
        /// <summary>
        /// Adds <see cref="PipingCalculationScenario"/> to <see cref="FailureMechanismBase.sectionResults"/>.
        /// </summary>
        /// <param name="pipingCalculationGroup">The group containing the calculations.</param>
        /// <param name="pipingFailureMechanism">The failure mechanism containing the section results.</param>
        public static void AddCalculationScenariosToFailureMechanismSectionResult(this ICalculationGroup pipingCalculationGroup, PipingFailureMechanism pipingFailureMechanism)
        {
            foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
            {
                var lineSegments = Math2D.ConvertLinePointsToLineSegments(failureMechanismSectionResult.Section.Points);
                var calculationScenarios = pipingCalculationGroup.GetCalculations().OfType<PipingCalculationScenario>()
                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments))
                                                                 .Where(pc => pc.GetType() == typeof(PipingCalculationScenario))
                                                                 .Where(pc => !failureMechanismSectionResult.CalculationScenarios.Contains(pc)).ToList();

                if (calculationScenarios.Any())
                {
                    failureMechanismSectionResult.CalculationScenarios.AddRange(calculationScenarios);
                }
            }
        }
    }
}