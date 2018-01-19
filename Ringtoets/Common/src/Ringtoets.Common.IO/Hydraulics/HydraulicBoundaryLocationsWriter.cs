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
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Writers;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Hydraulics
{
    /// <summary>
    /// Shapefile writer that writes the locations of a collection of <see cref="HydraulicBoundaryLocation"/> as point features.
    /// </summary>
    public class HydraulicBoundaryLocationsWriter
    {
        private readonly IHydraulicBoundaryLocationMetaDataAttributeNameProvider metaDataAttributeNameProvider;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsWriter"/>.
        /// </summary>
        /// <param name="metaDataAttributeNameProvider">The <see cref="IHydraulicBoundaryLocationMetaDataAttributeNameProvider"/>
        /// to be used for setting meta data attribute names.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metaDataAttributeNameProvider"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocationsWriter(IHydraulicBoundaryLocationMetaDataAttributeNameProvider metaDataAttributeNameProvider)
        {
            if (metaDataAttributeNameProvider == null)
            {
                throw new ArgumentNullException(nameof(metaDataAttributeNameProvider));
            }

            this.metaDataAttributeNameProvider = metaDataAttributeNameProvider;
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
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
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
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            var hydraulicBoundaryLocationGeometry = new MapGeometry(
                new List<IEnumerable<Point2D>>
                {
                    new[]
                    {
                        hydraulicBoundaryLocation.Location
                    }
                });

            var mapFeature = new MapFeature(new[]
            {
                hydraulicBoundaryLocationGeometry
            });

            mapFeature.MetaData.Add(Resources.HydraulicBoundaryLocation_Name, hydraulicBoundaryLocation.Name);
            mapFeature.MetaData.Add(Resources.HydraulicBoundaryLocation_Id, hydraulicBoundaryLocation.Id);

            mapFeature.MetaData.Add(metaDataAttributeNameProvider.DesignWaterLevelCalculation1AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.DesignWaterLevelCalculation1));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.DesignWaterLevelCalculation2AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.DesignWaterLevelCalculation2));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.DesignWaterLevelCalculation3AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.DesignWaterLevelCalculation3));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.DesignWaterLevelCalculation4AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.DesignWaterLevelCalculation4));

            mapFeature.MetaData.Add(metaDataAttributeNameProvider.WaveHeightCalculation1AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.WaveHeightCalculation1));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.WaveHeightCalculation2AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.WaveHeightCalculation2));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.WaveHeightCalculation3AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.WaveHeightCalculation3));
            mapFeature.MetaData.Add(metaDataAttributeNameProvider.WaveHeightCalculation4AttributeName,
                                    GetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation.WaveHeightCalculation4));

            return new MapPointData(hydraulicBoundaryLocation.Name)
            {
                Features = new[]
                {
                    mapFeature
                }
            };
        }

        private static double GetHydraulicBoundaryLocationOutput(HydraulicBoundaryLocationCalculation calculation)
        {
            return calculation.Output?.Result ?? double.NaN;
        }
    }
}