// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations.Helpers;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.IO.Exporters
{
    /// <summary>
    /// Exports a wave impact asphalt cover calculation configuration and stores it as an XML file.
    /// </summary>
    public class WaveImpactAsphaltCoverCalculationConfigurationExporter
        : CalculationConfigurationExporter<WaveConditionsCalculationConfigurationWriter, WaveImpactAsphaltCoverWaveConditionsCalculation, WaveConditionsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="configuration">The calculation configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public WaveImpactAsphaltCoverCalculationConfigurationExporter(IEnumerable<ICalculationBase> configuration, string filePath)
            : base(configuration, filePath) {}

        protected override WaveConditionsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new WaveConditionsCalculationConfigurationWriter(filePath);
        }

        protected override WaveConditionsCalculationConfiguration ToConfiguration(WaveImpactAsphaltCoverWaveConditionsCalculation calculation)
        {
            return calculation.InputParameters.ToConfiguration(calculation.Name);
        }
    }
}