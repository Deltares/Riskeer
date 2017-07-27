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
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.MacroStabilityInwards.Data;
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
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                Name = calculationConfiguration.Name
            };

            if (TrySetHydraulicBoundaryData(calculationConfiguration, calculation)
                && TrySetSurfaceLine(calculationConfiguration, calculation)
                && TrySetStochasticSoilModel(calculationConfiguration, calculation)
                && TrySetStochasticSoilProfile(calculationConfiguration, calculation)
                && TrySetScenarioParameters(calculationConfiguration.Scenario, calculation))
            {
                return calculation;
            }
            return null;
        }

        /// <summary>
        /// Assigns the hydraulic boundary location or the assessment level that is set manually.
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
        /// Assigns the surface line.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="macroStabilityInwardsCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="MacroStabilityInwardsSurfaceLine"/>
        /// set which is not available in <see cref="MacroStabilityInwardsFailureMechanism.SurfaceLines"/>, <c>true</c> otherwise.</returns>
        private bool TrySetSurfaceLine(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                       MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation)
        {
            if (calculationConfiguration.SurfaceLineName != null)
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                                        .FirstOrDefault(sl => sl.Name == calculationConfiguration.SurfaceLineName);

                if (surfaceLine == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist,
                                                          calculationConfiguration.SurfaceLineName),
                                                      macroStabilityInwardsCalculation.Name);
                    return false;
                }

                macroStabilityInwardsCalculation.InputParameters.SurfaceLine = surfaceLine;
            }
            return true;
        }

        /// <summary>
        /// Assigns the stochastic soil model.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="macroStabilityInwardsCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when
        /// <list type="bullet">
        /// <item>the <paramref name="calculationConfiguration"/> has a <see cref="StochasticSoilModel"/> set
        /// which is not available in the failure mechanism.</item>
        /// <item>The <see cref="StochasticSoilModel"/> does not intersect with the <see cref="MacroStabilityInwardsSurfaceLine"/>
        /// when this is set.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilModel(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                               MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation)
        {
            if (calculationConfiguration.StochasticSoilModelName != null)
            {
                StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                .FirstOrDefault(ssm => ssm.Name == calculationConfiguration.StochasticSoilModelName);

                if (soilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist,
                                                          calculationConfiguration.StochasticSoilModelName),
                                                      macroStabilityInwardsCalculation.Name);
                    return false;
                }

                if (macroStabilityInwardsCalculation.InputParameters.SurfaceLine != null
                    && !soilModel.IntersectsWithSurfaceLineGeometry(macroStabilityInwardsCalculation.InputParameters.SurfaceLine))
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1,
                                                          calculationConfiguration.StochasticSoilModelName,
                                                          calculationConfiguration.SurfaceLineName),
                                                      macroStabilityInwardsCalculation.Name);
                    return false;
                }

                macroStabilityInwardsCalculation.InputParameters.StochasticSoilModel = soilModel;
            }
            return true;
        }

        /// <summary>
        /// Assigns the stochastic soil profile.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="macroStabilityInwardsCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has:
        /// <list type="bullet">
        /// <item>a <see cref="StochasticSoilProfile"/> set but no <see cref="StochasticSoilModel"/> is specified;</item>
        /// <item>a <see cref="StochasticSoilProfile"/> set which is not available in the <see cref="StochasticSoilModel"/>.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilProfile(MacroStabilityInwardsCalculationConfiguration calculationConfiguration,
                                                 MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculation)
        {
            if (calculationConfiguration.StochasticSoilProfileName != null)
            {
                if (macroStabilityInwardsCalculation.InputParameters.StochasticSoilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0,
                                                          calculationConfiguration.StochasticSoilProfileName),
                                                      macroStabilityInwardsCalculation.Name);
                    return false;
                }

                StochasticSoilProfile soilProfile = macroStabilityInwardsCalculation.InputParameters
                                                                                    .StochasticSoilModel
                                                                                    .StochasticSoilProfiles
                                                                                    .FirstOrDefault(ssp => ssp.SoilProfile.Name == calculationConfiguration.StochasticSoilProfileName);

                if (soilProfile == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.MacroStabilityInwardsCalculationConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1,
                                                          calculationConfiguration.StochasticSoilProfileName,
                                                          calculationConfiguration.StochasticSoilModelName),
                                                      macroStabilityInwardsCalculation.Name);
                    return false;
                }

                macroStabilityInwardsCalculation.InputParameters.StochasticSoilProfile = soilProfile;
            }
            return true;
        }
    }
}