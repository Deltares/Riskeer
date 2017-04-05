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
using log4net;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="ILog"/> for logging problems occuring during import or export of configurations.
    /// </summary>
    public static class ILogExtensions
    {
        public static void LogOutOfRangeException(this ILog log, string errorMessage, string calculationName, ArgumentOutOfRangeException e)
        {
            log.LogCalculationConversionError($"{errorMessage} {e.Message}", calculationName);
        }

        public static void LogCalculationConversionError(this ILog log, string message, string calculationName)
        {
            log.ErrorFormat(Resources.CalculationConfigurationImporter_ValidateCalculation_ErrorMessage_0_Calculation_1_skipped,
                            message, calculationName);
        }
    }
}