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
using System.Collections.Generic;
using System.Linq;

namespace Riskeer.Common.IO.Structures
{
    /// <summary>
    /// This class represents the result of a validation.
    /// </summary>
    public class ValidationResult
    {
        private readonly List<string> errorMessages = new List<string>();

        /// <summary>
        /// Create a new instance of <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="errorMessages">The error messages for this <see cref="ValidationResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any message in <paramref name="errorMessages"/> is <c>null</c>, 
        /// empty or consists of whitespace.</exception>
        public ValidationResult(IEnumerable<string> errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            if (errorMessages.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Invalid error message string.");
            }

            IsValid = !errorMessages.Any();
            this.errorMessages.AddRange(errorMessages);
        }

        /// <summary>
        /// Gets a value which indicates whether the validation subject is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the error messages resulting from the validation.
        /// </summary>
        public IEnumerable<string> ErrorMessages
        {
            get
            {
                return errorMessages;
            }
        }
    }
}