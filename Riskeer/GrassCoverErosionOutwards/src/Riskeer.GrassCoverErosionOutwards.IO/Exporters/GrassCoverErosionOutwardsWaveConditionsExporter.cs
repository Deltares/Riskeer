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
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.GrassCoverErosionOutwards.IO.Exporters
{
    /// <summary>
    /// Exports grass cover erosion outwards wave conditions and stores them as a csv file.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsExporter : WaveConditionsExporterBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsExporter"/>.
        /// </summary>
        /// <param name="calculations">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> objects to export.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="getTargetProbabilityFunc"><see cref="Func{TResult}"/> for getting the target probability to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the file could not be written.</exception>
        public GrassCoverErosionOutwardsWaveConditionsExporter(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations, string filePath,
                                                               Func<WaveConditionsInput, string> getTargetProbabilityFunc)
            : base(CreateExportableWaveConditionsCollection(calculations, getTargetProbabilityFunc), filePath) {}

        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations,
                                                                                                      Func<WaveConditionsInput, string> getTargetProbabilityFunc)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (getTargetProbabilityFunc == null)
            {
                throw new ArgumentNullException(nameof(getTargetProbabilityFunc));
            }

            var exportableWaveConditions = new List<ExportableWaveConditions>();

            IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> exportableCalculations =
                calculations.Where(c => c.HasOutput && c.InputParameters.HydraulicBoundaryLocation != null);

            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in exportableCalculations)
            {
                GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType = calculation.InputParameters.CalculationType;

                if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                {
                    exportableWaveConditions.AddRange(
                        ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                            calculation.Name, calculation.InputParameters, calculation.Output.WaveRunUpOutput, CoverType.GrassWaveRunUp, getTargetProbabilityFunc));
                }

                if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                {
                    exportableWaveConditions.AddRange(
                        ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                            calculation.Name, calculation.InputParameters, calculation.Output.WaveImpactOutput, CoverType.GrassWaveImpact, getTargetProbabilityFunc));
                }

                if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                {
                    exportableWaveConditions.AddRange(
                        ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                            calculation.Name, calculation.InputParameters, calculation.Output.TailorMadeWaveImpactOutput, CoverType.GrassTailorMadeWaveImpact, getTargetProbabilityFunc));
                }
            }

            return exportableWaveConditions;
        }
    }
}