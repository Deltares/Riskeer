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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for marginal wave statistics.
    /// </summary>
    public class WaveHeightCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationService));

        private IWaveHeightCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="hydraulicBoundaryDatabaseFilePath"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="name">The name of the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        /// <returns><c>True</c>c> if there were no validation errors; <c>False</c>c> otherwise.</returns>
        public static bool Validate(string name, string hydraulicBoundaryDatabaseFilePath, ICalculationMessageProvider messageProvider)
        {
            string calculationName = messageProvider.GetCalculationName(name);

            CalculationServiceHelper.LogValidationBeginTime(calculationName);

            string[] validationProblems = ValidateInput(hydraulicBoundaryDatabaseFilePath);

            CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                        validationProblems);

            CalculationServiceHelper.LogValidationEndTime(calculationName);

            return !validationProblems.Any();
        }

        /// <summary>
        /// Performs a calculation for wave height.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="ringId">The id of the assessment section.</param>
        /// <param name="norm">The norm of the assessment section.</param>
        /// <param name="messageProvider">The object which is used to build log messages.</param>
        public void Calculate(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                              string hydraulicBoundaryDatabaseFilePath,
                              string ringId,
                              double norm,
                              ICalculationMessageProvider messageProvider)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string calculationName = messageProvider.GetCalculationName(hydraulicBoundaryLocation.Name);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            calculator = HydraRingCalculatorFactory.Instance.CreateWaveHeightCalculator(hlcdDirectory, ringId);

            try
            {
                calculator.Calculate(CreateInput(hydraulicBoundaryLocation, norm));

                hydraulicBoundaryLocation.WaveHeight = (RoundedDouble) calculator.WaveHeight;
                hydraulicBoundaryLocation.WaveHeightCalculationConvergence =
                    RingtoetsCommonDataCalculationService.CalculationConverged(calculator.ReliabilityIndex, norm);

                if (hydraulicBoundaryLocation.WaveHeightCalculationConvergence != CalculationConvergence.CalculatedConverged)
                {
                    log.Warn(messageProvider.GetCalculatedNotConvergedMessage(hydraulicBoundaryLocation.Name));
                }
            }
            catch (HydraRingFileParserException)
            {
                if (!canceled)
                {
                    log.Error(messageProvider.GetCalculationFailedMessage(hydraulicBoundaryLocation.Name));
                    throw;
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        /// <summary>
        /// Cancels any currently running wave height calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
                canceled = true;
            }
        }

        private WaveHeightCalculationInput CreateInput(HydraulicBoundaryLocation hydraulicBoundaryLocation, double norm)
        {
            return new WaveHeightCalculationInput(1, hydraulicBoundaryLocation.Id, norm);
        }

        private static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath)
        {
            var validationResult = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult.ToArray();
        }
    }
}