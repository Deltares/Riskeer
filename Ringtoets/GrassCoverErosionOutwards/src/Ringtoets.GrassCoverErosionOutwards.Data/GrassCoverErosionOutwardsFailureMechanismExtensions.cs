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
using System.Linq;
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="GrassCoverErosionOutwardsFailureMechanism"/> instances.
    /// </summary>
    public static class GrassCoverErosionOutwardsFailureMechanismExtensions
    {
        /// <summary>
        /// Sets <see cref="GrassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations"/> 
        /// based upon the <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionOutwardsFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(this GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                  IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            failureMechanism.HydraulicBoundaryLocations.Clear();
            if (!hydraulicBoundaryLocations.Any())
            {
                return;
            }

            hydraulicBoundaryLocations.ForEachElementDo(location => failureMechanism.HydraulicBoundaryLocations
                                                                                    .Add(new HydraulicBoundaryLocation(location.Id,
                                                                                                                       location.Name,
                                                                                                                       location.Location.X,
                                                                                                                       location.Location.Y)));
        }
    }
}