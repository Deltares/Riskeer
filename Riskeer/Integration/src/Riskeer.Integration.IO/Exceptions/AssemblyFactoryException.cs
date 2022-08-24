﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Riskeer.Integration.IO.Exceptions
{
    /// <summary>
    /// Exception thrown when the assembly result cannot be created.
    /// </summary>
    [Serializable]
    public class AssemblyFactoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFactoryException"/> class.
        /// </summary>
        public AssemblyFactoryException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFactoryException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AssemblyFactoryException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFactoryException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public AssemblyFactoryException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of <see cref="AssemblyFactoryException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        protected AssemblyFactoryException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}