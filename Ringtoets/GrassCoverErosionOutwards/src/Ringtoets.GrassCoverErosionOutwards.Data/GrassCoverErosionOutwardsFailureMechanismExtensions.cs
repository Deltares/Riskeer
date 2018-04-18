// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="GrassCoverErosionOutwardsFailureMechanism"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsFailureMechanismExtensions
    {
        /// <summary>
        /// Gets the normative assessment level for a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the normative assessment level from.</param>
        /// <param name="failureMechanism">The failure mechanism to get the normative assessment level from.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the normative assessment level for.</param>
        /// <returns>The normative assessment level or <see cref="RoundedDouble.NaN"/> when:
        /// <list type="bullet">
        /// <item><paramref name="hydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="failureMechanism"/>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> is not part of <paramref name="assessmentSection"/>;</item>
        /// <item><paramref name="hydraulicBoundaryLocation"/> contains no corresponding calculation output.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSection"/>
        /// contains an invalid value of <see cref="NormType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSection"/>
        /// contains a valid value of <see cref="NormType"/>, but unsupported.</exception>
        public static RoundedDouble GetNormativeAssessmentLevel(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                IAssessmentSection assessmentSection,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            NormType normType = assessmentSection.FailureMechanismContribution.NormativeNorm;

            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int)normType,
                                                       typeof(NormType));
            }

            IEnumerable<HydraulicBoundaryLocationCalculation> calculations;

            switch (normType)
            {
                case NormType.Signaling:
                    calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm;
                    break;
                case NormType.LowerLimit:
                    calculations = failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return calculations.FirstOrDefault(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation))?.Output?.Result
                   ?? RoundedDouble.NaN;
        }
    }
}