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
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Schema;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// Class for importing a configuration of <see cref="HeightStructuresCalculationConfiguration"/> from an XML file and storing
    /// it on a <see cref="CalculationGroup"/>.
    /// </summary>
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
        /// <param name="structures">The structures used to check if
        /// the imported objects contain the right structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
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
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = readCalculation.Name
            };

            if (TryReadStructure(readCalculation.StructureName, calculation)
                && TryReadHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TryReadForeshoreProfile(readCalculation.ForeshoreProfileName, calculation)
                && TryReadStochasts(readCalculation, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && ValidateWaveReduction(readCalculation.WaveReduction, calculation.InputParameters.ForeshoreProfile, calculation.Name))
            {
                ReadWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
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
        /// <returns><c>true</c> if reading all required stochast parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadStandardDeviationStochast<T>(
            StructuresCalculation<HeightStructuresInput> calculation,
            string stochastName,
            MeanStandardDeviationStochastConfiguration stochastConfiguration,
            Func<HeightStructuresInput, T> getStochast,
            Action<HeightStructuresInput, T> setStochast)
            where T : IDistribution
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
        /// <returns><c>true</c> if reading all required stochast parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadVariationCoefficientStochast<T>(
            StructuresCalculation<HeightStructuresInput> calculation,
            string stochastName,
            MeanVariationCoefficientStochastConfiguration stochastConfiguration,
            Func<HeightStructuresInput, T> getStochast,
            Action<HeightStructuresInput, T> setStochast)
            where T : IVariationCoefficientDistribution
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
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
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
                        string.Format(RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                      orientation,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
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
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityStructureWithErosion_DisplayName),
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
                        string.Format(
                            RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                            failureProbability,
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityStructureWithErosion_DisplayName),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        private bool TryReadHydraulicBoundaryLocation(string locationName, StructuresCalculation<HeightStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TryReadStructure(string structureName, StructuresCalculation<HeightStructuresInput> calculation)
        {
            HeightStructure structure;

            if (TryReadStructure(structureName, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TryReadForeshoreProfile(string foreshoreProfileName, StructuresCalculation<HeightStructuresInput> calculation)
        {
            ForeshoreProfile foreshoreProfile;

            if (TryReadForeshoreProfile(foreshoreProfileName, calculation.Name, availableForeshoreProfiles, out foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }
    }
}