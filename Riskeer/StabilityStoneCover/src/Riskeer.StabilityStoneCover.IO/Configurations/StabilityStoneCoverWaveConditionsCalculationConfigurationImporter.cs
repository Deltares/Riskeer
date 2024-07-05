﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.IO.Configurations.Converters;

namespace Riskeer.StabilityStoneCover.IO.Configurations
{
    /// <summary>
    /// Imports a stability stone cover wave conditions calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationConfigurationImporter : WaveConditionsCalculationConfigurationImporter<
        StabilityStoneCoverWaveConditionsCalculation,
        StabilityStoneCoverWaveConditionsCalculationConfigurationReader,
        StabilityStoneCoverWaveConditionsCalculationConfiguration>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationConfigurationImporter"/>.
        /// </summary>
        public StabilityStoneCoverWaveConditionsCalculationConfigurationImporter(string xmlFilePath, CalculationGroup importTarget,
                                                                                 IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                 IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                                                 FailureMechanismContribution failureMechanismContribution,
                                                                                 IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
            : base(xmlFilePath, importTarget, hydraulicBoundaryLocations, foreshoreProfiles, failureMechanismContribution, calculationsForTargetProbabilities) {}

        protected override StabilityStoneCoverWaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new StabilityStoneCoverWaveConditionsCalculationConfigurationReader(xmlFilePath);
        }

        protected override void SetCalculationSpecificParameters(StabilityStoneCoverWaveConditionsCalculationConfiguration calculationConfiguration,
                                                                 StabilityStoneCoverWaveConditionsCalculation calculation)
        {
            if (calculationConfiguration.CalculationType.HasValue)
            {
                calculation.InputParameters.CalculationType = (StabilityStoneCoverWaveConditionsCalculationType) new ConfigurationStabilityStoneCoverCalculationTypeConverter()
                    .ConvertTo(calculationConfiguration.CalculationType.Value, typeof(StabilityStoneCoverWaveConditionsCalculationType));
            }
        }
    }
}