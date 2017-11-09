﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Imports a macro stability inwards calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationImporter
        : CalculationConfigurationImporter<
            MacroStabilityInwardsCalculationConfigurationReader,
            MacroStabilityInwardsCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly MacroStabilityInwardsFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="availableHydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="failureMechanism">The failure mechanism used to check
        /// if the imported objects contain the right data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public MacroStabilityInwardsCalculationConfigurationImporter(string xmlFilePath,
                                                                     CalculationGroup importTarget,
                                                                     IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations,
                                                                     MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(xmlFilePath, importTarget)
        {
            if (availableHydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(availableHydraulicBoundaryLocations));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.availableHydraulicBoundaryLocations = availableHydraulicBoundaryLocations;
            this.failureMechanism = failureMechanism;
        }

        protected override MacroStabilityInwardsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new MacroStabilityInwardsCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(MacroStabilityInwardsCalculationConfiguration calculationConfiguration)
        {
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = calculationConfiguration.Name
            };

            if (TrySetHydraulicBoundaryData(calculationConfiguration, calculation)
                && TrySetSurfaceLine(calculationConfiguration, calculation)
                && TrySetStochasticSoilModel(calculationConfiguration, calculation)
                && TrySetStochasticSoilProfile(calculationConfiguration, calculation)
                && TrySetScenarioParameters(calculationConfiguration.Scenario, calculation)
                && TrySetTangentLineZTopBottom(calculationConfiguration, calculation)
                && TrySetTangentLineNumber(calculationConfiguration, calculation)
                && TrySetGrids(calculationConfiguration, calculation))
            {
                SetSimpleProperties(calculationConfiguration, calculation.InputParameters);

                SetDikeSoilScenario(calculationConfiguration, calculation.InputParameters);
                SetGridDeterminationType(calculationConfiguration, calculation.InputParameters);
                SetTangentLineDeterminationType(calculationConfiguration, calculation.InputParameters);
                SetMacroStabilityInwardsLocationInput(calculationConfiguration.LocationInputDaily, calculation.InputParameters.LocationInputDaily);
                SetMacroStabilityInwardsLocationInputExtreme(calculationConfiguration.LocationInputExtreme, calculation.InputParameters.LocationInputExtreme);
                return calculation;
            }
            return null;
        }

        /// <summary>
        /// Tries to assign the hydraulic boundary location or the assessment level that is set manually.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="macroStabilityInwardsCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TrySetHydraulicBoundaryData(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                                 MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation)
        {
            HydraulicBoundaryLocation location;

            bool locationRead = TryReadHydraulicBoundaryLocation(calculationConfiguration.HydraulicBoundaryLocationName,
                                                                 calculationConfiguration.Name,
                                                                 availableHydraulicBoundaryLocations,
                                                                 out location);

            if (!locationRead)
            {
                return false;
            }

            if (location != null)
            {
                macroStabilityInwardsCalculation.InputParameters.HydraulicBoundaryLocation = location;
            }
            else if (calculationConfiguration.AssessmentLevel.HasValue)
            {
                macroStabilityInwardsCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                macroStabilityInwardsCalculation.InputParameters.AssessmentLevel = (RoundedDouble) calculationConfiguration.AssessmentLevel.Value;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the surface line.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="MacroStabilityInwardsSurfaceLine"/>
        /// set which is not available in <see cref="MacroStabilityInwardsFailureMechanism.SurfaceLines"/>, <c>true</c> otherwise.</returns>
        private bool TrySetSurfaceLine(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                       MacroStabilityInwardsCalculationScenario calculation)
        {
            if (calculationConfiguration.SurfaceLineName == null)
            {
                return true;
            }

            MacroStabilityInwardsSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                           .FirstOrDefault(sl => sl.Name == calculationConfiguration.SurfaceLineName);

            if (surfaceLine == null)
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist,
                                                      calculationConfiguration.SurfaceLineName),
                                                  calculation.Name);
                return false;
            }

            calculation.InputParameters.SurfaceLine = surfaceLine;
            return true;
        }

        /// <summary>
        /// Tries to assign the stochastic soil profile.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has:
        /// <list type="bullet">
        /// <item>a <see cref="MacroStabilityInwardsStochasticSoilProfile"/> set but no <see cref="MacroStabilityInwardsStochasticSoilModel"/> 
        /// is specified;</item>
        /// <item>a <see cref="MacroStabilityInwardsStochasticSoilProfile"/> set which is not
        ///  available in the <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilProfile(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                                 MacroStabilityInwardsCalculationScenario calculation)
        {
            if (calculationConfiguration.StochasticSoilProfileName == null)
            {
                return true;
            }

            if (calculation.InputParameters.StochasticSoilModel == null)
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0,
                                                      calculationConfiguration.StochasticSoilProfileName),
                                                  calculation.Name);
                return false;
            }

            MacroStabilityInwardsStochasticSoilProfile soilProfile = calculation.InputParameters
                                                                                .StochasticSoilModel
                                                                                .StochasticSoilProfiles
                                                                                .FirstOrDefault(ssp => ssp.SoilProfile.Name == calculationConfiguration.StochasticSoilProfileName);

            if (soilProfile == null)
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1,
                                                      calculationConfiguration.StochasticSoilProfileName,
                                                      calculationConfiguration.StochasticSoilModelName),
                                                  calculation.Name);
                return false;
            }

            calculation.InputParameters.StochasticSoilProfile = soilProfile;
            return true;
        }

        /// <summary>
        /// Tries to assign the tangent line Z top and tangent line Z bottom parameters to the <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if no tangent line z top and tangent line z bottom was given, or when 
        /// tangent line z top and tangent line z bottom are set to the <paramref name="calculation"/>, 
        /// <c>false</c> otherwise.</returns>
        private bool TrySetTangentLineZTopBottom(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                                 MacroStabilityInwardsCalculationScenario calculation)
        {
            bool hasTangentLineZTop = calculationConfiguration.TangentLineZTop.HasValue;
            bool hasTangentLineZBottom = calculationConfiguration.TangentLineZBottom.HasValue;

            if (!hasTangentLineZTop && !hasTangentLineZBottom)
            {
                return true;
            }

            RoundedDouble tangentLineZTop = hasTangentLineZTop
                                                ? (RoundedDouble) calculationConfiguration.TangentLineZTop.Value
                                                : RoundedDouble.NaN;
            RoundedDouble tangentLineZBottom = hasTangentLineZBottom
                                                   ? (RoundedDouble) calculationConfiguration.TangentLineZBottom.Value
                                                   : RoundedDouble.NaN;

            MacroStabilityInwardsInput input = calculation.InputParameters;
            try
            {
                input.TangentLineZTop = tangentLineZTop;
                input.TangentLineZBottom = tangentLineZBottom;
            }
            catch (ArgumentException e)
            {
                Log.LogCalculationConversionError(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetTangentLineZTopBottom_Combination_of_TangentLineZTop_0_and_TangentLineZBottom_1_invalid_Reason_2,
                                                                tangentLineZTop.ToPrecision(input.TangentLineZTop.NumberOfDecimalPlaces),
                                                                tangentLineZBottom.ToPrecision(input.TangentLineZTop.NumberOfDecimalPlaces),
                                                                e.Message),
                                                  calculation.Name);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the tangent line number.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> when the tangent line number is set to the <paramref name="calculation"/>,
        /// <c>false</c> otherwise.</returns>
        private bool TrySetTangentLineNumber(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                             MacroStabilityInwardsCalculationScenario calculation)
        {
            if (!calculationConfiguration.TangentLineNumber.HasValue)
            {
                return true;
            }

            int tangentLineNumber = calculationConfiguration.TangentLineNumber.Value;

            try
            {
                calculation.InputParameters.TangentLineNumber = tangentLineNumber;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.LogOutOfRangeException(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetTangentLineNumber_TangentLineNumber_0_invalid,
                                                         tangentLineNumber),
                                           calculation.Name,
                                           e);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the read grids to the <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculationConfiguration">The configuration read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if no grids where given, or set to the <paramref name="calculation"/>, 
        /// <c>false</c> otherwise.</returns>
        private bool TrySetGrids(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                 MacroStabilityInwardsCalculationScenario calculation)
        {
            return TrySetGrid(calculationConfiguration.LeftGrid,
                              calculation.InputParameters.LeftGrid,
                              calculation.Name)
                   && TrySetGrid(calculationConfiguration.RightGrid,
                                 calculation.InputParameters.RightGrid,
                                 calculation.Name);
        }

        /// <summary>
        /// Tries to assign the read grid to the <paramref name="grid"/>.
        /// </summary>
        /// <param name="gridConfiguration">The grid configuration read from the imported file.</param>
        /// <param name="grid">The grid to configure.</param>
        /// <param name="calculationName">The name of the read calculation.</param>
        /// <returns><c>true</c> if no grid configuration where given, or set to the <paramref name="grid"/>
        /// , <c>false</c> otherwise.</returns>
        private bool TrySetGrid(MacroStabilityInwardsGridConfiguration gridConfiguration,
                                MacroStabilityInwardsGrid grid,
                                string calculationName)
        {
            if (gridConfiguration == null)
            {
                return true;
            }

            return TrySetGridZTopZBottom(gridConfiguration, grid, calculationName)
                   && TrySetGridXLeftXRight(gridConfiguration, grid, calculationName)
                   && TrySetNumberOfHorizontalPoints(gridConfiguration, grid, calculationName)
                   && TrySetNumberOfVerticalPoints(gridConfiguration, grid, calculationName);
        }

        /// <summary>
        /// Tries to assign the grid Z top and Z bottom parameters to the <paramref name="grid"/>.
        /// </summary>
        /// <param name="gridConfiguration">The grid configuration read from the imported file.</param>
        /// <param name="grid">The calculation grid to configure.</param>
        /// <param name="calculationName">The name of the read calculation.</param>
        /// <returns><c>true</c> if no z top and z bottom was given, or when z top and z bottom 
        /// are set to the <paramref name="grid"/>, <c>false</c> otherwise.</returns>
        private bool TrySetGridZTopZBottom(MacroStabilityInwardsGridConfiguration gridConfiguration,
                                           MacroStabilityInwardsGrid grid,
                                           string calculationName)
        {
            bool hasZTopValue = gridConfiguration.ZTop.HasValue;
            bool hasZBottomValue = gridConfiguration.ZBottom.HasValue;
            if (!hasZTopValue && !hasZBottomValue)
            {
                return true;
            }

            RoundedDouble zTop = hasZTopValue
                                     ? (RoundedDouble) gridConfiguration.ZTop.Value
                                     : RoundedDouble.NaN;

            RoundedDouble zBottom = hasZBottomValue
                                        ? (RoundedDouble) gridConfiguration.ZBottom.Value
                                        : RoundedDouble.NaN;
            try
            {
                grid.ZTop = zTop;
                grid.ZBottom = zBottom;
            }
            catch (ArgumentException e)
            {
                Log.LogCalculationConversionError(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetGridZTopZBottom_Combination_of_ZTop_0_and_ZBottom_1_invalid_Reason_2,
                                                                zTop.ToPrecision(grid.ZTop.NumberOfDecimalPlaces),
                                                                zBottom.ToPrecision(grid.ZBottom.NumberOfDecimalPlaces),
                                                                e.Message),
                                                  calculationName);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the grid x left and x right parameters to the <paramref name="grid"/>.
        /// </summary>
        /// <param name="gridConfiguration">The grid configuration read from the imported file.</param>
        /// <param name="grid">The calculation grid to configure.</param>
        /// <param name="calculationName">The name of the read calculation.</param>
        /// <returns><c>true</c> if no x left and x right was given, or when x left and x right
        /// are set to the <paramref name="grid"/>, <c>false</c> otherwise.</returns>
        private bool TrySetGridXLeftXRight(MacroStabilityInwardsGridConfiguration gridConfiguration,
                                           MacroStabilityInwardsGrid grid,
                                           string calculationName)
        {
            bool hasXLeftValue = gridConfiguration.XLeft.HasValue;
            bool hasXRightValue = gridConfiguration.XRight.HasValue;
            if (!hasXLeftValue && !hasXRightValue)
            {
                return true;
            }

            RoundedDouble xLeft = hasXLeftValue
                                      ? (RoundedDouble) gridConfiguration.XLeft.Value
                                      : RoundedDouble.NaN;

            RoundedDouble xRight = hasXRightValue
                                       ? (RoundedDouble) gridConfiguration.XRight.Value
                                       : RoundedDouble.NaN;
            try
            {
                grid.XLeft = xLeft;
                grid.XRight = xRight;
            }
            catch (ArgumentException e)
            {
                Log.LogCalculationConversionError(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetGridXLeftXRight_Combination_of_XLeft_0_and_XRight_1_invalid_Reason_2,
                                                                xLeft.ToPrecision(grid.XLeft.NumberOfDecimalPlaces),
                                                                xRight.ToPrecision(grid.XRight.NumberOfDecimalPlaces),
                                                                e.Message),
                                                  calculationName);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the number of horizontal points to the <paramref name="grid"/>.
        /// </summary>
        /// <param name="gridConfiguration">The grid configuration read from the imported file.</param>
        /// <param name="grid">The calculation grid to configure.</param>
        /// <param name="calculationName">The name of the read calculation.</param>
        /// <returns><c>true</c> if no number of horizontal points was given, or when number of 
        /// horizontal points is set to the <paramref name="grid"/>, <c>false</c> otherwise.</returns>
        private bool TrySetNumberOfHorizontalPoints(MacroStabilityInwardsGridConfiguration gridConfiguration,
                                                    MacroStabilityInwardsGrid grid,
                                                    string calculationName)
        {
            if (!gridConfiguration.NumberOfHorizontalPoints.HasValue)
            {
                return true;
            }

            int numberOfHorizontalPoints = gridConfiguration.NumberOfHorizontalPoints.Value;

            try
            {
                grid.NumberOfHorizontalPoints = numberOfHorizontalPoints;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.LogOutOfRangeException(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetNumberOfHorizontalPoints_NumberOfHorizontalPoints_0_invalid,
                                                         numberOfHorizontalPoints),
                                           calculationName,
                                           e);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the number of vertical points to the <paramref name="grid"/>.
        /// </summary>
        /// <param name="gridConfiguration">The grid configuration read from the imported file.</param>
        /// <param name="grid">The calculation grid to configure.</param>
        /// <param name="calculationName">The name of the read calculation.</param>
        /// <returns><c>true</c> if no number of vertical points was given, or when number of 
        /// vertical points is set to the <paramref name="grid"/>, <c>false</c> otherwise.</returns>
        private bool TrySetNumberOfVerticalPoints(MacroStabilityInwardsGridConfiguration gridConfiguration,
                                                  MacroStabilityInwardsGrid grid,
                                                  string calculationName)
        {
            if (!gridConfiguration.NumberOfVerticalPoints.HasValue)
            {
                return true;
            }

            int numberOfVerticalPoints = gridConfiguration.NumberOfVerticalPoints.Value;

            try
            {
                grid.NumberOfVerticalPoints = numberOfVerticalPoints;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.LogOutOfRangeException(string.Format(Resources.MacroStabilityInwardsCalculationConfigurationImporter_TrySetNumberOfVerticalPoints_NumberOfVerticalPoints_0_invalid,
                                                         numberOfVerticalPoints),
                                           calculationName,
                                           e);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to assign the stochastic soil model.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when
        /// <list type="bullet">
        /// <item>the <paramref name="calculationConfiguration"/> has a <see cref="MacroStabilityInwardsStochasticSoilModel"/> 
        /// set which is not available in the failure mechanism.</item>
        /// <item>The <see cref="MacroStabilityInwardsStochasticSoilModel"/> does not intersect 
        /// with the <see cref="MacroStabilityInwardsSurfaceLine"/>
        /// when this is set.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilModel(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                               MacroStabilityInwardsCalculationScenario calculation)
        {
            if (calculationConfiguration.StochasticSoilModelName == null)
            {
                return true;
            }

            MacroStabilityInwardsStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                                 .FirstOrDefault(ssm => ssm.Name == calculationConfiguration.StochasticSoilModelName);

            if (soilModel == null)
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist,
                                                      calculationConfiguration.StochasticSoilModelName),
                                                  calculation.Name);
                return false;
            }

            if (calculation.InputParameters.SurfaceLine != null
                && !soilModel.IntersectsWithSurfaceLineGeometry(calculation.InputParameters.SurfaceLine))
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1,
                                                      calculationConfiguration.StochasticSoilModelName,
                                                      calculationConfiguration.SurfaceLineName),
                                                  calculation.Name);
                return false;
            }

            calculation.InputParameters.StochasticSoilModel = soilModel;
            return true;
        }

        /// <summary>
        /// Assigns simple properties of <see cref="MacroStabilityInwardsInput"/>.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="input">The calculation input to configure.</param>
        private static void SetSimpleProperties(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                                MacroStabilityInwardsInput input)
        {
            if (calculationConfiguration.WaterLevelRiverAverage.HasValue)
            {
                input.WaterLevelRiverAverage = (RoundedDouble) calculationConfiguration.WaterLevelRiverAverage.Value;
            }

            if (calculationConfiguration.DrainageConstructionPresent.HasValue)
            {
                input.DrainageConstructionPresent = calculationConfiguration.DrainageConstructionPresent.Value;
            }
            if (calculationConfiguration.XCoordinateDrainageConstruction.HasValue)
            {
                input.XCoordinateDrainageConstruction = (RoundedDouble) calculationConfiguration.XCoordinateDrainageConstruction.Value;
            }
            if (calculationConfiguration.ZCoordinateDrainageConstruction.HasValue)
            {
                input.ZCoordinateDrainageConstruction = (RoundedDouble) calculationConfiguration.ZCoordinateDrainageConstruction.Value;
            }

            if (calculationConfiguration.MinimumLevelPhreaticLineAtDikeTopRiver.HasValue)
            {
                input.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) calculationConfiguration.MinimumLevelPhreaticLineAtDikeTopRiver.Value;
            }
            if (calculationConfiguration.MinimumLevelPhreaticLineAtDikeTopPolder.HasValue)
            {
                input.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) calculationConfiguration.MinimumLevelPhreaticLineAtDikeTopPolder.Value;
            }

            if (calculationConfiguration.AdjustPhreaticLine3And4ForUplift.HasValue)
            {
                input.AdjustPhreaticLine3And4ForUplift = calculationConfiguration.AdjustPhreaticLine3And4ForUplift.Value;
            }

            if (calculationConfiguration.PiezometricHeadPhreaticLine2Outwards.HasValue)
            {
                input.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) calculationConfiguration.PiezometricHeadPhreaticLine2Outwards.Value;
            }
            if (calculationConfiguration.PiezometricHeadPhreaticLine2Inwards.HasValue)
            {
                input.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) calculationConfiguration.PiezometricHeadPhreaticLine2Inwards.Value;
            }

            if (calculationConfiguration.LeakageLengthOutwardsPhreaticLine3.HasValue)
            {
                input.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) calculationConfiguration.LeakageLengthOutwardsPhreaticLine3.Value;
            }
            if (calculationConfiguration.LeakageLengthInwardsPhreaticLine3.HasValue)
            {
                input.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) calculationConfiguration.LeakageLengthInwardsPhreaticLine3.Value;
            }

            if (calculationConfiguration.LeakageLengthOutwardsPhreaticLine4.HasValue)
            {
                input.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) calculationConfiguration.LeakageLengthOutwardsPhreaticLine4.Value;
            }
            if (calculationConfiguration.LeakageLengthInwardsPhreaticLine4.HasValue)
            {
                input.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) calculationConfiguration.LeakageLengthInwardsPhreaticLine4.Value;
            }

            if (calculationConfiguration.SlipPlaneMinimumDepth.HasValue)
            {
                input.SlipPlaneMinimumDepth = (RoundedDouble) calculationConfiguration.SlipPlaneMinimumDepth.Value;
            }
            if (calculationConfiguration.SlipPlaneMinimumLength.HasValue)
            {
                input.SlipPlaneMinimumLength = (RoundedDouble) calculationConfiguration.SlipPlaneMinimumLength.Value;
            }
            if (calculationConfiguration.MaximumSliceWidth.HasValue)
            {
                input.MaximumSliceWidth = (RoundedDouble) calculationConfiguration.MaximumSliceWidth.Value;
            }

            if (calculationConfiguration.CreateZones.HasValue)
            {
                input.CreateZones = calculationConfiguration.CreateZones.Value;
            }

            if (calculationConfiguration.MoveGrid.HasValue)
            {
                input.MoveGrid = calculationConfiguration.MoveGrid.Value;
            }
        }

        /// <summary>
        /// Assigns the dike soil scenario.
        /// </summary>
        /// <param name="configuration">The read calculation read.</param>
        /// <param name="input">The calculation input to configure.</param>
        private static void SetDikeSoilScenario(MacroStabilityInwardsCalculationConfiguration configuration,
                                                MacroStabilityInwardsInput input)
        {
            if (!configuration.DikeSoilScenario.HasValue)
            {
                return;
            }

            input.DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) new ConfigurationDikeSoilScenarioTypeConverter()
                .ConvertTo(configuration.DikeSoilScenario.Value, typeof(MacroStabilityInwardsDikeSoilScenario));
        }

        /// <summary>
        /// Assigns the grid determination type.
        /// </summary>
        /// <param name="configuration">The read calculation read.</param>
        /// <param name="input">The calculation input to configure.</param>
        private static void SetGridDeterminationType(MacroStabilityInwardsCalculationConfiguration configuration,
                                                     MacroStabilityInwardsInput input)
        {
            if (!configuration.GridDeterminationType.HasValue)
            {
                return;
            }

            input.GridDeterminationType = (MacroStabilityInwardsGridDeterminationType) new ConfigurationGridDeterminationTypeConverter()
                .ConvertTo(configuration.GridDeterminationType.Value, typeof(MacroStabilityInwardsGridDeterminationType));
        }

        /// <summary>
        /// Assigns the tangent line determination type.
        /// </summary>
        /// <param name="configuration">The read calculation read.</param>
        /// <param name="input">The calculation input to configure.</param>
        private static void SetTangentLineDeterminationType(MacroStabilityInwardsCalculationConfiguration configuration,
                                                            MacroStabilityInwardsInput input)
        {
            if (!configuration.TangentLineDeterminationType.HasValue)
            {
                return;
            }

            input.TangentLineDeterminationType = (MacroStabilityInwardsTangentLineDeterminationType)
                new ConfigurationTangentLineDeterminationTypeConverter()
                    .ConvertTo(configuration.TangentLineDeterminationType.Value, typeof(MacroStabilityInwardsTangentLineDeterminationType));
        }

        /// <summary>
        /// Assigns the read location input configuration to the of <paramref name="locationInput"/>.
        /// </summary>
        /// <param name="configurationLocationInput">The location input configuration read from the imported file.</param>
        /// <param name="locationInput">The location input to configure.</param>
        private static void SetMacroStabilityInwardsLocationInput(MacroStabilityInwardsLocationInputConfiguration configurationLocationInput,
                                                                  IMacroStabilityInwardsLocationInput locationInput)
        {
            if (configurationLocationInput == null)
            {
                return;
            }

            if (configurationLocationInput.WaterLevelPolder.HasValue)
            {
                locationInput.WaterLevelPolder = (RoundedDouble) configurationLocationInput.WaterLevelPolder.Value;
            }
            if (configurationLocationInput.UseDefaultOffsets.HasValue)
            {
                locationInput.UseDefaultOffsets = configurationLocationInput.UseDefaultOffsets.Value;
            }
            if (configurationLocationInput.PhreaticLineOffsetBelowDikeTopAtRiver.HasValue)
            {
                locationInput.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) configurationLocationInput.PhreaticLineOffsetBelowDikeTopAtRiver.Value;
            }
            if (configurationLocationInput.PhreaticLineOffsetBelowDikeTopAtPolder.HasValue)
            {
                locationInput.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) configurationLocationInput.PhreaticLineOffsetBelowDikeTopAtPolder.Value;
            }
            if (configurationLocationInput.PhreaticLineOffsetBelowShoulderBaseInside.HasValue)
            {
                locationInput.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) configurationLocationInput.PhreaticLineOffsetBelowShoulderBaseInside.Value;
            }
            if (configurationLocationInput.PhreaticLineOffsetBelowDikeToeAtPolder.HasValue)
            {
                locationInput.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) configurationLocationInput.PhreaticLineOffsetBelowDikeToeAtPolder.Value;
            }
        }

        /// <summary>
        /// Assigns the read location input configuration to the of <paramref name="locationInput"/>.
        /// </summary>
        /// <param name="configurationLocationInput">The location input configuration read from the imported file.</param>
        /// <param name="locationInput">The location input to configure.</param>
        private static void SetMacroStabilityInwardsLocationInputExtreme(MacroStabilityInwardsLocationInputExtremeConfiguration configurationLocationInput,
                                                                         IMacroStabilityInwardsLocationInputExtreme locationInput)
        {
            if (configurationLocationInput == null)
            {
                return;
            }

            if (configurationLocationInput.PenetrationLength.HasValue)
            {
                locationInput.PenetrationLength = (RoundedDouble) configurationLocationInput.PenetrationLength.Value;
            }

            SetMacroStabilityInwardsLocationInput(configurationLocationInput, locationInput);
        }
    }
}