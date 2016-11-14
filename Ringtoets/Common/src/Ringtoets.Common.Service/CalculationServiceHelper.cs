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
            foreach (var waningMessage in warningMessages)
            {
                log.Warn(waningMessage);
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

        /// <summary>
        /// Determines whether an error has occurred during the calculation.
        /// </summary>
        /// <param name="canceled">The canceled state of the calculation.</param>
        /// <param name="exceptionThrown">Indicator if there is already an exception thrown in the calculation.</param>
        /// <param name="lastErrorFileContent">The contents of the last error file.</param>
        /// <returns><c>true</c> when a calculation isn't canceled, has not already thrown an exception and 
        /// <paramref name="lastErrorFileContent"/> is set. <c>false</c> otherwise.</returns>
        public static bool ErrorOccurred(bool canceled, bool exceptionThrown, string lastErrorFileContent)
        {
            return !canceled && !exceptionThrown && !string.IsNullOrEmpty(lastErrorFileContent);
        }
    }
}