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
using System.IO;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
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

        private IStructuresCalculator<StructuresOvertoppingCalculationInput> calculator;
        private bool canceled;

        /// <summary>
        /// Cancels any currently running height structures calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Performs a height structures calculation based on the supplied <see cref="StructuresCalculation{T}"/> and sets <see cref="StructuresCalculation{T}.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism"> The <see cref="HeightStructuresFailureMechanism"/> that holds the information about the contribution 
        /// and the general inputs used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public override void Calculate(StructuresCalculation<HeightStructuresInput> calculation,
                                       IAssessmentSection assessmentSection,
                                       HeightStructuresFailureMechanism failureMechanism,
                                       string hydraulicBoundaryDatabaseFilePath)
        {
            base.Calculate(calculation, assessmentSection, failureMechanism, hydraulicBoundaryDatabaseFilePath);

            string calculationName = calculation.Name;

            StructuresOvertoppingCalculationInput input = CreateInput(calculation, failureMechanism, hydraulicBoundaryDatabaseFilePath);

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(hlcdDirectory);

            CalculationServiceHelper.LogCalculationBegin();

            var exceptionThrown = false;

            try
            {
                calculator.Calculate(input);

                if (!canceled && string.IsNullOrEmpty(calculator.LastErrorFileContent))
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
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_no_error_report,
                                        calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_click_details_for_last_error_report_1,
                                        calculationName, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_HeightStructuresCalculation_0_click_details_for_last_error_report_1,
                                    calculationName, lastErrorFileContent);
                }

                log.InfoFormat(Resources.HeightStructuresCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

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
    }
}