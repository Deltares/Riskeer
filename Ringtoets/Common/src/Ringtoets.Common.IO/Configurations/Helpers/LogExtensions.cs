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
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="ILog"/> for logging problems occurring during conversion
    /// of configurations.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs an out of range exception that was thrown when converting configuration to actual data model instances.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to use for logging.</param>
        /// <param name="errorMessage">Part of the message that precedes the out of range error message.</param>
        /// <param name="calculationName">The name of the calculation for which the exception was thrown.</param>
        /// <param name="exception">The exception that was thrown.</param>
        public static void LogOutOfRangeException(this ILog log, string errorMessage, string calculationName, ArgumentOutOfRangeException exception)
        {
            log.LogCalculationConversionError($"{errorMessage} {exception.Message}", calculationName);
        }

        /// <summary>
        /// Logs a message for an error which occurred when converting configuration to actual data model instances.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to use for logging.</param>
        /// <param name="errorMessage">The error message to log.</param>
        /// <param name="calculationName">The name of the calculation for which the error occurred.</param>
        public static void LogCalculationConversionError(this ILog log, string errorMessage, string calculationName)
        {
            log.ErrorFormat(Resources.ILogExtensions_LogCalculationConversionError_ErrorMessage_0_Calculation_1_skipped,
                            errorMessage, calculationName);
        }
    }
}