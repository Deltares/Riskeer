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
using System.Collections.Generic;
using System.Linq;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class represents the result of a validation.
    /// </summary>
    public class ValidationResult
    {
        private readonly List<string> errorMessages = new List<string>();

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="criticalValidationError">Indicator whether a critical validation error has occurred.</param>
        /// <param name="errorMessages">The error messages for this <see cref="ValidationResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>Any message in <paramref name="errorMessages"/> is <c>null</c>, 
        /// empty or consists of whitespace.</item>
        /// <item><paramref name="errorMessages"/> is empty when <paramref name="criticalValidationError"/>
        /// is <c>true</c>.</item>
        /// </list>
        /// </exception>
        public ValidationResult(bool criticalValidationError, ICollection<string> errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }
            if (errorMessages.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Invalid error message string.");
            }
            if (criticalValidationError && !errorMessages.Any())
            {
                throw new ArgumentException("No messages supplied with critical error.");
            }

            CriticalValidationError = criticalValidationError;
            ValidationWarning = errorMessages.Count > 0 && !criticalValidationError;
            this.errorMessages.AddRange(errorMessages);
        }

        /// <summary>
        /// Gets a value which indicates whether a critical validation error has occurred.
        /// </summary>
        public bool CriticalValidationError { get; }

        /// <summary>
        /// Gets a value which indicates whether there are validation warnings.
        /// </summary>
        public bool ValidationWarning { get; }

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