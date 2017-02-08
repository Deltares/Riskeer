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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.IO;
using Ringtoets.DuneErosion.Service.Properties;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// Service for synchronizing dune erosion data.
    /// </summary>
    public static class DuneErosionDataSynchronizationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneErosionDataSynchronizationService));

        /// <summary>
        /// Sets <see cref="DuneErosionFailureMechanism.DuneLocations"/> based upon 
        /// the <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="DuneErosionFailureMechanism"/> to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary location to use.</param>
        /// <param name="duneLocations">The dune locations to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void SetDuneLocations(DuneErosionFailureMechanism failureMechanism,
                                            HydraulicBoundaryLocation[] hydraulicBoundaryLocations,
                                            ReadDuneLocation[] duneLocations)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }

            failureMechanism.DuneLocations.Clear();

            if (!hydraulicBoundaryLocations.Any() || !duneLocations.Any())
            {
                return;
            }

            foreach (ReadDuneLocation duneLocation in duneLocations)
            {
                string duneLocationOffset = duneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture);

                foreach (var hydraulicBoundaryLocation in hydraulicBoundaryLocations)
                {
                    if (Math2D.AreEqualPoints(hydraulicBoundaryLocation.Location, duneLocation.Location))
                    {
                        // Regex to search for a pattern like "schiermonnikoog_21_100.1"
                        // Only the last number is captured in a group called "Offset"
                        // The last number can also contain decimals.
                        var regex = new Regex(@"^(?:\S+)_(?:\d+)_(?<Offset>(?:\d+\.)?\d+$)");
                        Match match = regex.Match(hydraulicBoundaryLocation.Name);

                        if (match.Success)
                        {
                            if (match.Groups["Offset"].Value == duneLocationOffset)
                            {
                                failureMechanism.DuneLocations.Add(new DuneLocation(hydraulicBoundaryLocation.Id,
                                                                                    duneLocation.Name,
                                                                                    duneLocation.Location,
                                                                                    new DuneLocation.ConstructionProperties
                                                                                    {
                                                                                        CoastalAreaId = duneLocation.CoastalAreaId,
                                                                                        Offset = duneLocation.Offset,
                                                                                        Orientation = duneLocation.Orientation,
                                                                                        D50 = duneLocation.D50
                                                                                    }));
                                break;
                            }
                        }
                        else
                        {
                            log.ErrorFormat(Resources.DuneErosionDataSynchronizationService_SetDuneLocations_Location_0_is_dune_location_but_name_is_not_according_format,
                                            hydraulicBoundaryLocation.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the output of the dune locations within the collection.
        /// </summary>
        /// <param name="locations">The locations for which the output needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearDuneLocationOutput(ObservableList<DuneLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            return locations.SelectMany(ClearDuneLocationOutput)
                            .ToArray();
        }

        private static IEnumerable<IObservable> ClearDuneLocationOutput(DuneLocation location)
        {
            if (location.Output != null)
            {
                location.Output = null;
                yield return location;
            }
        }
    }
}