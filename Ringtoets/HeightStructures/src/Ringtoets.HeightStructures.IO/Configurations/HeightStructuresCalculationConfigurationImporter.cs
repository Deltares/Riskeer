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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Schema;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO.Configurations
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
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile, calculation.Name, Log))
            {
                ReadWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                return calculation;
            }
            return null;
        }

        private bool TryReadStochasts(HeightStructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (!readCalculation.ValidateStructureBaseStochasts(Log))
            {
                return false;
            }

            return TryReadStandardDeviationStochast(
                       HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.LevelCrestStructure,
                       i => i.LevelCrestStructure, (i, d) => i.LevelCrestStructure = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.AllowedLevelIncreaseStorage,
                       i => i.AllowedLevelIncreaseStorage, (i, d) => i.AllowedLevelIncreaseStorage = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.FlowWidthAtBottomProtection,
                       i => i.FlowWidthAtBottomProtection, (i, d) => i.FlowWidthAtBottomProtection = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ModelFactorSuperCriticalFlow,
                       i => i.ModelFactorSuperCriticalFlow, (i, d) => i.ModelFactorSuperCriticalFlow = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.WidthFlowApertures, i => i.WidthFlowApertures, (i, d) => i.WidthFlowApertures = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.CriticalOvertoppingDischarge,
                       i => i.CriticalOvertoppingDischarge, (i, d) => i.CriticalOvertoppingDischarge = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StorageStructureArea,
                       i => i.StorageStructureArea, (i, d) => i.StorageStructureArea = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.StormDurationStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StormDuration,
                       i => i.StormDuration, (i, d) => i.StormDuration = d);
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
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
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
                    Log.LogOutOfRangeException(string.Format(RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
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
        /// <returns><c>false</c> when the failure probability structure with erosion is invalid or when there is
        /// a failure probability structure with erosion but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityStructureWithErosion.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
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
                    Log.LogOutOfRangeException(string.Format(
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