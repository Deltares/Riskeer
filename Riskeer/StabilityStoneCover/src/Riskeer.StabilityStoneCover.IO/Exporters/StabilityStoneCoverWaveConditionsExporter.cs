﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.WaveConditions;
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.IO.Exporters
{
    /// <summary>
    /// Exports stability stone cover wave conditions and stores them as a csv file.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsExporter : WaveConditionsExporterBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsExporter"/>.
        /// </summary>
        /// <param name="calculations">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/> objects to export.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="getTargetProbabilityFunc"><see cref="Func{TResult}"/> for getting the target probability to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or 
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the file could not be written.</exception>
        public StabilityStoneCoverWaveConditionsExporter(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, string filePath,
                                                         Func<WaveConditionsInput, string> getTargetProbabilityFunc)
            : base(CreateExportableWaveConditionsCollection(calculations, getTargetProbabilityFunc), filePath) {}

        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations,
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

            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> exportableCalculations =
                calculations.Where(c => c.HasOutput && c.InputParameters.HydraulicBoundaryLocation != null);

            return CreateExportableWaveConditions(exportableCalculations, getTargetProbabilityFunc);
        }

        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditions(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> exportableCalculations,
                                                                                            Func<WaveConditionsInput, string> getTargetProbabilityFunc)
        {
            var exportableWaveConditions = new List<ExportableWaveConditions>();
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in exportableCalculations)
            {
                StabilityStoneCoverWaveConditionsCalculationType calculationType = calculation.InputParameters.CalculationType;
                if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                    || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Blocks)
                {
                    exportableWaveConditions.AddRange(
                        ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                            calculation.Name, calculation.InputParameters, calculation.Output.BlocksOutput, CoverType.StoneCoverBlocks, getTargetProbabilityFunc));
                }

                if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                    || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns)
                {
                    exportableWaveConditions.AddRange(
                        ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                            calculation.Name, calculation.InputParameters, calculation.Output.ColumnsOutput, CoverType.StoneCoverColumns, getTargetProbabilityFunc));
                }
            }

            return exportableWaveConditions;
        }
    }
}