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

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Container of all data necessary for performing an assessment level calculation via Hydra-Ring.
    /// </summary>
    public class AssessmentLevelCalculationData : HydraulicCalculationData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AssessmentLevelCalculationData"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="beta">The target reliability index to use during the calculation.</param>
        public AssessmentLevelCalculationData(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId, beta) {}

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.AssessmentLevel;
            }
        }
    }
}