// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Riskeer.Common.IO.Exceptions
{
    /// <summary>
    /// The exception that is thrown when transforming imported data to specific data fails.
    /// </summary>
    public class ImportedDataTransformException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedDataTransformException"/> class.
        /// </summary>
        public ImportedDataTransformException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedDataTransformException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ImportedDataTransformException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedDataTransformException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a <c>null</c> reference if no inner exception is specified.</param>
        public ImportedDataTransformException(string message, Exception inner) : base(message, inner) {}
    }
}