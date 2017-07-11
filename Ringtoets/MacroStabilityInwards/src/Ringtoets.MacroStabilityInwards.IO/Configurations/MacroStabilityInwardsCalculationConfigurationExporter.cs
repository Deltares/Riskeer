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
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Exports a macro stability inwards calculation configuration and stores it as an XML file.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            MacroStabilityInwardsCalculationConfigurationWriter,
            MacroStabilityInwardsCalculationScenario,
            MacroStabilityInwardsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public MacroStabilityInwardsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override MacroStabilityInwardsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationWriter(filePath);
        }

        protected override MacroStabilityInwardsCalculationConfiguration ToConfiguration(MacroStabilityInwardsCalculationScenario calculation)
        {
            MacroStabilityInwardsInput input = calculation.InputParameters;

            var calculationConfiguration = new MacroStabilityInwardsCalculationConfiguration(calculation.Name);

            if (input.HydraulicBoundaryLocation != null)
            {
                calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation.Name;
            }
            else if (input.UseAssessmentLevelManualInput)
            {
                calculationConfiguration.AssessmentLevel = input.AssessmentLevel;
            }

            if (input.SurfaceLine != null)
            {
                calculationConfiguration.SurfaceLineName = input.SurfaceLine.Name;
            }

            if (input.StochasticSoilModel != null)
            {
                calculationConfiguration.StochasticSoilModelName = input.StochasticSoilModel.Name;
                calculationConfiguration.StochasticSoilProfileName = input.StochasticSoilProfile?.SoilProfile.Name;
            }

            calculationConfiguration.Scenario = calculation.ToScenarioConfiguration();

            return calculationConfiguration;
        }
    }
}