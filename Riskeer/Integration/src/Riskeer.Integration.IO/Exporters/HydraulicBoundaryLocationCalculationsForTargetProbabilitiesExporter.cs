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
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources ;

namespace Riskeer.Integration.IO.Exporters
{
    /// <summary>
    /// Exports <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> and stores them as a shapefile.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter : IFileExporter
    {
        private readonly IEnumerable<Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>> locationCalculationsForTargetProbabilities;
        private readonly string folderPath;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter"/>.
        /// </summary>
        /// <param name="locationCalculationsForTargetProbabilities">The collection of
        /// <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/> to export.</param>
        /// <param name="folderPath">The folder path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locationCalculationsForTargetProbabilities"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is invalid.</exception>
        /// <remarks>A valid path:<list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>is not too long.</item>
        /// </list></remarks>
        public HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
            IEnumerable<Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>> locationCalculationsForTargetProbabilities,
            string folderPath)
        {
            if (locationCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(locationCalculationsForTargetProbabilities));
            }

            IOUtils.ValidateFolderPath(folderPath);

            this.locationCalculationsForTargetProbabilities = locationCalculationsForTargetProbabilities;
            this.folderPath = folderPath;
        }

        public bool Export()
        {
            var exportedCalculations = new Dictionary<HydraulicBoundaryLocationCalculationsForTargetProbability, string>();
            return locationCalculationsForTargetProbabilities.All(
                locationCalculationsForTargetProbability => ExportLocationCalculationsForTargetProbability(
                    locationCalculationsForTargetProbability.Item1,
                    locationCalculationsForTargetProbability.Item2,
                    exportedCalculations));
        }

        private bool ExportLocationCalculationsForTargetProbability(
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
            HydraulicBoundaryLocationCalculationsType calculationsType,
            IDictionary<HydraulicBoundaryLocationCalculationsForTargetProbability, string> exportedCalculations)
        {
            string exportType = calculationsType == HydraulicBoundaryLocationCalculationsType.WaterLevel
                                    ? Resources.HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter_WaterLevels_DisplayName
                                    : Resources.HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter_WaveHeights_DisplayName;

            string uniqueName = NamingHelper.GetUniqueName(
                exportedCalculations, $"{exportType}_{ReturnPeriodFormattingHelper.FormatFromProbability(calculationsForTargetProbability.TargetProbability)}",
                c => c.Value);

            string filePath = Path.Combine(folderPath, $"{uniqueName}.{RiskeerCommonIOResources.Shape_file_filter_Extension}");

            var exporter = new HydraulicBoundaryLocationCalculationsExporter(
                calculationsForTargetProbability.HydraulicBoundaryLocationCalculations,
                filePath, calculationsType);

            if (!exporter.Export())
            {
                return false;
            }

            exportedCalculations.Add(calculationsForTargetProbability, uniqueName);
            return true;
        }
    }
}