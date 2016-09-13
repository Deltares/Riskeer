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

using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="GrassCoverErosionOutwardsFailureMechanism"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsFailureMechanismExtensions
    {
        /// <summary>
        /// Sets <see cref="GrassCoverErosionOutwardsFailureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations"/> 
        /// based upon the <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryDatabase">The database to use.</param>
        public static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                  HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.Clear();
            if (hydraulicBoundaryDatabase == null)
            {
                return;
            }

            foreach (var hydraulicBoundaryLocation in hydraulicBoundaryDatabase.Locations)
            {
                failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations.Add(new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation));
            }
        }
    }
}