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
    /// Service that provides methods for performing Hydra-Ring calculations for design water level.
    /// </summary>
    public class DesignWaterLevelCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DesignWaterLevelCalculationService));
        private IDesignWaterLevelCalculator calculator;
        private bool canceled;

        public bool Validate(string name, string hydraulicBoundaryDatabaseFilePath, ICalculationMessageProvider messageProvider)
        {
            string calculationName = messageProvider.GetCalculationName(name);

            CalculationServiceHelper.LogValidationBeginTime(calculationName);

            string[] validationProblem = ValidateInput(hydraulicBoundaryDatabaseFilePath);

            CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                        validationProblem);

            CalculationServiceHelper.LogValidationEndTime(calculationName);

            return !validationProblem.Any();
        }

        public void Calculate(HydraulicBoundaryLocation hydraulicBoundaryLocation, string hydraulicBoundaryDatabaseFilePath, string ringId, double norm, ICalculationMessageProvider messageProvider)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string calculationName = messageProvider.GetCalculationName(hydraulicBoundaryLocation.Name);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            calculator = HydraRingCalculatorFactory.Instance.CreateDesignWaterLevelCalculator(hlcdDirectory, ringId);

            try
            {
                calculator.Calculate(CreateInput(hydraulicBoundaryLocation, norm));

                hydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) calculator.DesignWaterLevel;
                hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence =
                    RingtoetsCommonDataCalculationService.CalculationConverged(calculator.ReliabilityIndex, norm);

                if (hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence != CalculationConvergence.CalculatedConverged)
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

        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
                canceled = true;
            }
        }

        private AssessmentLevelCalculationInput CreateInput(HydraulicBoundaryLocation hydraulicBoundaryLocation, double norm)
        {
            return new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocation.Id, norm);
        }

        private static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath)
        {
            List<string> validationResult = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult.ToArray();
        }
    }
}