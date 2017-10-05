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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan
{
    /// <summary>
    /// The validation result of an Uplift Van calculation.
    /// </summary>
    public class UpliftVanValidationResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanValidationResult"/>.
        /// </summary>
        /// <param name="type">The result type of the validation result</param>
        /// <param name="message">The message of the validation result</param>
        public UpliftVanValidationResult(UpliftVanValidationResultType type, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            ResultType = type;
            Message = message;
        }

        /// <summary>
        /// Gets the type of the validation result message.
        /// </summary>
        public UpliftVanValidationResultType ResultType { get; }

        /// <summary>
        /// Gets the message of the validation result.
        /// </summary>
        public string Message { get; }
    }
}