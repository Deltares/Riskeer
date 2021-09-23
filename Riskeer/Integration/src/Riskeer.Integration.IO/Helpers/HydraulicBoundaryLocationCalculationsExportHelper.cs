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
using System.ComponentModel;
using System.IO;
using Core.Common.Util;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Helper class for exporting hydraulic boundary location calculations.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationsExportHelper
    {
        /// <summary>
        /// Exports the location calculations for the target probability.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The calculations to export.</param>
        /// <param name="exportedCalculations">The already exported calculations.</param>
        /// <param name="calculationsType">The type of the calculations.</param>
        /// <param name="folderPath">The path of the folder to export to.</param>
        /// <returns><c>true</c> when the export was successful; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationsForTargetProbability"/>
        /// or <paramref name="exportedCalculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationsType"/>
        /// is invalid.</exception>
        public static bool ExportLocationCalculationsForTargetProbability(
            Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double> calculationsForTargetProbability,
            IDictionary<IEnumerable<HydraulicBoundaryLocationCalculation>, string> exportedCalculations,
            HydraulicBoundaryLocationCalculationsType calculationsType,
            string folderPath)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            if (exportedCalculations == null)
            {
                throw new ArgumentNullException(nameof(exportedCalculations));
            }

            if (!Enum.IsDefined(typeof(HydraulicBoundaryLocationCalculationsType), calculationsType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationsType),
                                                       (int) calculationsType,
                                                       typeof(HydraulicBoundaryLocationCalculationsType));
            }

            IOUtils.ValidateFolderPath(folderPath);

            IEnumerable<HydraulicBoundaryLocationCalculation> calculations = calculationsForTargetProbability.Item1;
            double targetProbability = calculationsForTargetProbability.Item2;

            string exportType = calculationsType == HydraulicBoundaryLocationCalculationsType.WaterLevel
                                    ? Resources.WaterLevels_DisplayName
                                    : Resources.WaveHeights_DisplayName;

            string uniqueName = NamingHelper.GetUniqueName(
                exportedCalculations, $"{exportType}_{ReturnPeriodFormattingHelper.FormatFromProbability(targetProbability)}",
                c => c.Value);

            string tempFilePath = Path.Combine(folderPath, $"{uniqueName}.{RiskeerCommonIOResources.Shape_file_filter_Extension}");

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                calculations, tempFilePath, calculationsType);

            if (!exporter.Export())
            {
                return false;
            }

            exportedCalculations.Add(calculations, uniqueName);
            return true;
        }
    }
}