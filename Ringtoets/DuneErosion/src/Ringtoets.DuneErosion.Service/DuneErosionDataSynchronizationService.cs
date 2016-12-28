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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.IO;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// Service for synchronizing dune erosion data.
    /// </summary>
    public static class DuneErosionDataSynchronizationService
    {
        /// <summary>
        /// Sets <see cref="DuneErosionFailureMechanism.DuneLocations"/> based upon 
        /// the locations from the <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryDatabase">The database to use.</param>
        /// <param name="duneLocations">The dune locations to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static void SetDuneLocations(DuneErosionFailureMechanism failureMechanism,
                                            HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                            IEnumerable<ReadDuneLocation> duneLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            failureMechanism.DuneLocations.Clear();

            if (hydraulicBoundaryDatabase == null || duneLocations == null || !duneLocations.Any())
            {
                return;
            }

            foreach (ReadDuneLocation duneLocation in duneLocations)
            {
                foreach (var hydraulicBoundaryLocation in hydraulicBoundaryDatabase.Locations)
                {
                    if (Math2D.AreEqualPoints(hydraulicBoundaryLocation.Location, duneLocation.Location))
                    {
                        failureMechanism.DuneLocations.Add(new DuneLocation(hydraulicBoundaryLocation.Id,
                                                                            duneLocation.Name,
                                                                            duneLocation.Location.X,
                                                                            duneLocation.Location.Y,
                                                                            duneLocation.CoastalAreaId,
                                                                            duneLocation.Offset,
                                                                            duneLocation.Orientation,
                                                                            duneLocation.D50));
                        break;
                    }
                }
            }
        }

    }
}