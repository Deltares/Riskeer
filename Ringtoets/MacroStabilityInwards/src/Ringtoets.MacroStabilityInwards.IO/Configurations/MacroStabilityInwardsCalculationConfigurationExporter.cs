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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.Primitives;

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

            var calculationConfiguration = new MacroStabilityInwardsCalculationConfiguration(calculation.Name)
            {
                WaterLevelRiverAverage = input.WaterLevelRiverAverage,
                DrainageConstructionPresent = input.DrainageConstructionPresent,
                XCoordinateDrainageConstruction = input.XCoordinateDrainageConstruction,
                ZCoordinateDrainageConstruction = input.ZCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopPolder = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                MinimumLevelPhreaticLineAtDikeTopRiver = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                AdjustPhreaticLine3And4ForUplift = input.AdjustPhreaticLine3And4ForUplift,
                PiezometricHeadPhreaticLine2Inwards = input.PiezometricHeadPhreaticLine2Inwards,
                PiezometricHeadPhreaticLine2Outwards = input.PiezometricHeadPhreaticLine2Outwards,
                LeakageLengthInwardsPhreaticLine3 = input.LeakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine3 = input.LeakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine4 = input.LeakageLengthInwardsPhreaticLine4,
                LeakageLengthOutwardsPhreaticLine4 = input.LeakageLengthOutwardsPhreaticLine4,
                LocationInputDaily = input.LocationInputDaily.ToMacroStabilityInwardsLocationInputConfiguration(),
                LocationInputExtreme = input.LocationInputExtreme.ToMacroStabilityInwardsLocationInputExtremeConfiguration(),
                Scenario = calculation.ToScenarioConfiguration(),
                SlipPlaneMinimumDepth = input.SlipPlaneMinimumDepth,
                SlipPlaneMinimumLength = input.SlipPlaneMinimumLength,
                MaximumSliceWidth = input.MaximumSliceWidth,
                CreateZones = input.CreateZones,
                ZoneBoundaryLeft = input.ZoneBoundaryLeft,
                ZoneBoundaryRight = input.ZoneBoundaryRight,
                TangentLineZTop = input.TangentLineZTop,
                TangentLineZBottom = input.TangentLineZBottom,
                TangentLineNumber = input.TangentLineNumber,
                MoveGrid = input.MoveGrid,
                LeftGrid = input.LeftGrid.ToMacroStabilityInwardsGridConfiguration(),
                RightGrid = input.RightGrid.ToMacroStabilityInwardsGridConfiguration()
            };

            if (input.UseAssessmentLevelManualInput)
            {
                calculationConfiguration.AssessmentLevel = input.AssessmentLevel;
            }
            else if (input.HydraulicBoundaryLocation != null)
            {
                calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation.Name;
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

            if (Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), input.DikeSoilScenario))
            {
                calculationConfiguration.DikeSoilScenario = (ConfigurationDikeSoilScenario?)
                    new ConfigurationDikeSoilScenarioTypeConverter().ConvertFrom(input.DikeSoilScenario);
            }

            if (Enum.IsDefined(typeof(MacroStabilityInwardsGridDeterminationType), input.GridDeterminationType))
            {
                calculationConfiguration.GridDeterminationType = (ConfigurationGridDeterminationType?)
                    new ConfigurationGridDeterminationTypeConverter().ConvertFrom(input.GridDeterminationType);
            }

            if (Enum.IsDefined(typeof(MacroStabilityInwardsTangentLineDeterminationType), input.TangentLineDeterminationType))
            {
                calculationConfiguration.TangentLineDeterminationType = (ConfigurationTangentLineDeterminationType?)
                    new ConfigurationTangentLineDeterminationTypeConverter().ConvertFrom(input.TangentLineDeterminationType);
            }

            if (Enum.IsDefined(typeof(MacroStabilityInwardsZoningBoundariesDeterminationType), input.ZoningBoundariesDeterminationType))
            {
                calculationConfiguration.ZoningBoundariesDeterminationType = (ConfigurationZoningBoundariesDeterminationType?)
                    new ConfigurationZoningBoundariesDeterminationTypeConverter().ConvertFrom(input.ZoningBoundariesDeterminationType);
            }

            return calculationConfiguration;
        }
    }
}