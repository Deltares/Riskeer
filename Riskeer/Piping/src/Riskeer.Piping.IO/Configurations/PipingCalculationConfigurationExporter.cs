// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.IO.Configurations
{
    /// <summary>
    /// Exports a piping calculation configuration and stores it as an XML file.
    /// </summary>
    public class PipingCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            PipingCalculationConfigurationWriter,
            IPipingCalculationScenario<PipingInput>,
            PipingCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public PipingCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, new PipingCalculationConfigurationWriter(filePath)) {}

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="calculation"/>
        /// is of an unsupported type.</exception>
        protected override PipingCalculationConfiguration ToConfiguration(IPipingCalculationScenario<PipingInput> calculation)
        {
            PipingInput input = calculation.InputParameters;
            PipingCalculationConfigurationType calculationConfigurationType = GetCalculationConfigurationType(calculation);

            var calculationConfiguration = new PipingCalculationConfiguration(calculation.Name, calculationConfigurationType)
            {
                DampingFactorExit = input.DampingFactorExit.ToStochastConfiguration(),
                PhreaticLevelExit = input.PhreaticLevelExit.ToStochastConfiguration(),
                Scenario = calculation.ToScenarioConfiguration()
            };

            if (input.SurfaceLine != null)
            {
                calculationConfiguration.SurfaceLineName = input.SurfaceLine.Name;
                calculationConfiguration.EntryPointL = input.EntryPointL;
                calculationConfiguration.ExitPointL = input.ExitPointL;
            }

            if (input.StochasticSoilModel != null)
            {
                calculationConfiguration.StochasticSoilModelName = input.StochasticSoilModel.Name;
                calculationConfiguration.StochasticSoilProfileName = input.StochasticSoilProfile?.SoilProfile.Name;
            }

            if (calculationConfigurationType == PipingCalculationConfigurationType.SemiProbabilistic)
            {
                ToSemiProbabilisticConfiguration(calculationConfiguration, (SemiProbabilisticPipingInput) calculation.InputParameters);
            }

            if (calculationConfigurationType == PipingCalculationConfigurationType.Probabilistic)
            {
                ToProbabilisticConfiguration(calculationConfiguration, (ProbabilisticPipingInput) calculation.InputParameters);
            }

            return calculationConfiguration;
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationConfigurationType"/> based on the type of <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation scenario to get the type for.</param>
        /// <returns>The <see cref="PipingCalculationConfigurationType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="calculation"/>
        /// is of an unsupported type.</exception>
        private static PipingCalculationConfigurationType GetCalculationConfigurationType(IPipingCalculationScenario<PipingInput> calculation)
        {
            switch (calculation)
            {
                case SemiProbabilisticPipingCalculationScenario _:
                    return PipingCalculationConfigurationType.SemiProbabilistic;
                case ProbabilisticPipingCalculationScenario _:
                    return PipingCalculationConfigurationType.Probabilistic;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void ToSemiProbabilisticConfiguration(PipingCalculationConfiguration calculationConfiguration,
                                                             SemiProbabilisticPipingInput input)
        {
            if (input.UseAssessmentLevelManualInput)
            {
                calculationConfiguration.AssessmentLevel = input.AssessmentLevel;
            }
            else if (input.HydraulicBoundaryLocation != null)
            {
                calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation.Name;
            }
        }

        private static void ToProbabilisticConfiguration(PipingCalculationConfiguration calculationConfiguration,
                                                         ProbabilisticPipingInput input)
        {
            if (input.HydraulicBoundaryLocation != null)
            {
                calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation.Name;
            }

            calculationConfiguration.ShouldProfileSpecificIllustrationPointsBeCalculated = input.ShouldProfileSpecificIllustrationPointsBeCalculated;
            calculationConfiguration.ShouldSectionSpecificIllustrationPointsBeCalculated = input.ShouldSectionSpecificIllustrationPointsBeCalculated;
        }
    }
}