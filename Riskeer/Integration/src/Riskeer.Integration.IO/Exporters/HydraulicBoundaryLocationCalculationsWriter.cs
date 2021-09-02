// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Util;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Exporters
{
    /// <summary>
    /// Shapefile writer that writes a collection of
    /// <see cref="HydraulicBoundaryLocationCalculation"/> as point features.
    /// </summary>
    internal static class HydraulicBoundaryLocationCalculationsWriter
    {
        /// <summary>
        /// Writes the collection of <see cref="HydraulicBoundaryLocationCalculation"/> as point features in a shapefile.
        /// </summary>
        /// <param name="calculations">The hydraulic boundary locations calculations to be written to file.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <param name="calculationsType">The type of calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public static void WriteHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                      string filePath, HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var pointShapeFileWriter = new PointShapeFileWriter();

            foreach (MapPointData mapDataLocation in calculations.Select(c => CreateCalculationData(c, calculationsType)))
            {
                pointShapeFileWriter.CopyToFeature(mapDataLocation);
            }

            pointShapeFileWriter.SaveAs(filePath);
        }

        private static MapPointData CreateCalculationData(HydraulicBoundaryLocationCalculation calculation,
                                                          HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            string metaDataHeader = calculationsType == HydraulicBoundaryLocationCalculationsType.WaterLevel
                                        ? Resources.HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaterLevel_DisplayName
                                        : Resources.HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaveHeight_DisplayName;
            
            return new MapPointData(calculation.HydraulicBoundaryLocation.Name)
            {
                Features = new[]
                {
                    HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationCalculationFeature(
                        calculation, metaDataHeader)
                }
            };
        }
    }
}