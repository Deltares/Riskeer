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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service.IllustrationPoints;
using Riskeer.Common.Service.MessageProviders;
using Riskeer.Common.Service.Properties;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Exceptions;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;

namespace Riskeer.Common.Service.Structures
{
    /// <summary>
    /// Service that provides generic logic for performing Hydra-Ring calculations for structures.
    /// </summary>
    /// <typeparam name="TStructureValidationRules">The type of the validation rules.</typeparam>
    /// <typeparam name="TStructureInput">The structure input type.</typeparam>
    /// <typeparam name="TStructure">The structure type.</typeparam>
    /// <typeparam name="TGeneralInput">The general input type.</typeparam>
    /// <typeparam name="TCalculationInput">The calculation input type.</typeparam>
    public abstract class StructuresCalculationServiceBase
    <TStructureValidationRules, TStructureInput,
     TStructure, TGeneralInput, TCalculationInput>
        where TStructureValidationRules : IStructuresValidationRulesRegistry<TStructureInput, TStructure>, new()
        where TStructureInput : StructuresInputBase<TStructure>, new()
        where TStructure : StructureBase
        where TGeneralInput : class
        where TCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StructuresCalculationServiceBase<TStructureValidationRules, TStructureInput,
                                                                    TStructure, TGeneralInput, TCalculationInput>));

        private readonly IStructuresCalculationMessageProvider messageProvider;

        private IStructuresCalculator<TCalculationInput> calculator;
        private bool canceled;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresCalculationServiceBase{TStructureValidationRules,TStructureInput, TStructure,TGeneralInput,TCalculationInput}"/>.
        /// </summary>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageProvider"/>
        /// is <c>null</c>.</exception>
        protected StructuresCalculationServiceBase(IStructuresCalculationMessageProvider messageProvider)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            this.messageProvider = messageProvider;
        }

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an invalid enum value is encountered.</exception>
        /// <exception cref="NotSupportedException">Thrown when an unsupported enum value is encountered.</exception>
        public static bool Validate(StructuresCalculation<TStructureInput> calculation, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogValidationBegin();
            string[] messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(messages);
            CalculationServiceHelper.LogValidationEnd();

            return !messages.Any();
        }

        /// <summary>
        /// Performs a structures calculation based on the supplied <see cref="StructuresCalculation{T}"/> and sets <see cref="StructuresCalculation{T}.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> that holds all the information required to perform the calculation.</param>
        /// <param name="generalInput">The general inputs used in the calculations.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an unexpected
        /// enum value is encountered.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public void Calculate(StructuresCalculation<TStructureInput> calculation,
                              TGeneralInput generalInput,
                              HydraulicBoundaryCalculationSettings calculationSettings)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            TCalculationInput input = CreateInput(calculation.InputParameters,
                                                  generalInput,
                                                  calculationSettings.HydraulicBoundaryDatabaseFilePath,
                                                  !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory));

            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresCalculator<TCalculationInput>(
                HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings));
            string calculationName = calculation.Name;

            CalculationServiceHelper.LogCalculationBegin();

            var exceptionThrown = false;
            try
            {
                PerformCalculation(calculation, input);
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorFileContent = calculator.LastErrorFileContent;

                    string message = string.IsNullOrEmpty(lastErrorFileContent)
                                         ? messageProvider.GetCalculationFailedMessage(calculationName)
                                         : messageProvider.GetCalculationFailedWithErrorReportMessage(calculationName, lastErrorFileContent);

                    log.Error(message);

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
                    log.Error(messageProvider.GetCalculationFailedWithErrorReportMessage(calculationName, lastErrorFileContent));
                }

                log.Info(messageProvider.GetCalculationPerformedMessage(calculator.OutputDirectory));

                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any currently running structures calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Creates the input for a structures calculation.
        /// </summary>
        /// <param name="structureInput">The structure input to create the calculation input for.</param>
        /// <param name="generalInput">The <see cref="TGeneralInput"/> that is used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A <see cref="TCalculationInput"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an unexpected
        /// enum value is encountered.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        protected abstract TCalculationInput CreateInput(TStructureInput structureInput,
                                                         TGeneralInput generalInput,
                                                         string hydraulicBoundaryDatabaseFilePath,
                                                         bool usePreprocessor);

        /// <summary>
        /// Performs a structures calculation.
        /// </summary>
        /// <param name="calculation">The structures calculation to use.</param>
        /// <param name="calculationInput">The HydraRing calculation input used for the calculation.</param>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private void PerformCalculation(StructuresCalculation<TStructureInput> calculation,
                                        TCalculationInput calculationInput)
        {
            calculator.Calculate(calculationInput);

            if (canceled || !string.IsNullOrEmpty(calculator.LastErrorFileContent))
            {
                return;
            }

            SetOutput(calculation, calculator.ExceedanceProbabilityBeta);
        }

        /// <summary>
        /// Sets the calculated output to the calculation object.
        /// </summary>
        /// <param name="calculation">The calculation to set the output for.</param>
        /// <param name="reliability">The reliability of the calculation.</param>
        private void SetOutput(StructuresCalculation<TStructureInput> calculation,
                               double reliability)
        {
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult =
                    calculation.InputParameters.ShouldIllustrationPointsBeCalculated
                        ? GetGeneralResult(calculator.IllustrationPointsResult)
                        : null;
            }
            catch (ArgumentException e)
            {
                log.Warn(string.Format(Resources.CalculationService_Error_in_reading_illustrationPoints_for_CalculationName_0_with_ErrorMessage_1,
                                       calculation.Name,
                                       e.Message));
            }

            calculation.Output = new StructuresOutput(reliability, generalResult);
        }

        /// <summary>
        /// Gets a <see cref="GeneralResult{T}"/> based on the information 
        /// of <paramref name="hydraRingGeneralResult"/>.
        /// </summary>
        /// <param name="hydraRingGeneralResult">The <see cref="HydraRingGeneralResult"/> to base the 
        /// <see cref="GeneralResult{T}"/> to create on.</param>
        /// <returns>A <see cref="GeneralResult{T}"/>, or <c>null</c> if the 
        /// <paramref name="hydraRingGeneralResult"/> could not be converted.</returns>
        private GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResult(HydraRingGeneralResult hydraRingGeneralResult)
        {
            if (hydraRingGeneralResult == null)
            {
                log.Warn(calculator.IllustrationPointsParserErrorMessage);
                return null;
            }

            try
            {
                return GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(hydraRingGeneralResult);
            }
            catch (IllustrationPointConversionException e)
            {
                log.Warn(Resources.SetGeneralResult_Converting_IllustrationPointResult_Failed, e);
            }

            return null;
        }

        /// <summary>
        /// Validates the input.
        /// </summary>
        /// <param name="input">The input of the calculation.</param>
        /// <param name="assessmentSection">The assessment section that holds
        /// information about the hydraulic boundary database.</param>
        /// <returns>An <see cref="Array"/> of validation messages.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an unexpected
        /// enum value is encountered.</exception>
        private static string[] ValidateInput(TStructureInput input, IAssessmentSection assessmentSection)
        {
            var validationResults = new List<string>();

            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory();
            string databaseFilePathValidationProblem = HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
            if (!string.IsNullOrEmpty(databaseFilePathValidationProblem))
            {
                validationResults.Add(databaseFilePathValidationProblem);
            }

            string preprocessorDirectoryValidationProblem = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(preprocessorDirectory);
            if (!string.IsNullOrEmpty(preprocessorDirectoryValidationProblem))
            {
                validationResults.Add(preprocessorDirectoryValidationProblem);
            }

            if (validationResults.Any())
            {
                return validationResults.ToArray();
            }

            if (input.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(Resources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            if (input.Structure == null)
            {
                validationResults.Add(Resources.StructuresCalculationService_ValidateInput_No_Structure_selected);
            }
            else
            {
                IEnumerable<ValidationRule> validationRules = new TStructureValidationRules().GetValidationRules(input);

                foreach (ValidationRule validationRule in validationRules)
                {
                    validationResults.AddRange(validationRule.Validate());
                }
            }

            return validationResults.ToArray();
        }
    }
}