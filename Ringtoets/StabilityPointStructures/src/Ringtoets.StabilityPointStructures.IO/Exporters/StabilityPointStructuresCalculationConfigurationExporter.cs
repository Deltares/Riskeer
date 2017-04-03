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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.IO.Writers;

namespace Ringtoets.StabilityPointStructures.IO.Exporters
{
    /// <summary>
    /// Exports a stability point structures calculation configuration and stores it as an XML file.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationExporter : SchemaCalculationConfigurationExporter<
        StabilityPointStructuresCalculationConfigurationWriter,
        StructuresCalculation<StabilityPointStructuresInput>,
        StabilityPointStructuresCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The calculation configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public StabilityPointStructuresCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override StabilityPointStructuresCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationWriter(filePath);
        }

        protected override StabilityPointStructuresCalculationConfiguration ToConfiguration(StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            var calculationConfiguration = new StabilityPointStructuresCalculationConfiguration(calculation.Name);
            StabilityPointStructuresInput input = calculation.InputParameters;

            calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name;
            calculationConfiguration.DrainCoefficient = input.DrainCoefficient.ToStochastConfigurationWithMean();
            calculationConfiguration.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;
            calculationConfiguration.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure;
            calculationConfiguration.ModelFactorSuperCriticalFlow = input.ModelFactorSuperCriticalFlow.ToStochastConfigurationWithMean();
            calculationConfiguration.StormDuration = input.StormDuration.ToStochastConfigurationWithMean();
            calculationConfiguration.VolumicWeightWater = input.VolumicWeightWater;
            
            return calculationConfiguration;
        }
    }
}