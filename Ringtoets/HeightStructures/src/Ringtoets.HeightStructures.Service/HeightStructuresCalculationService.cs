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

using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for height structures.
    /// </summary>
    public class HeightStructuresCalculationService : StructuresCalculationServiceBase<HeightStructuresValidationRulesRegistry, HeightStructuresInput,
        HeightStructure, HeightStructuresFailureMechanism, StructuresOvertoppingCalculationInput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HeightStructuresCalculationService));

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationService"/>.
        /// </summary>
        public HeightStructuresCalculationService() : base(new HeightStructuresCalculationMessageProvider()) {}

        protected override StructuresOvertoppingCalculationInput CreateInput(StructuresCalculation<HeightStructuresInput> calculation,
                                                                             HeightStructuresFailureMechanism failureMechanism,
                                                                             string hydraulicBoundaryDatabaseFilePath)
        {
            GeneralHeightStructuresInput generalInput = failureMechanism.GeneralInput;

            var structuresOvertoppingCalculationInput = new StructuresOvertoppingCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                calculation.InputParameters.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                calculation.InputParameters.LevelCrestStructure.Mean, calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.WidthFlowApertures.Mean, calculation.InputParameters.WidthFlowApertures.StandardDeviation,
                calculation.InputParameters.DeviationWaveDirection,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresOvertoppingCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return structuresOvertoppingCalculationInput;
        }

        protected override void PerformCalculation(IStructuresCalculator<StructuresOvertoppingCalculationInput> calculator,
                                                   StructuresOvertoppingCalculationInput input,
                                                   StructuresCalculation<HeightStructuresInput> calculation,
                                                   IAssessmentSection assessmentSection,
                                                   HeightStructuresFailureMechanism failureMechanism)
        {
            var exceptionThrown = false;

            try
            {
                calculator.Calculate(input);

                if (!Canceled && string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    ProbabilityAssessmentOutput probabilityAssessmentOutput =
                        ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                               failureMechanism.Contribution,
                                                               failureMechanism.GeneralInput.N,
                                                               calculator.ExceedanceProbabilityBeta);
                    calculation.Output = new StructuresOutput(probabilityAssessmentOutput);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!Canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_no_error_report,
                                        calculation.Name);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_click_details_for_last_error_report_1,
                                        calculation.Name, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(Canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_click_details_for_last_error_report_1,
                                    calculation.Name, lastErrorFileContent);
                }

                log.InfoFormat(Resources.HeightStructuresCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }
    }
}