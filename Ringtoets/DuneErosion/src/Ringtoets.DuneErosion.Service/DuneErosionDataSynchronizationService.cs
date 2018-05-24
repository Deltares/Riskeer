﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
                                            ICollection<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                            IEnumerable<ReadDuneLocation> duneLocations)
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

            if (hydraulicBoundaryLocations.Count == 0 || !duneLocations.Any())
            {
                return;
            }

            var correspondingDuneLocations = new List<DuneLocation>();
            foreach (ReadDuneLocation readDuneLocation in duneLocations)
            {
                HydraulicBoundaryLocation correspondingHydraulicBoundaryLocation = hydraulicBoundaryLocations
                    .FirstOrDefault(hbl => DoesHydraulicBoundaryLocationMatchWithDuneLocation(hbl, readDuneLocation));
                if (correspondingHydraulicBoundaryLocation != null)
                {
                    var duneLocation = new DuneLocation(correspondingHydraulicBoundaryLocation.Id,
                                                        readDuneLocation.Name,
                                                        readDuneLocation.Location,
                                                        new DuneLocation.ConstructionProperties
                                                        {
                                                            CoastalAreaId = readDuneLocation.CoastalAreaId,
                                                            Offset = readDuneLocation.Offset,
                                                            Orientation = readDuneLocation.Orientation,
                                                            D50 = readDuneLocation.D50
                                                        });
                    correspondingDuneLocations.Add(duneLocation);
                }
            }

            failureMechanism.SetDuneLocations(correspondingDuneLocations);
        }

        /// <summary>
        /// Clears the output of the dune locations within the collection.
        /// </summary>
        /// <param name="locations">The locations for which the output needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearDuneLocationOutput(IObservableEnumerable<DuneLocation> locations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            return locations.SelectMany(ClearDuneLocationOutput)
                            .ToArray();
        }

        /// <summary>
        /// Clears the output of the dune location calculations within the dune erosion failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism for which the output of the calculations needs to be cleared.</param>
        /// <returns>All objects changed during the clear.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<IObservable> ClearDuneCalculationOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            var affectedCalculations = new List<IObservable>();

            affectedCalculations.AddRange(ClearDuneCalculationsOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm));
            affectedCalculations.AddRange(ClearDuneCalculationsOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm));
            affectedCalculations.AddRange(ClearDuneCalculationsOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm));
            affectedCalculations.AddRange(ClearDuneCalculationsOutput(failureMechanism.CalculationsForLowerLimitNorm));
            affectedCalculations.AddRange(ClearDuneCalculationsOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm));

            return affectedCalculations;
        }

        private static IEnumerable<IObservable> ClearDuneCalculationsOutput(IEnumerable<DuneLocationCalculation> calculations)
        {
            var affectedCalculations = new List<IObservable>();
            foreach (DuneLocationCalculation calculation in calculations)
            {
                if (calculation.Output != null)
                {
                    calculation.Output = null;
                    affectedCalculations.Add(calculation);
                }
            }

            return affectedCalculations;
        }

        private static bool DoesHydraulicBoundaryLocationMatchWithDuneLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                               ReadDuneLocation readDuneLocation)
        {
            if (!Math2D.AreEqualPoints(hydraulicBoundaryLocation.Location, readDuneLocation.Location))
            {
                return false;
            }

            // Regex to search for a pattern like "<Some text without white spaces>_<integer>_<decimal>"
            // Only the last number is captured in a group called "Offset"
            // The last number can also contain decimals.
            var regex = new Regex(@"^(?:\S+)_(?:\d+)_(?<Offset>(?:\d+\.)?\d+$)");
            Match match = regex.Match(hydraulicBoundaryLocation.Name);

            if (!match.Success)
            {
                log.ErrorFormat(Resources.DuneErosionDataSynchronizationService_SetDuneLocations_Location_0_is_dune_location_but_name_is_not_according_format,
                                hydraulicBoundaryLocation.Name);
                return false;
            }

            string duneLocationOffset = readDuneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format,
                                                                         CultureInfo.InvariantCulture);

            return match.Groups["Offset"].Value == duneLocationOffset;
        }

        private static IEnumerable<IObservable> ClearDuneLocationOutput(DuneLocation location)
        {
            if (location.Calculation.Output != null)
            {
                location.Calculation.Output = null;
                yield return location;
            }
        }
    }
}