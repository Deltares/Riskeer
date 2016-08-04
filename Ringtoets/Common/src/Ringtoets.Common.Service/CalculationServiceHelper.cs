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
using System.Linq;
using log4net;
using Ringtoets.Common.Service.Properties;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// This class defines helper methods for performing calculations.
    /// </summary>
    public static class CalculationServiceHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CalculationServiceHelper));

        /// <summary>
        /// Method for performing validation. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="name">The name of the calculation.</param>
        /// <param name="validationFunc">The method used to perform the validation.</param>
        /// <returns><c>True</c> if <paramref name="validationFunc"/> has no validation errors; <c>False</c> otherwise.</returns>
        public static bool PerformValidation(string name, Func<string[]> validationFunc)
        {
            LogValidationBeginTime(name);

            var inputValidationResults = validationFunc();

            if (inputValidationResults.Any())
            {
                LogMessagesAsError(Resources.Error_in_validation_0, inputValidationResults);
            }

            LogValidationEndTime(name);

            return !inputValidationResults.Any();
        }

        /// <summary>
        /// Method for performing calculations. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="name">The name of the calculation.</param>
        /// <param name="calculationFunc">The method used to perform the calculation.</param>
        /// <remarks>When <paramref name="calculationFunc"/> throws an exception, this will not be caught in this method.</remarks>
        public static void PerformCalculation(string name, Action calculationFunc)
        {
            LogCalculationBeginTime(name);

            try
            {
                calculationFunc();
            }
            finally
            {
                LogCalculationEndTime(name);
            }
        }

        /// <summary>
        /// Logs messages as errors.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="errorMessages">The messages to log.</param>
        public static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                log.ErrorFormat(format, errorMessage);
            }
        }

        /// <summary>
        /// Logs messages as warnings.
        /// </summary>
        /// <param name="warningMessages">The messages to log.</param>
        public static void LogMessagesAsWarning(params string[] warningMessages)
        {
            foreach (var errorMessage in warningMessages)
            {
                log.Warn(errorMessage);
            }
        }

        /// <summary>
        /// Logs the begin time of the validation.
        /// </summary>
        /// <param name="name">The name of the object being validated.</param>
        public static void LogValidationBeginTime(string name)
        {
            log.Info(string.Format(Resources.Validation_Subject_0_started_Time_1_,
                                                              name, DateTimeService.CurrentTimeAsString));
        }

        /// <summary>
        /// Logs the end time of the validation.
        /// </summary>
        /// <param name="name">The name of the object being validated.</param>
        public static void LogValidationEndTime(string name)
        {
            log.Info(string.Format(Resources.Validation_Subject_0_ended_Time_1_,
                                                              name, DateTimeService.CurrentTimeAsString));
        }

        /// <summary>
        /// Logs the begin time of the calculation.
        /// </summary>
        /// <param name="name">The name of the calculation being started.</param>
        public static void LogCalculationBeginTime(string name)
        {
            log.Info(string.Format(Resources.Calculation_Subject_0_started_Time_1_,
                                                                 name, DateTimeService.CurrentTimeAsString));
        }

        /// <summary>
        /// Logs the end time of the calculation.
        /// </summary>
        /// <param name="name">The name of the calculation that has ended.</param>
        public static void LogCalculationEndTime(string name)
        {
            log.Info(string.Format(Resources.Calculation_Subject_0_ended_Time_1_,
                                                              name, DateTimeService.CurrentTimeAsString));
        }
    }
}