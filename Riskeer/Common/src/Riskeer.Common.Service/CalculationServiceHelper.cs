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
        /// Logs messages as errors with formatting.
        /// </summary>
        /// <param name="format">The format for the message.</param>
        /// <param name="errorMessages">The messages to log.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public static void LogMessagesAsError(string format, string[] errorMessages)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            foreach (string errorMessage in errorMessages)
            {
                log.ErrorFormat(format, errorMessage);
            }
        }

        /// <summary>
        /// Logs messages as errors.
        /// </summary>
        /// <param name="errorMessages">The messages to log.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/>
        /// is <c>null</c>.</exception>
        public static void LogMessagesAsError(string[] errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            LogMessagesAsError("{0}", errorMessages);
        }

        /// <summary>
        /// Logs messages as warnings.
        /// </summary>
        /// <param name="warningMessages">The messages to log.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="warningMessages"/>
        /// is <c>null</c>.</exception>
        public static void LogMessagesAsWarning(string[] warningMessages)
        {
            if (warningMessages == null)
            {
                throw new ArgumentNullException(nameof(warningMessages));
            }

            foreach (string waningMessage in warningMessages)
            {
                log.Warn(waningMessage);
            }
        }

        /// <summary>
        /// Logs the begin of a validation action.
        /// </summary>
        public static void LogValidationBegin()
        {
            log.Info(Resources.Validation_started);
        }

        /// <summary>
        /// Logs the end of a validation action.
        /// </summary>
        public static void LogValidationEnd()
        {
            log.Info(Resources.Validation_ended);
        }

        /// <summary>
        /// Logs the begin of a calculation action.
        /// </summary>
        public static void LogCalculationBegin()
        {
            log.Info(Resources.Calculation_started);
        }

        /// <summary>
        /// Logs the end of a calculation action.
        /// </summary>
        public static void LogCalculationEnd()
        {
            log.Info(Resources.Calculation_ended);
        }

        /// <summary>
        /// Determines whether an error has occurred during the calculation.
        /// </summary>
        /// <param name="canceled">The canceled state of the calculation.</param>
        /// <param name="exceptionThrown">Indicator if there is already an exception thrown in the calculation.</param>
        /// <param name="lastErrorFileContent">The contents of the last error file.</param>
        /// <returns><c>true</c> when a calculation isn't canceled, has not already thrown an exception and 
        /// <paramref name="lastErrorFileContent"/> is set. <c>false</c> otherwise.</returns>
        public static bool HasErrorOccurred(bool canceled, bool exceptionThrown, string lastErrorFileContent)
        {
            return !canceled && !exceptionThrown && !string.IsNullOrEmpty(lastErrorFileContent);
        }

        /// <summary>
        /// Logs message and exception as error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public static void LogExceptionAsError(string message, Exception exception)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            log.Error(message, exception);
        }
    }
}