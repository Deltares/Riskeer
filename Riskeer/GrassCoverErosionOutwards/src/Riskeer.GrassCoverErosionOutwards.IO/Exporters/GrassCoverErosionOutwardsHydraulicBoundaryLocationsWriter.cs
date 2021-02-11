﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Writers;
using Riskeer.GrassCoverErosionOutwards.Util;

namespace Riskeer.GrassCoverErosionOutwards.IO.Exporters
{
    /// <summary>
    /// Shapefile writer that writes the locations of a collection of
    /// <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/> as point features.
    /// </summary>
    internal static class GrassCoverErosionOutwardsHydraulicBoundaryLocationsWriter
    {
        private static readonly GrassCoverErosionOutwardsHydraulicBoundaryLocationExportMetaDataAttributeNameProvider metaDataAttributeNameProvider
            = new GrassCoverErosionOutwardsHydraulicBoundaryLocationExportMetaDataAttributeNameProvider();

        /// <summary>
        /// Writes the collection of <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/> as point features in a shapefile.
        /// </summary>
        /// <param name="locations">The hydraulic boundary locations to be written to file.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locations"/> or
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public static void WriteHydraulicBoundaryLocations(IEnumerable<GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation> locations,
                                                           string filePath)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var pointShapeFileWriter = new PointShapeFileWriter();

            foreach (MapPointData mapDataLocation in locations.Select(CreateLocationData))
            {
                pointShapeFileWriter.CopyToFeature(mapDataLocation);
            }

            pointShapeFileWriter.SaveAs(filePath);
        }

        private static MapPointData CreateLocationData(GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location)
        {
            return new MapPointData(location.Name)
            {
                Features = new[]
                {
                    GrassCoverErosionOutwardsHydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(
                        location, metaDataAttributeNameProvider)
                }
            };
        }
    }
}