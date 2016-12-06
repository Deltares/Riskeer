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
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data.Properties;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="GrassCoverErosionOutwardsFailureMechanism"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsFailureMechanismExtensions
    {
        /// <summary>
        /// Sets <see cref="GrassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations"/> 
        /// based upon the <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryDatabase">The database to use.</param>
        public static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                  HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            failureMechanism.HydraulicBoundaryLocations.Clear();
            if (hydraulicBoundaryDatabase == null)
            {
                return;
            }

            hydraulicBoundaryDatabase.Locations
                                     .ForEachElementDo(location => failureMechanism.HydraulicBoundaryLocations
                                                                                   .Add(new HydraulicBoundaryLocation(location.Id,
                                                                                                                      location.Name,
                                                                                                                      location.Location.X,
                                                                                                                      location.Location.Y)));
        }

        /// <summary>
        /// Gets the norm which is needed in the calculations within <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to get the failure mechanism norm for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> which contains the assessment section norm</param>
        /// <returns>The value of the failure mechanism norm.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="failureMechanism"/> has no (0) contribution.</exception>
        public static double GetMechanismSpecificNorm(this GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }
            if (!(failureMechanism.Contribution > 0))
            {
                throw new ArgumentException(Resources.GrassCoverErosionOutwardsFailureMechanismExtensions_GetMechanismSpecificNorm_Contribution_is_zero);
            }

            return assessmentSection.FailureMechanismContribution.Norm
                   *(failureMechanism.Contribution/100)
                   /failureMechanism.GeneralInput.N;
        }
    }
}