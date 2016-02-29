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
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.Hydraulics
{
    /// <summary>
    /// Container of all data necessary for performing an assessment level calculation via Hydra-Ring.
    /// </summary>
    public class AssessmentLevelCalculation : HydraulicCalculation
    {
        private readonly HydraRingDikeSection dikeSection;

        /// <summary>
        /// Creates a new instance of the <see cref="AssessmentLevelCalculation"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="beta">The target reliability index to use during the calculation.</param>
        public AssessmentLevelCalculation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId, beta)
        {
            dikeSection = new HydraRingDikeSection(HydraulicBoundaryLocationId, HydraulicBoundaryLocationId.ToString(), double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.AssessmentLevel;
            }
        }

        public override HydraRingDikeSection DikeSection
        {
            get
            {
                return dikeSection;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new AssessmentLevelVariable();
            }
        }

        private class AssessmentLevelVariable : HydraRingVariable
        {
            public AssessmentLevelVariable() : base(26, HydraRingDistributionType.Deterministic, 0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) { }
        }
    }
}