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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.IO.Configurations
{
    /// <summary>
    /// Exports a wave impact asphalt cover wave conditions calculation configuration and stores it as an XML file.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter : WaveConditionsCalculationConfigurationExporter<
        WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter,
        WaveConditionsCalculationConfiguration,
        WaveImpactAsphaltCoverWaveConditionsCalculation>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter(
            IEnumerable<ICalculationBase> calculations, string filePath, IAssessmentSection assessmentSection)
            : base(calculations, new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationWriter(filePath), assessmentSection) {}

        protected override WaveConditionsCalculationConfiguration ToConfiguration(
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation)
        {
            var configuration = new WaveConditionsCalculationConfiguration(calculation.Name);
            SetConfigurationProperties(configuration, calculation);
            return configuration;
        }
    }
}