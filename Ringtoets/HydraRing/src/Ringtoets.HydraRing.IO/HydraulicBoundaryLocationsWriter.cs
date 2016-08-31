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
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Writers;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.IO
{
    /// <summary>
    /// Shapefile writer that writes the locations of a <see cref="HydraulicBoundaryDatabase"/> as point features.
    /// </summary>
    public class HydraulicBoundaryLocationsWriter
    {
        private readonly string designWaterLevelName;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsWriter"/>.
        /// </summary>
        /// <param name="designWaterLevelName">The Dutch name of the content of the 
        /// <see cref="IHydraulicBoundaryLocation.DesignWaterLevel"/> property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designWaterLevelName"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocationsWriter(string designWaterLevelName)
        {
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException("designWaterLevelName");
            }

            this.designWaterLevelName = designWaterLevelName;
        }

        /// <summary>
        /// Writes the locations of a <see cref="HydraulicBoundaryDatabase"/> as point features in a shapefile.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to be written to file.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> or
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public void WriteHydraulicBoundaryLocations(IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                    string filePath)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocations");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var pointShapeFileWriter = new PointShapeFileWriter();

            foreach (MapPointData mapLineData in hydraulicBoundaryLocations.Select(CreateMapPointData))
            {
                pointShapeFileWriter.CopyToFeature(mapLineData);
            }

            pointShapeFileWriter.SaveAs(filePath);
        }

        private MapPointData CreateMapPointData(IHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }

            MapGeometry hydraulicBoundaryLocationGeometry = new MapGeometry(
                new List<IEnumerable<Point2D>>
                {
                    new[]
                    {
                        hydraulicBoundaryLocation.Location
                    }
                });

            MapFeature mapFeature = new MapFeature(new[]
            {
                hydraulicBoundaryLocationGeometry
            });

            mapFeature.MetaData.Add("Naam", hydraulicBoundaryLocation.Name);
            mapFeature.MetaData.Add("ID", hydraulicBoundaryLocation.Id);
            mapFeature.MetaData.Add(designWaterLevelName, hydraulicBoundaryLocation.DesignWaterLevel.Value);
            mapFeature.MetaData.Add("Golfhoogte", hydraulicBoundaryLocation.WaveHeight.Value);

            return new MapPointData(hydraulicBoundaryLocation.Name)
            {
                Features = new[]
                {
                    mapFeature
                }
            };
        }
    }
}