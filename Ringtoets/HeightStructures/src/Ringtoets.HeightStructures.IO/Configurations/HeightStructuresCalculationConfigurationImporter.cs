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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Util;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO.Configurations
{
    /// <summary>
    /// Class for importing a configuration of <see cref="HeightStructuresCalculationConfiguration"/> 
    /// from an XML file and storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class HeightStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<HeightStructuresCalculationConfigurationReader, HeightStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<HeightStructure> availableStructures;
        private readonly HeightStructuresFailureMechanism failureMechanism;

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
        /// <param name="failureMechanism">The failure mechanism used to propagate changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<HeightStructure> structures,
            HeightStructuresFailureMechanism failureMechanism)
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
            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            base.DoPostImportUpdates();
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

            if (TrySetStructure(readCalculation.StructureId, calculation)
                && TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TrySetForeshoreProfile(readCalculation.ForeshoreProfileId, calculation)
                && TrySetStochasts(readCalculation, calculation)
                && TrySetOrientation(readCalculation, calculation)
                && TrySetFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile,
                                                                       calculation.Name, Log))
            {
                SetWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                SetShouldIllustrationPointsBeCalculated(readCalculation, calculation);
                return calculation;
            }

            return null;
        }

        private bool TrySetStochasts(HeightStructuresCalculationConfiguration readCalculation,
                                     StructuresCalculation<HeightStructuresInput> calculation)
        {
            var assigner = new HeightStructuresCalculationStochastAssigner(
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
        private bool TrySetOrientation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<HeightStructuresInput> calculation)
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
        /// <returns><c>false</c> when the failure probability structure with erosion is invalid or when there is
        /// a failure probability structure with erosion but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation,
                                                                  StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityStructureWithErosion.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(
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

        private bool TrySetHydraulicBoundaryLocation(string locationName, StructuresCalculation<HeightStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetStructure(string structureId, StructuresCalculation<HeightStructuresInput> calculation)
        {
            HeightStructure structure;

            if (TryReadStructure(structureId, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TrySetForeshoreProfile(string foreshoreProfileId, StructuresCalculation<HeightStructuresInput> calculation)
        {
            ForeshoreProfile foreshoreProfile;

            if (TryReadForeshoreProfile(foreshoreProfileId, calculation.Name, availableForeshoreProfiles, out foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }

        private static void SetShouldIllustrationPointsBeCalculated(HeightStructuresCalculationConfiguration calculationConfiguration,
                                                                    StructuresCalculation<HeightStructuresInput> calculation)
        {
            if (calculationConfiguration.ShouldIllustrationPointsBeCalculated.HasValue)
            {
                calculation.InputParameters.ShouldIllustrationPointsBeCalculated = calculationConfiguration.ShouldIllustrationPointsBeCalculated.Value;
            }
        }
    }
}