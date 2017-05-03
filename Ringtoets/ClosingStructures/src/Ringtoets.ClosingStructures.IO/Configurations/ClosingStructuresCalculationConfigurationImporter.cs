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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.IO.Configurations.Helpers;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Common.Utils;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Class for importing a configuration of <see cref="ClosingStructuresCalculationConfiguration"/> 
    /// from an XML file and storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class ClosingStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<ClosingStructuresCalculationConfigurationReader, ClosingStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<ClosingStructure> availableStructures;
        private readonly ClosingStructuresFailureMechanism failureMechanism;

        /// <summary>
        /// Create new instance of <see cref="ClosingStructuresCalculationConfigurationImporter"/>
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if 
        /// the imported objects contain the right foreshore profile.</param>
        /// <param name="structures">The structures used to check if
        /// the imported objects contain the right structure.</param>
        /// <param name="failureMechanism">The failure mechanism used to propagate changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<ClosingStructure> structures,
            ClosingStructuresFailureMechanism failureMechanism)
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
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableForeshoreProfiles = foreshoreProfiles;
            availableStructures = structures;
            this.failureMechanism = failureMechanism;
        }

        protected override void DoPostImportUpdates()
        {
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                failureMechanism.SectionResults,
                failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>());

            base.DoPostImportUpdates();
        }

        protected override ClosingStructuresCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new ClosingStructuresCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(ClosingStructuresCalculationConfiguration readCalculation)
        {
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = readCalculation.Name
            };

            if (TrySetStructure(readCalculation.StructureName, calculation)
                && TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TrySetForeshoreProfile(readCalculation.ForeshoreProfileId, calculation)
                && TrySetStochasts(readCalculation, calculation)
                && TrySetOrientation(readCalculation, calculation)
                && TrySetFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && TrySetFailureProbabilityOpenStructure(readCalculation, calculation)
                && TrySetFailureProbabilityReparation(readCalculation, calculation)
                && TrySetProbabilityOrFrequencyOpenStructureBeforeFlooding(readCalculation, calculation)
                && TrySetInflowModelType(readCalculation, calculation)
                && TrySetIdenticalApertures(readCalculation, calculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile,
                                                                       calculation.Name, Log))
            {
                SetFactorStormDurationOpenStructure(readCalculation, calculation);
                SetWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                return calculation;
            }
            return null;
        }

        private bool TrySetStochasts(ClosingStructuresCalculationConfiguration readCalculation,
                                     StructuresCalculation<ClosingStructuresInput> calculation)
        {
            var assigner = new ClosingStructuresCalculationStochastAssigner(
                readCalculation,
                calculation);

            return assigner.Assign();
        }

        /// <summary>
        /// Sets the orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetOrientation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.StructureNormalOrientation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
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
                    Log.LogOutOfRangeException(
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
        /// Sets the failure probability structure with erosion.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is a failure probability 
        /// structure with erosion but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation,
                                                                  StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityStructureWithErosion.HasValue)
            {
                double failureProbability = readCalculation.FailureProbabilityStructureWithErosion.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(
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

        /// <summary>
        /// Sets the factor storm duration.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private void SetFactorStormDurationOpenStructure(ClosingStructuresCalculationConfiguration readCalculation,
                                                         StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FactorStormDurationOpenStructure.HasValue)
            {
                calculation.InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) readCalculation.FactorStormDurationOpenStructure.Value;
            }
        }

        /// <summary>
        /// Sets the failure probability open structure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the failure probability open structure is invalid or when 
        /// there is a failure probability open structure but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetFailureProbabilityOpenStructure(ClosingStructuresCalculationConfiguration readCalculation,
                                                           StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityOpenStructure.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityOpenStructure_DisplayName),
                        calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.FailureProbabilityOpenStructure.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityOpenStructure = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(
                        string.Format(
                            RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                            failureProbability,
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityOpenStructure_DisplayName),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the failure probability reparation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the failure probability reparation is invalid or when there 
        /// is a failure probability reparation but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetFailureProbabilityReparation(ClosingStructuresCalculationConfiguration readCalculation,
                                                        StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityReparation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityReparation_DisplayName),
                        calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.FailureProbabilityReparation.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityReparation = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(
                        string.Format(
                            RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                            failureProbability,
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityReparation_DisplayName),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the probability or frequency open structure before flooding.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the probability or frequency open structure before flooding 
        /// is invalid or when there is a probability or frequency open structure before flooding 
        /// but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetProbabilityOrFrequencyOpenStructureBeforeFlooding(ClosingStructuresCalculationConfiguration readCalculation,
                                                                             StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.ProbabilityOrFrequencyOpenStructureBeforeFlooding.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_ProbabilityOrFrequencyOpenStructureBeforeFlooding_DisplayName),
                        calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.ProbabilityOrFrequencyOpenStructureBeforeFlooding.Value;

                try
                {
                    calculation.InputParameters.ProbabilityOrFrequencyOpenStructureBeforeFlooding = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(
                        string.Format(
                            RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                            failureProbability,
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_ProbabilityOrFrequencyOpenStructureBeforeFlooding_DisplayName),
                        calculation.Name,
                        e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the inflow model type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the inflow model type is invalid or when there is a 
        /// inflow model type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetInflowModelType(ClosingStructuresCalculationConfiguration readCalculation,
                                           StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.InflowModelType.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_InflowModelType_DisplayName),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.InflowModelType =
                    (ClosingStructureInflowModelType) new ConfigurationClosingStructureInflowModelTypeConverter()
                        .ConvertTo(readCalculation.InflowModelType.Value, typeof(ClosingStructureInflowModelType));
            }

            return true;
        }

        /// <summary>
        /// Sets the number of identical apertures.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the number of identical apertures is invalid or when there is a 
        /// number of identical apertures but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetIdenticalApertures(ClosingStructuresCalculationConfiguration readCalculation,
                                              StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.IdenticalApertures.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
                        string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                      RingtoetsCommonIOResources.CalculationConfigurationImporter_IdenticalApertures_DisplayName),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.IdenticalApertures = readCalculation.IdenticalApertures.Value;
            }

            return true;
        }

        private bool TrySetHydraulicBoundaryLocation(string locationName, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetStructure(string structureName, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            ClosingStructure structure;

            if (TryReadStructure(structureName, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TrySetForeshoreProfile(string foreshoreProfileName, StructuresCalculation<ClosingStructuresInput> calculation)
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