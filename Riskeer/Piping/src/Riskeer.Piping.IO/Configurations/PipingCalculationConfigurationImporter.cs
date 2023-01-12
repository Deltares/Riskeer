﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.IO.Properties;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.IO.Configurations
{
    /// <summary>
    /// Imports a piping calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    public class PipingCalculationConfigurationImporter
        : CalculationConfigurationImporter<PipingCalculationConfigurationReader,
            PipingCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="availableHydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="failureMechanism">The piping failure mechanism used to check
        /// if the imported objects contain the right data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public PipingCalculationConfigurationImporter(string xmlFilePath,
                                                      CalculationGroup importTarget,
                                                      IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations,
                                                      PipingFailureMechanism failureMechanism)
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

        protected override PipingCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new PipingCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(PipingCalculationConfiguration readCalculation)
        {
            switch (readCalculation.CalculationType)
            {
                case PipingCalculationConfigurationType.SemiProbabilistic:
                    return ParseReadCalculationAsSemiProbabilistic(readCalculation);
                case PipingCalculationConfigurationType.Probabilistic:
                    return ParseReadCalculationAsProbabilistic(readCalculation);
                default:
                    return null;
            }
        }

        #region SemiProbabilistic

        /// <summary>
        /// Parses a semi-probabilistic calculation from the provided <paramref name="readCalculation"/>.
        /// </summary>
        /// <param name="readCalculation">The calculation read from XML.</param>
        /// <returns>The parsed <see cref="SemiProbabilisticPipingCalculationScenario"/>,
        /// or <c>null</c> when parsing failed.</returns>
        private ICalculation ParseReadCalculationAsSemiProbabilistic(PipingCalculationConfiguration readCalculation)
        {
            var pipingCalculation = new SemiProbabilisticPipingCalculationScenario
            {
                Name = readCalculation.Name
            };

            if (TrySetSemiProbabilisticHydraulicBoundaryData(readCalculation, pipingCalculation)
                && TrySetGenericData(readCalculation, pipingCalculation))
            {
                return pipingCalculation;
            }

            return null;
        }

        /// <summary>
        /// Assigns the hydraulic boundary location or the assessment level that is set manually.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The semi-probabilistic calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TrySetSemiProbabilisticHydraulicBoundaryData(PipingCalculationConfiguration calculationConfiguration,
                                                                  SemiProbabilisticPipingCalculationScenario pipingCalculation)
        {
            bool locationRead = TryReadHydraulicBoundaryLocation(calculationConfiguration.HydraulicBoundaryLocationName,
                                                                 calculationConfiguration.Name,
                                                                 availableHydraulicBoundaryLocations,
                                                                 out HydraulicBoundaryLocation location);

            if (!locationRead)
            {
                return false;
            }

            if (calculationConfiguration.AssessmentLevel.HasValue)
            {
                pipingCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                pipingCalculation.InputParameters.AssessmentLevel = (RoundedDouble) calculationConfiguration.AssessmentLevel.Value;
            }
            else
            {
                SetLocation(pipingCalculation, location);
            }

            return true;
        }

        #endregion

        #region Probabilistic

        /// <summary>
        /// Parses a probabilistic calculation from the provided <paramref name="readCalculation"/>.
        /// </summary>
        /// <param name="readCalculation">The calculation read from XML.</param>
        /// <returns>The parsed <see cref="ProbabilisticPipingCalculationScenario"/>,
        /// or <c>null</c> when parsing failed.</returns>
        private ICalculation ParseReadCalculationAsProbabilistic(PipingCalculationConfiguration readCalculation)
        {
            var pipingCalculation = new ProbabilisticPipingCalculationScenario
            {
                Name = readCalculation.Name
            };

            if (TrySetProbabilisticHydraulicBoundaryData(readCalculation, pipingCalculation)
                && TrySetGenericData(readCalculation, pipingCalculation))
            {
                SetShouldIllustrationPointsBeCalculated(readCalculation, pipingCalculation);
                return pipingCalculation;
            }

            return null;
        }

        /// <summary>
        /// Assigns the hydraulic boundary location.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The probabilistic calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TrySetProbabilisticHydraulicBoundaryData(PipingCalculationConfiguration calculationConfiguration,
                                                              ProbabilisticPipingCalculationScenario pipingCalculation)
        {
            bool locationRead = TryReadHydraulicBoundaryLocation(calculationConfiguration.HydraulicBoundaryLocationName,
                                                                 calculationConfiguration.Name,
                                                                 availableHydraulicBoundaryLocations,
                                                                 out HydraulicBoundaryLocation location);

            if (!locationRead)
            {
                return false;
            }

            SetLocation(pipingCalculation, location);

            return true;
        }

        /// <summary>
        /// Assigns the properties defining whether the illustration points need to be read for various calculations.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The probabilistic calculation to configure.</param>
        private static void SetShouldIllustrationPointsBeCalculated(PipingCalculationConfiguration calculationConfiguration,
                                                                    ProbabilisticPipingCalculationScenario pipingCalculation)
        {
            if (calculationConfiguration.ShouldProfileSpecificIllustrationPointsBeCalculated.HasValue)
            {
                pipingCalculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = calculationConfiguration.ShouldProfileSpecificIllustrationPointsBeCalculated.Value;
            }

            if (calculationConfiguration.ShouldSectionSpecificIllustrationPointsBeCalculated.HasValue)
            {
                pipingCalculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = calculationConfiguration.ShouldSectionSpecificIllustrationPointsBeCalculated.Value;
            }
        }

        #endregion

        #region Generic

        /// <summary>
        /// Assigns the hydraulic boundary location.
        /// </summary>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <param name="location">The <see cref="HydraulicBoundaryLocation"/> to assign.</param>
        private static void SetLocation(IPipingCalculationScenario<PipingInput> pipingCalculation, HydraulicBoundaryLocation location)
        {
            if (location != null)
            {
                pipingCalculation.InputParameters.HydraulicBoundaryLocation = location;
            }
        }

        private bool TrySetGenericData(PipingCalculationConfiguration readCalculation,
                                       IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            return TrySetSurfaceLine(readCalculation, pipingCalculation)
                   && TrySetEntryExitPoint(readCalculation, pipingCalculation)
                   && TrySetStochasticSoilModel(readCalculation, pipingCalculation)
                   && TrySetStochasticSoilProfile(readCalculation, pipingCalculation)
                   && TrySetStochasts(readCalculation, pipingCalculation)
                   && TrySetScenarioParameters(readCalculation.Scenario, pipingCalculation);
        }

        /// <summary>
        /// Assigns the surface line.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has a <see cref="PipingSurfaceLine"/>
        /// set which is not available in <see cref="PipingFailureMechanism.SurfaceLines"/>, <c>true</c> otherwise.</returns>
        private bool TrySetSurfaceLine(PipingCalculationConfiguration calculationConfiguration,
                                       IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            if (calculationConfiguration.SurfaceLineName != null)
            {
                PipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                .FirstOrDefault(sl => sl.Name == calculationConfiguration.SurfaceLineName);

                if (surfaceLine == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist,
                                                          calculationConfiguration.SurfaceLineName),
                                                      pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.SurfaceLine = surfaceLine;
            }

            return true;
        }

        /// <summary>
        /// Assigns the entry point and exit point.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when entry or exit point is set without <see cref="PipingSurfaceLine"/>,
        /// or when entry or exit point is invalid, <c>true</c> otherwise.</returns>
        private bool TrySetEntryExitPoint(PipingCalculationConfiguration calculationConfiguration,
                                          IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            bool hasEntryPoint = calculationConfiguration.EntryPointL.HasValue;
            bool hasExitPoint = calculationConfiguration.ExitPointL.HasValue;

            if (calculationConfiguration.SurfaceLineName == null && (hasEntryPoint || hasExitPoint))
            {
                Log.LogCalculationConversionError(Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_EntryPointL_or_ExitPointL_defined_without_SurfaceLine,
                                                  pipingCalculation.Name);
                return false;
            }

            if (hasEntryPoint)
            {
                double entryPoint = calculationConfiguration.EntryPointL.Value;

                try
                {
                    pipingCalculation.InputParameters.EntryPointL = (RoundedDouble) entryPoint;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Entry_point_invalid, entryPoint),
                                               pipingCalculation.Name,
                                               e);
                    return false;
                }
            }

            if (hasExitPoint)
            {
                double exitPoint = calculationConfiguration.ExitPointL.Value;

                try
                {
                    pipingCalculation.InputParameters.ExitPointL = (RoundedDouble) exitPoint;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Exit_point_invalid, exitPoint),
                                               pipingCalculation.Name,
                                               e);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Assigns the stochastic soil model.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when
        /// <list type="bullet">
        /// <item>the <paramref name="calculationConfiguration"/> has a <see cref="PipingStochasticSoilModel"/> set
        /// which is not available in the failure mechanism.</item>
        /// <item>The <see cref="PipingStochasticSoilModel"/> does not intersect with the <see cref="PipingSurfaceLine"/>
        /// when this is set.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilModel(PipingCalculationConfiguration calculationConfiguration,
                                               IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            if (calculationConfiguration.StochasticSoilModelName != null)
            {
                PipingStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                      .FirstOrDefault(ssm => ssm.Name == calculationConfiguration.StochasticSoilModelName);

                if (soilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist,
                                                          calculationConfiguration.StochasticSoilModelName),
                                                      pipingCalculation.Name);
                    return false;
                }

                if (pipingCalculation.InputParameters.SurfaceLine != null
                    && !soilModel.IntersectsWithSurfaceLineGeometry(pipingCalculation.InputParameters.SurfaceLine))
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1,
                                                          calculationConfiguration.StochasticSoilModelName,
                                                          calculationConfiguration.SurfaceLineName),
                                                      pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.StochasticSoilModel = soilModel;
            }

            return true;
        }

        /// <summary>
        /// Assigns the stochastic soil profile.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="calculationConfiguration"/> has:
        /// <list type="bullet">
        /// <item>a <see cref="PipingStochasticSoilProfile"/> set but no <see cref="PipingStochasticSoilModel"/> is specified;</item>
        /// <item>a <see cref="PipingStochasticSoilProfile"/> set which is not available in the <see cref="PipingStochasticSoilModel"/>.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TrySetStochasticSoilProfile(PipingCalculationConfiguration calculationConfiguration,
                                                 IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            if (calculationConfiguration.StochasticSoilProfileName != null)
            {
                if (pipingCalculation.InputParameters.StochasticSoilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0,
                                                          calculationConfiguration.StochasticSoilProfileName),
                                                      pipingCalculation.Name);
                    return false;
                }

                PipingStochasticSoilProfile soilProfile = pipingCalculation.InputParameters
                                                                           .StochasticSoilModel
                                                                           .StochasticSoilProfiles
                                                                           .FirstOrDefault(ssp => ssp.SoilProfile.Name == calculationConfiguration.StochasticSoilProfileName);

                if (soilProfile == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1,
                                                          calculationConfiguration.StochasticSoilProfileName,
                                                          calculationConfiguration.StochasticSoilModelName),
                                                      pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.StochasticSoilProfile = soilProfile;
            }

            return true;
        }

        /// <summary>
        /// Assigns the stochasts.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when a stochast value (mean or standard deviation) is invalid, <c>true</c> otherwise.</returns>
        private bool TrySetStochasts(PipingCalculationConfiguration calculationConfiguration,
                                     IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            return TrySetPhreaticLevelExit(calculationConfiguration, pipingCalculation)
                   && TrySetDampingFactorExit(calculationConfiguration, pipingCalculation);
        }

        private bool TrySetDampingFactorExit(PipingCalculationConfiguration calculationConfiguration,
                                             IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            return ConfigurationImportHelper.TrySetStandardDeviationStochast(
                PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName,
                pipingCalculation.Name,
                pipingCalculation.InputParameters,
                calculationConfiguration.DampingFactorExit,
                i => i.DampingFactorExit,
                (i, s) => i.DampingFactorExit = s,
                Log);
        }

        private bool TrySetPhreaticLevelExit(PipingCalculationConfiguration calculationConfiguration,
                                             IPipingCalculationScenario<PipingInput> pipingCalculation)
        {
            return ConfigurationImportHelper.TrySetStandardDeviationStochast(
                PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName,
                pipingCalculation.Name,
                pipingCalculation.InputParameters,
                calculationConfiguration.PhreaticLevelExit,
                i => i.PhreaticLevelExit,
                (i, s) => i.PhreaticLevelExit = s,
                Log);
        }

        #endregion
    }
}