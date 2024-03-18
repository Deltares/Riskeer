// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Writers;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Exporters
{
    /// <summary>
    /// Shapefile writer that writes a collection of
    /// <see cref="HydraulicBoundaryLocationCalculation"/> as point features.
    /// </summary>
    internal static class HydraulicBoundaryLocationCalculationsForTargetProbabilityWriter
    {
        /// <summary>
        /// Writes the collection of <see cref="HydraulicBoundaryLocationCalculation"/> as point features in a shapefile.
        /// </summary>
        /// <param name="calculations">The hydraulic boundary locations calculations to be written to file.</param>
        /// <param name="assessmentSection">The assessment section the calculations belong to.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <param name="calculationsType">The type of calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/>, <paramref name="assessmentSection"/> or
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <see cref="calculationsType"/>
        /// is an invalid value.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public static void WriteHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                      IAssessmentSection assessmentSection,
                                                                      string filePath, HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!Enum.IsDefined(typeof(HydraulicBoundaryLocationCalculationsType), calculationsType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationsType),
                                                       (int) calculationsType,
                                                       typeof(HydraulicBoundaryLocationCalculationsType));
            }

            var pointShapeFileWriter = new PointShapeFileWriter();
            IReadOnlyDictionary<HydraulicBoundaryLocation, string> lookup = assessmentSection.GetHydraulicBoundaryLocationLookup();
            foreach (MapPointData mapDataLocation in calculations.Select(c => CreateCalculationData(c, lookup, calculationsType)))
            {
                pointShapeFileWriter.CopyToFeature(mapDataLocation);
            }

            pointShapeFileWriter.SaveAs(filePath);
        }

        private static MapPointData CreateCalculationData(HydraulicBoundaryLocationCalculation calculation,
                                                          IReadOnlyDictionary<HydraulicBoundaryLocation, string> lookup,
                                                          HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            string metaDataHeader = calculationsType == HydraulicBoundaryLocationCalculationsType.WaterLevel
                                        ? Resources.HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaterLevel_DisplayName
                                        : Resources.HydraulicBoundaryLocationCalculationsWriter_WaterLevelCalculationType_WaveHeight_DisplayName;

            HydraulicBoundaryLocation hydraulicBoundaryLocation = calculation.HydraulicBoundaryLocation;
            return new MapPointData(hydraulicBoundaryLocation.Name)
            {
                Features = new[]
                {
                    HydraulicBoundaryLocationCalculationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationCalculationFeature(
                        calculation, lookup[hydraulicBoundaryLocation], metaDataHeader)
                }
            };
        }
    }
}