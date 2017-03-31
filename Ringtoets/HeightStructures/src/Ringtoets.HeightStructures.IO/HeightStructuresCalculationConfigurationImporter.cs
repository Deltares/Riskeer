// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Schema;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO
{
    public class HeightStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<HeightStructuresCalculationConfigurationReader, HeightStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<HeightStructure> availableStructures;

        /// <summary>
        /// Create new instance of <see cref="HeightStructuresCalculationConfigurationImporter"/>
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if 
        /// the imported objects contain the right foreshore profile.</param>
        /// <param name="structures">The dike profiles used to check if
        /// the imported objects contain the right profile.</param>
        public HeightStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<HeightStructure> structures)
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
            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableForeshoreProfiles = foreshoreProfiles;
            availableStructures = structures;
        }

        protected override HeightStructuresCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new HeightStructuresCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(HeightStructuresCalculationConfiguration readCalculation)
        {
            var calculation = new StructuresCalculation<HeightStructuresInput>()
            {
                Name = readCalculation.Name
            };

            if (TryReadStructure(readCalculation, calculation)
                && TryReadStochasts(readCalculation, calculation)
                && TryReadHydraulicBoundaryLocation(readCalculation, calculation)
                && TryReadDikeProfile(readCalculation, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && TryReadWaveReduction(readCalculation, calculation))
            {
                return calculation;
            }
            return null;
        }

        private bool TryReadStochasts(HeightStructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (!ValidateStochasts(readCalculation))
            {
                return false;
            }

            return TryReadStandardDeviationStochast(
                       calculation,
                       HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                       readCalculation.LevelCrestStructure,
                       i => i.LevelCrestStructure,
                       (i, d) => i.LevelCrestStructure = d)
                   && TryReadStandardDeviationStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                       readCalculation.AllowedLevelIncreaseStorage,
                       i => i.AllowedLevelIncreaseStorage,
                       (i, d) => i.AllowedLevelIncreaseStorage = d)
                   && TryReadStandardDeviationStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                       readCalculation.FlowWidthAtBottomProtection,
                       i => i.FlowWidthAtBottomProtection,
                       (i, d) => i.FlowWidthAtBottomProtection = d)
                   && TryReadStandardDeviationStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                       readCalculation.ModelFactorSuperCriticalFlow,
                       i => i.ModelFactorSuperCriticalFlow,
                       (i, d) => i.ModelFactorSuperCriticalFlow = d)
                   && TryReadStandardDeviationStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                       readCalculation.WidthFlowApertures,
                       i => i.WidthFlowApertures,
                       (i, d) => i.WidthFlowApertures = d)
                   && TryReadVariationCoefficientStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                       readCalculation.CriticalOvertoppingDischarge,
                       i => i.CriticalOvertoppingDischarge,
                       (i, d) => i.CriticalOvertoppingDischarge = d)
                   && TryReadVariationCoefficientStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                       readCalculation.StorageStructureArea,
                       i => i.StorageStructureArea,
                       (i, d) => i.StorageStructureArea = d)
                   && TryReadVariationCoefficientStochast(
                       calculation,
                       ConfigurationSchemaIdentifiers.StormDurationStochastName,
                       readCalculation.StormDuration,
                       i => i.StormDuration,
                       (i, d) => i.StormDuration = d);
        }

        private bool ValidateStochasts(HeightStructuresCalculationConfiguration readCalculation)
        {
            if (readCalculation.StormDuration?.VariationCoefficient != null)
            {
                LogReadCalculationConversionError("Er kan geen variatiecoëfficiënt voor stochast 'stormduur' opgegeven worden.", readCalculation.Name);
                return false;
            }
            if (readCalculation.ModelFactorSuperCriticalFlow?.StandardDeviation != null)
            {
                LogReadCalculationConversionError("Er kan geen standaardafwijking voor stochast 'modelfactoroverloopdebiet' opgegeven worden.", readCalculation.Name);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the stochast parameters.
        /// </summary>
        /// <param name="calculation">The calculation to configure.</param>
        /// <param name="stochastName">The stochast's name.</param>
        /// <param name="stochastConfiguration">The configuration of the stochast.</param>
        /// <param name="getStochast">The function for obtaining the stochast to read.</param>
        /// <param name="setStochast">The function to set the stochast with the read parameters.</param>
        /// <returns><c>true</c> if reading all required wave reduction parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadStandardDeviationStochast<T>(
            StructuresCalculation<HeightStructuresInput> calculation,
            string stochastName,
            MeanStandardDeviationStochastConfiguration stochastConfiguration,
            Func<HeightStructuresInput, T> getStochast,
            Action<HeightStructuresInput, T> setStochast)
            where T : class, IDistribution
        {
            if (stochastConfiguration == null)
            {
                return true;
            }
            var distribution = (T) getStochast(calculation.InputParameters).Clone();

            if (!distribution.TrySetDistributionProperties(stochastConfiguration,
                                                           stochastName,
                                                           calculation.Name))
            {
                return false;
            }
            setStochast(calculation.InputParameters, distribution);
            return true;
        }

        /// <summary>
        /// Reads the stochast parameters.
        /// </summary>
        /// <param name="calculation">The calculation to configure.</param>
        /// <param name="stochastName">The stochast's name.</param>
        /// <param name="stochastConfiguration">The configuration of the stochast.</param>
        /// <param name="getStochast">The function for obtaining the stochast to read.</param>
        /// <param name="setStochast">The function to set the stochast with the read parameters.</param>
        /// <returns><c>true</c> if reading all required wave reduction parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadVariationCoefficientStochast<T>(
            StructuresCalculation<HeightStructuresInput> calculation,
            string stochastName,
            MeanVariationCoefficientStochastConfiguration stochastConfiguration,
            Func<HeightStructuresInput, T> getStochast,
            Action<HeightStructuresInput, T> setStochast)
            where T : class, IVariationCoefficientDistribution
        {
            if (stochastConfiguration == null)
            {
                return true;
            }
            var distribution = (T) getStochast(calculation.InputParameters).Clone();

            if (!distribution.TrySetDistributionProperties(stochastConfiguration,
                                                           stochastName,
                                                           calculation.Name))
            {
                return false;
            }
            setStochast(calculation.InputParameters, distribution);
            return true;
        }

        /// <summary>
        /// Reads the orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.StructureNormalOrientation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    LogReadCalculationConversionError(
                        "Er is geen kunstwerk opgegeven om de oriëntatie aan toe te voegen.",
                        calculation.Name);

                    return false;
                }

                double orientation = readCalculation.StructureNormalOrientation.Value;

                try
                {
                    calculation.InputParameters.StructureNormalOrientation = (RoundedDouble) orientation;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    LogOutOfRangeException(
                        string.Format("Een waarde van '{0}' als oriëntatie is ongeldig.", orientation),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the failure probability structure with erosion.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is a failure probability 
        /// structure with erosion but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityStructureWithErosion.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    LogReadCalculationConversionError(
                        "Er is geen kunstwerk opgegeven om de faalkans gegeven erosie bodem aan toe te voegen.",
                        calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.FailureProbabilityStructureWithErosion.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    LogOutOfRangeException(
                        string.Format("Een waarde van '{0}' als faalkans gegeven erosie bodem is ongeldig.", failureProbability),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the hydraulic boundary location.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TryReadHydraulicBoundaryLocation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.HydraulicBoundaryLocationName != null)
            {
                HydraulicBoundaryLocation location = availableHydraulicBoundaryLocations
                    .FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocationName);

                if (location == null)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_ReadHydraulicBoundaryLocation_HydraulicBoundaryLocation_0_does_not_exist,
                            readCalculation.HydraulicBoundaryLocationName),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.HydraulicBoundaryLocation = location;
            }

            return true;
        }

        /// <summary>
        /// Reads the hydraulic boundary location.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableStructures"/>, <c>true</c> otherwise.</returns>
        private bool TryReadStructure(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.StructureName != null)
            {
                HeightStructure structure = availableStructures
                    .FirstOrDefault(l => l.Name == readCalculation.StructureName);

                if (structure == null)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_ReadStructure_Structure_0_does_not_exist,
                            readCalculation.StructureName),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.Structure = structure;
            }

            return true;
        }

        /// <summary>
        /// Reads the dike profile.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="ForeshoreProfile"/> 
        /// set which is not available in <see cref="availableForeshoreProfiles"/>, <c>true</c> otherwise.</returns>
        private bool TryReadDikeProfile(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.ForeshoreProfileName != null)
            {
                ForeshoreProfile foreshoreProfile = availableForeshoreProfiles.FirstOrDefault(fp => fp.Name == readCalculation.ForeshoreProfileName);

                if (foreshoreProfile == null)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            "Het voorlandprofiel '{0}' bestaat niet.",
                            readCalculation.ForeshoreProfileName),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
            }

            return true;
        }

        /// <summary>
        /// Reads the wave reduction parameters.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        private bool TryReadWaveReduction(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (!ValidateWaveReduction(readCalculation, calculation))
            {
                return false;
            }

            WaveReductionConfiguration waveReduction = readCalculation.WaveReduction;
            if (waveReduction == null)
            {
                return true;
            }

            if (waveReduction.UseForeshoreProfile.HasValue)
            {
                calculation.InputParameters.UseForeshore = waveReduction.UseForeshoreProfile.Value;
            }

            if (waveReduction.UseBreakWater.HasValue)
            {
                calculation.InputParameters.UseBreakWater = waveReduction.UseBreakWater.Value;
            }

            if (waveReduction.BreakWaterType.HasValue)
            {
                calculation.InputParameters.BreakWater.Type = (BreakWaterType) new SchemaBreakWaterTypeConverter().ConvertTo(waveReduction.BreakWaterType.Value, typeof(BreakWaterType));
            }

            if (waveReduction.BreakWaterHeight.HasValue)
            {
                calculation.InputParameters.BreakWater.Height = (RoundedDouble) waveReduction.BreakWaterHeight.Value;
            }

            return true;
        }

        /// <summary>
        /// Validation to check if the defined wave reduction parameters are valid.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        private bool ValidateWaveReduction(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (calculation.InputParameters.ForeshoreProfile == null)
            {
                if (readCalculation.WaveReduction != null
                    && (readCalculation.WaveReduction.UseBreakWater.HasValue
                        || readCalculation.WaveReduction.UseForeshoreProfile.HasValue
                        || readCalculation.WaveReduction.BreakWaterHeight.HasValue
                        || readCalculation.WaveReduction.BreakWaterType.HasValue))
                {
                    LogReadCalculationConversionError(
                        "Er is geen voorlandprofiel opgegeven om golfreductie parameters aan toe te voegen.",
                        calculation.Name);

                    return false;
                }
            }
            else if (!calculation.InputParameters.ForeshoreGeometry.Any())
            {
                if (readCalculation.WaveReduction.UseForeshoreProfile.HasValue && readCalculation.WaveReduction.UseForeshoreProfile.Value)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            "Het opgegeven voorlandprofiel '{0}' heeft geen geometrie en kan daarom niet gebruikt worden.",
                            readCalculation.ForeshoreProfileName),
                        calculation.Name);
                    return false;
                }
            }
            return true;
        }
    }
}