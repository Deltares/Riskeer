﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// The exception that is thrown when a database updater class encounters an error while
    /// parsing a row/line during the read.
    /// </summary>
    [Serializable]
    public class UpdateStorageException : StorageException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStorageException"/> class.
        /// </summary>
        public UpdateStorageException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStorageException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UpdateStorageException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStorageException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public UpdateStorageException(string message, Exception inner) : base(message, inner) {}

        /// <summary>
        /// Initializes a new instance of <see cref="UpdateStorageException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        protected UpdateStorageException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}