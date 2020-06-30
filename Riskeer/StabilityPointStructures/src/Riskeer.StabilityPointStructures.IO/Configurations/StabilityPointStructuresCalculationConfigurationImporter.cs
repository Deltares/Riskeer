// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.IO.Configurations.Helpers;
using Riskeer.StabilityPointStructures.IO.Properties;
using Riskeer.StabilityPointStructures.Util;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Class for importing a configuration of <see cref="StabilityPointStructuresCalculationConfiguration"/> 
    /// from an XML file and storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<
            StabilityPointStructuresCalculationConfigurationReader,
            StabilityPointStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<StabilityPointStructure> availableStructures;
        private readonly StabilityPointStructuresFailureMechanism failureMechanism;

        /// <summary>
        /// Create new instance of <see cref="StabilityPointStructuresCalculationConfigurationImporter"/>
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
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<StabilityPointStructure> structures,
            StabilityPointStructuresFailureMechanism failureMechanism)
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
            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);

            base.DoPostImportUpdates();
        }

        protected override StabilityPointStructuresCalculationConfigurationReader CreateCalculationConfigurationReader(
            string xmlFilePath)
        {
            return new StabilityPointStructuresCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(StabilityPointStructuresCalculationConfiguration readCalculation)
        {
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = readCalculation.Name
            };

            if (TrySetStructure(readCalculation.StructureId, calculation)
                && TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TrySetForeshoreProfile(readCalculation.ForeshoreProfileId, calculation)
                && TrySetEvaluationLevel(readCalculation, calculation)
                && TrySetFailureProbabilityRepairClosure(readCalculation, calculation)
                && TrySetFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && TrySetInflowModelType(readCalculation, calculation)
                && TrySetLevellingCount(readCalculation, calculation)
                && TrySetLoadSchematizationType(readCalculation, calculation)
                && TrySetProbabilityCollisionSecondaryStructure(readCalculation, calculation)
                && TrySetStochasts(readCalculation, calculation)
                && TrySetStructureNormalOrientation(readCalculation, calculation)
                && TrySetVerticalDistance(readCalculation, calculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile, calculation.Name, Log))
            {
                SetFactorStormDurationOpenStructure(readCalculation, calculation);
                SetVolumicWeightWater(readCalculation, calculation);
                SetWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                SetShouldIllustrationPointsBeCalculated(readCalculation, calculation);
                return calculation;
            }

            return null;
        }

        /// <summary>
        /// Sets the evaluation level.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the evaluation level is invalid or when there is an
        /// evaluation level but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetEvaluationLevel(StabilityPointStructuresCalculationConfiguration readCalculation,
                                           StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.EvaluationLevel.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_EvaluationLevel_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.EvaluationLevel = (RoundedDouble) readCalculation.EvaluationLevel.Value;
            }

            return true;
        }

        private bool TrySetStochasts(StabilityPointStructuresCalculationConfiguration readCalculation,
                                     StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                readCalculation,
                calculation);

            return assigner.Assign();
        }

        /// <summary>
        /// Sets the structure normal orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the structure normal orientation is invalid or when there 
        /// is a structure normal orientation but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetStructureNormalOrientation(StructuresCalculationConfiguration readCalculation,
                                                      StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.StructureNormalOrientation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RiskeerCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
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
                    Log.LogOutOfRangeException(string.Format(RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                             orientation,
                                                             RiskeerCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the failure probability repair closure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the failure probability repair closure is invalid or when there 
        /// is a failure probability repair closure but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetFailureProbabilityRepairClosure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                           StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (!readCalculation.FailureProbabilityRepairClosure.HasValue)
            {
                return true;
            }

            if (calculation.InputParameters.Structure == null)
            {
                Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                Resources.CalculationConfigurationImporter_FailureProbabilityRepairClosure_DisplayName),
                                                  calculation.Name);

                return false;
            }

            double failureProbabilityRepairClosure = readCalculation.FailureProbabilityRepairClosure.Value;

            try
            {
                calculation.InputParameters.FailureProbabilityRepairClosure = (RoundedDouble) failureProbabilityRepairClosure;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.LogOutOfRangeException(string.Format(RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                         failureProbabilityRepairClosure,
                                                         Resources.CalculationConfigurationImporter_FailureProbabilityRepairClosure_DisplayName),
                                           calculation.Name,
                                           e);

                return false;
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
                                                                  StructuresCalculation<StabilityPointStructuresInput> calculation)
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
                    Log.LogOutOfRangeException(string.Format(
                                                   RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   RiskeerCommonIOResources.CalculationConfigurationImporter_FailureProbabilityStructureWithErosion_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the probability collision secondary structure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the probability collision secondary structure is invalid or 
        /// when there is a probability collision secondary structure but no structure defined, 
        /// <c>true</c> otherwise.</returns>
        private bool TrySetProbabilityCollisionSecondaryStructure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                                  StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.ProbabilityCollisionSecondaryStructure.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_ProbabilityCollisionSecondaryStructure_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.ProbabilityCollisionSecondaryStructure.Value;

                try
                {
                    calculation.InputParameters.ProbabilityCollisionSecondaryStructure = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RiskeerCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   Resources.CalculationConfigurationImporter_ProbabilityCollisionSecondaryStructure_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the factor storm duration open structure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private void SetFactorStormDurationOpenStructure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                         StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.FactorStormDurationOpenStructure.HasValue)
            {
                calculation.InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) readCalculation.FactorStormDurationOpenStructure.Value;
            }
        }

        /// <summary>
        /// Sets the volumic weight water.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private void SetVolumicWeightWater(StabilityPointStructuresCalculationConfiguration readCalculation,
                                           StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.VolumicWeightWater.HasValue)
            {
                calculation.InputParameters.VolumicWeightWater = (RoundedDouble) readCalculation.VolumicWeightWater.Value;
            }
        }

        /// <summary>
        /// Sets the inflow model type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the inflow model type is invalid or when there is a 
        /// inflow model type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetInflowModelType(StabilityPointStructuresCalculationConfiguration readCalculation,
                                           StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.InflowModelType.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RiskeerCommonIOResources.CalculationConfigurationImporter_InflowModelType_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.InflowModelType = (StabilityPointStructureInflowModelType)
                    new ConfigurationStabilityPointStructuresInflowModelTypeConverter()
                        .ConvertTo(readCalculation.InflowModelType.Value, typeof(StabilityPointStructureInflowModelType));
            }

            return true;
        }

        /// <summary>
        /// Sets the load schematization type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the load schematization type is invalid or when there is a 
        /// load schematization type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetLoadSchematizationType(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                  StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.LoadSchematizationType.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_LoadSchematizationType_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.LoadSchematizationType = (LoadSchematizationType)
                    new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter()
                        .ConvertTo(readCalculation.LoadSchematizationType.Value, typeof(LoadSchematizationType));
            }

            return true;
        }

        /// <summary>
        /// Sets the levelling count.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the levelling count is invalid or when there is a 
        /// levelling count but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetLevellingCount(StabilityPointStructuresCalculationConfiguration readCalculation,
                                          StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.LevellingCount.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_LevellingCount_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.LevellingCount = readCalculation.LevellingCount.Value;
            }

            return true;
        }

        /// <summary>
        /// Sets the vertical distance.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the vertical distance is invalid or when there is a 
        /// vertical distance but no structure defined, <c>true</c> otherwise.</returns>
        private bool TrySetVerticalDistance(StabilityPointStructuresCalculationConfiguration readCalculation,
                                            StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.VerticalDistance.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RiskeerCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_VerticalDistance_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.VerticalDistance = (RoundedDouble) readCalculation.VerticalDistance.Value;
            }

            return true;
        }

        private bool TrySetHydraulicBoundaryLocation(string locationName, StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetStructure(string structureId, StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            StabilityPointStructure structure;

            if (TryReadStructure(structureId, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TrySetForeshoreProfile(string foreshoreProfileName, StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            ForeshoreProfile foreshoreProfile;

            if (TryReadForeshoreProfile(foreshoreProfileName, calculation.Name, availableForeshoreProfiles, out foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }

        private static void SetShouldIllustrationPointsBeCalculated(StabilityPointStructuresCalculationConfiguration calculationConfiguration,
                                                                    StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (calculationConfiguration.ShouldIllustrationPointsBeCalculated.HasValue)
            {
                calculation.InputParameters.ShouldIllustrationPointsBeCalculated = calculationConfiguration.ShouldIllustrationPointsBeCalculated.Value;
            }
        }
    }
}