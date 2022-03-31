﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Imports a wave conditions calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="TCalculation">The type of the calculation to import.</typeparam>
    /// <typeparam name="TCalculationConfigurationReader">The type of the reader to use.</typeparam>
    /// <typeparam name="TCalculationConfiguration">The type of calculation configuration.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationImporter<TCalculation, TCalculationConfigurationReader, TCalculationConfiguration>
        : CalculationConfigurationImporter<TCalculationConfigurationReader, TCalculationConfiguration>
        where TCalculation : ICalculation<WaveConditionsInput>, new()
        where TCalculationConfigurationReader : CalculationConfigurationReader<TCalculationConfiguration>
        where TCalculationConfiguration : WaveConditionsCalculationConfiguration
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly FailureMechanismContribution failureMechanismContribution;
        private readonly IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForTargetProbabilities;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationImporter{TCalculation, TCalculationConfigurationReader, TCalculationConfiguration}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if
        /// the imported objects contain the right profile.</param>
        /// <param name="failureMechanismContribution">The <see cref="FailureMechanismContribution"/>
        /// to get the norms from.</param>
        /// <param name="calculationsForTargetProbabilities">The  hydraulic boundary location calculations for target probabilities
        /// used to check if the imported objects contain the right target probability.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter <c>null</c>.</exception>
        protected WaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                                 CalculationGroup importTarget,
                                                                 IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                 IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                                 FailureMechanismContribution failureMechanismContribution,
                                                                 IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
            : base(xmlFilePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }

            if (failureMechanismContribution == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismContribution));
            }

            if (calculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbabilities));
            }

            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableForeshoreProfiles = foreshoreProfiles;
            this.failureMechanismContribution = failureMechanismContribution;
            this.calculationsForTargetProbabilities = calculationsForTargetProbabilities;
        }

        protected override ICalculation ParseReadCalculation(TCalculationConfiguration readCalculation)
        {
            var waveConditionsCalculation = new TCalculation
            {
                Name = readCalculation.Name
            };

            SetStepSize(readCalculation, waveConditionsCalculation);
            SetCalculationSpecificParameters(readCalculation, waveConditionsCalculation);

            if (TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, waveConditionsCalculation)
                && TrySetTargetProbability(readCalculation, waveConditionsCalculation)
                && TrySetBoundaries(readCalculation, waveConditionsCalculation)
                && TrySetForeshoreProfile(readCalculation.ForeshoreProfileId, waveConditionsCalculation)
                && TrySetOrientation(readCalculation, waveConditionsCalculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(waveConditionsCalculation.InputParameters.ForeshoreProfile,
                                                                       waveConditionsCalculation.Name, Log))
            {
                SetWaveReductionParameters(readCalculation.WaveReduction, waveConditionsCalculation.InputParameters);
                return waveConditionsCalculation;
            }

            return null;
        }

        /// <summary>
        /// Assigns calculation specific parameters of the calculation.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        protected virtual void SetCalculationSpecificParameters(TCalculationConfiguration calculationConfiguration,
                                                                TCalculation calculation) {}

        /// <summary>
        /// Assigns the boundaries of the calculation.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when one of the boundaries is invalid, <c>true</c> otherwise.</returns>
        private bool TrySetBoundaries(WaveConditionsCalculationConfiguration calculationConfiguration,
                                      ICalculation<WaveConditionsInput> calculation)
        {
            WaveConditionsInput input = calculation.InputParameters;
            return TryReadParameter(calculationConfiguration.UpperBoundaryRevetment,
                                    v => input.UpperBoundaryRevetment = v,
                                    Resources.WaveConditionsCalculationConfigurationImporter_UpperBoundaryRevetment_DisplayName,
                                    calculation.Name)
                   && TryReadParameter(calculationConfiguration.LowerBoundaryRevetment,
                                       v => input.LowerBoundaryRevetment = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_LowerBoundaryRevetment_DisplayName,
                                       calculation.Name)
                   && TryReadParameter(calculationConfiguration.UpperBoundaryWaterLevels,
                                       v => input.UpperBoundaryWaterLevels = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_UpperBoundaryWaterLevels_DisplayName,
                                       calculation.Name)
                   && TryReadParameter(calculationConfiguration.LowerBoundaryWaterLevels,
                                       v => input.LowerBoundaryWaterLevels = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_LowerBoundaryWaterLevels_DisplayName,
                                       calculation.Name);
        }

        private bool TryReadParameter(double? readValue, Action<RoundedDouble> setAsRoundedDouble,
                                      string parameterName,
                                      string calculationName)
        {
            if (readValue.HasValue)
            {
                try
                {
                    setAsRoundedDouble((RoundedDouble) readValue.Value);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   readValue.Value, parameterName),
                                               calculationName, e);
                    return false;
                }
            }

            return true;
        }

        private static void SetStepSize(WaveConditionsCalculationConfiguration calculationConfiguration,
                                        ICalculation<WaveConditionsInput> calculation)
        {
            if (calculationConfiguration.StepSize.HasValue)
            {
                calculation.InputParameters.StepSize = (WaveConditionsInputStepSize) calculationConfiguration.StepSize.Value;
            }
        }

        private bool TrySetHydraulicBoundaryLocation(string locationName, ICalculation<WaveConditionsInput> calculation)
        {
            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out HydraulicBoundaryLocation location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetTargetProbability(TCalculationConfiguration readCalculation, ICalculation<WaveConditionsInput> waveConditionsCalculation)
        {
            if (readCalculation.TargetProbability.HasValue)
            {
                if (TryMatchTargetProbability(readCalculation.TargetProbability.Value, readCalculation.Name,
                                              out Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability> matchedTargetProbability))
                {
                    waveConditionsCalculation.InputParameters.WaterLevelType = matchedTargetProbability.Item2;
                    waveConditionsCalculation.InputParameters.CalculationsTargetProbability = matchedTargetProbability.Item3;
                    return true;
                }
            }
            else
            {
                waveConditionsCalculation.InputParameters.WaterLevelType = ConvertNormType();
                return true;
            }

            return false;
        }

        private WaveConditionsInputWaterLevelType ConvertNormType()
        {
            switch (failureMechanismContribution.NormativeProbabilityType)
            {
                case NormativeProbabilityType.MaximumAllowableFloodingProbability:
                    return WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability;
                case NormativeProbabilityType.SignalFloodingProbability:
                    return WaveConditionsInputWaterLevelType.SignalFloodingProbability;
                default:
                    throw new NotSupportedException();
            }
        }

        private bool TryMatchTargetProbability(
            double targetProbability, string calculationName,
            out Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability> matchedTargetProbability)
        {
            var availableTargetProbabilities = new List<Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability>>(
                calculationsForTargetProbabilities.Select(c => new Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                                                              c.TargetProbability, WaveConditionsInputWaterLevelType.UserDefinedTargetProbability, c)))
            {
                new Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                    failureMechanismContribution.MaximumAllowableFloodingProbability, WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability, null),
                new Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability>(
                    failureMechanismContribution.SignalFloodingProbability, WaveConditionsInputWaterLevelType.SignalFloodingProbability, null)
            };

            Tuple<double, WaveConditionsInputWaterLevelType, HydraulicBoundaryLocationCalculationsForTargetProbability>[] matchedTargetProbabilities = availableTargetProbabilities.Where(
                tp => Math.Abs(tp.Item1 - targetProbability) < 1e-6).ToArray();

            if (matchedTargetProbabilities.Length != 1)
            {
                matchedTargetProbability = null;
                Log.LogCalculationConversionError(
                    string.Format(
                        Resources.WaveConditionsCalculationConfigurationImporter_TryMatchTargetProbability_TargetProbability_0_cannot_be_found,
                        targetProbability.ToString(CultureInfo.InvariantCulture)),
                    calculationName);
                return false;
            }

            matchedTargetProbability = matchedTargetProbabilities.Single();
            return true;
        }

        private bool TrySetForeshoreProfile(string foreshoreProfileName, ICalculation<WaveConditionsInput> calculation)
        {
            if (TryReadForeshoreProfile(foreshoreProfileName, calculation.Name, availableForeshoreProfiles, out ForeshoreProfile foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Assigns the orientation.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid, <c>true</c> otherwise.</returns>
        private bool TrySetOrientation(WaveConditionsCalculationConfiguration calculationConfiguration,
                                       ICalculation<WaveConditionsInput> calculation)
        {
            if (calculationConfiguration.Orientation.HasValue)
            {
                double orientation = calculationConfiguration.Orientation.Value;

                try
                {
                    calculation.InputParameters.Orientation = (RoundedDouble) orientation;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   orientation,
                                                   RiskeerCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
                                               calculation.Name, e);
                    return false;
                }
            }

            return true;
        }
    }
}