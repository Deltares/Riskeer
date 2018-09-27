// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Ringtoets.Revetment.IO.WaveConditions;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.IO.Exporters
{
    /// <summary>
    /// Exports wave impact asphalt cover wave conditions and stores then as a csv file.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsExporter : WaveConditionsExporterBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsExporter"/>.
        /// </summary>
        /// <param name="calculations">The <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> objects to export.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the file could not be written.</exception>
        public WaveImpactAsphaltCoverWaveConditionsExporter(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations, string filePath)
            : base(CreateExportableWaveConditionsCollection(calculations), filePath) {}

        private static IEnumerable<ExportableWaveConditions> CreateExportableWaveConditionsCollection(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            var exportableWaveConditions = new List<ExportableWaveConditions>();

            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> exportableCalculations =
                calculations.Where(c => c.HasOutput && c.InputParameters.HydraulicBoundaryLocation != null);

            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in exportableCalculations)
            {
                exportableWaveConditions.AddRange(
                    ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                        calculation.Name, calculation.InputParameters, calculation.Output.Items));
            }

            return exportableWaveConditions;
        }
    }
}