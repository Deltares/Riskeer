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
using System.IO;
using System.IO.Compression;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Util;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Exporters
{
    /// <summary>
    /// Exports hydraulic boundary locations calculations and stores them as shape files in a zip file.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationCalculationsExporter));
        private readonly IAssessmentSection assessmentSection;
        private readonly string filePath;
        private readonly string tempFolderPath;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsExporter"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get locations and calculations from.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationCalculationsExporter(IAssessmentSection assessmentSection, string filePath)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IOUtils.ValidateFilePath(filePath);

            this.assessmentSection = assessmentSection;
            this.filePath = filePath;
            string folderPath = Path.GetDirectoryName(filePath);
            tempFolderPath = Path.Combine(folderPath, "~temp");
        }

        public bool Export()
        {
            try
            {
                if (!ExportWaterLevelsForNormProbabilities()
                    || !ExportWaterLevelCalculationsForUserDefinedTargetProbabilities()
                    || !ExportWaveHeightCalculationsForUserDefinedTargetProbabilities())
                {
                    return false;
                }

                ZipFile.CreateFromDirectory(tempFolderPath, filePath);
                return true;
            }
            catch (Exception e)
            {
                log.ErrorFormat(RiskeerCommonIOResources.HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported,
                                e.Message);
                return false;
            }
            finally
            {
                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }
            }
        }

        private bool ExportWaterLevelsForNormProbabilities()
        {
            return ExportLocationCalculationsForTargetProbabilities(
                new[]
                {
                    new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                        assessmentSection.WaterLevelCalculationsForLowerLimitNorm, assessmentSection.FailureMechanismContribution.LowerLimitNorm),
                    new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                        assessmentSection.WaterLevelCalculationsForSignalingNorm, assessmentSection.FailureMechanismContribution.SignalingNorm)
                },
                HydraulicBoundaryLocationCalculationsType.WaterLevel,
                Path.Combine(tempFolderPath, RiskeerCommonFormsResources.WaterLevelCalculationsForNormTargetProbabilities_DisplayName));
        }

        private bool ExportWaterLevelCalculationsForUserDefinedTargetProbabilities()
        {
            return ExportLocationCalculationsForTargetProbabilities(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                 .Select(tp => new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                                             tp.HydraulicBoundaryLocationCalculations, tp.TargetProbability)),
                HydraulicBoundaryLocationCalculationsType.WaterLevel,
                Path.Combine(tempFolderPath, RiskeerCommonFormsResources.WaterLevelCalculationsForUserDefinedTargetProbabilities_DisplayName));
        }

        private bool ExportWaveHeightCalculationsForUserDefinedTargetProbabilities()
        {
            return ExportLocationCalculationsForTargetProbabilities(
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                 .Select(tp => new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                                             tp.HydraulicBoundaryLocationCalculations, tp.TargetProbability)),
                HydraulicBoundaryLocationCalculationsType.WaveHeight,
                Path.Combine(tempFolderPath, RiskeerCommonFormsResources.WaveHeightCalculationsForUserDefinedTargetProbabilities_DisplayName));
        }

        private static bool ExportLocationCalculationsForTargetProbabilities(
            IEnumerable<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>> calculationsForTargetProbabilities,
            HydraulicBoundaryLocationCalculationsType calculationsType, string folderPath)
        {
            var exportedCalculations = new Dictionary<IEnumerable<HydraulicBoundaryLocationCalculation>, string>();
            return calculationsForTargetProbabilities.All(
                calculationsForTargetProbability => ExportLocationCalculationsForTargetProbability(
                    calculationsForTargetProbability, exportedCalculations, calculationsType, folderPath));
        }

        private static bool ExportLocationCalculationsForTargetProbability(
            Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double> calculationsForTargetProbability,
            IDictionary<IEnumerable<HydraulicBoundaryLocationCalculation>, string> exportedCalculations,
            HydraulicBoundaryLocationCalculationsType calculationsType,
            string folderPath)
        {
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