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
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO
{
    /// <summary>
    /// Shapefile writer that writes the locations of a collection of <see cref="HydraulicBoundaryLocation"/> as point features.
    /// </summary>
    public class HydraulicBoundaryLocationsWriter
    {
        private readonly string designWaterLevelName;
        private readonly string waveHeightName;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsWriter"/>.
        /// </summary>
        /// <param name="designWaterLevelName">The Dutch name of the content of the 
        /// <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> property.</param>
        /// <param name="waveHeightName">The Dutch name of the content of the
        /// <see cref="HydraulicBoundaryLocation.WaveHeight"/> property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designWaterLevelName"/> or
        /// <see cref="waveHeightName"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocationsWriter(string designWaterLevelName, string waveHeightName)
        {
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException("designWaterLevelName");
            }
            if (waveHeightName == null)
            {
                throw new ArgumentNullException("waveHeightName");
            }

            this.designWaterLevelName = designWaterLevelName;
            this.waveHeightName = waveHeightName;
        }

        /// <summary>
        /// Writes the collection of <see cref="HydraulicBoundaryLocation"/> as point features in a shapefile.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to be written to file.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> or
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public void WriteHydraulicBoundaryLocations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
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

            foreach (MapPointData mapPointData in hydraulicBoundaryLocations.Select(CreateMapPointData))
            {
                pointShapeFileWriter.CopyToFeature(mapPointData);
            }

            pointShapeFileWriter.SaveAs(filePath);
        }

        private MapPointData CreateMapPointData(HydraulicBoundaryLocation hydraulicBoundaryLocation)
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

            mapFeature.MetaData.Add(Resources.HydraulicBoundaryLocation_Name, hydraulicBoundaryLocation.Name);
            mapFeature.MetaData.Add(Resources.HydraulicBoundaryLocation_Id, hydraulicBoundaryLocation.Id);
            mapFeature.MetaData.Add(designWaterLevelName, hydraulicBoundaryLocation.DesignWaterLevel.Value);
            mapFeature.MetaData.Add(waveHeightName, hydraulicBoundaryLocation.WaveHeight.Value);

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