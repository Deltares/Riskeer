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
using System.Runtime.Serialization;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Exceptions
{
    /// <summary>
    /// Exception thrown when something went wrong while trying to read a <see cref="ISoilProfile"/>.
    /// </summary>
    [Serializable]
    public class SoilProfileReadException : Exception
    {
        private const string profileNameKey = nameof(ProfileName);

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileReadException"/> class.
        /// </summary>
        public SoilProfileReadException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileReadException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public SoilProfileReadException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileReadException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="profileName">The name of the profile for which this exception was thrown.</param>
        public SoilProfileReadException(string message, string profileName)
            : base(message)
        {
            ProfileName = profileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileReadException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public SoilProfileReadException(string message, Exception inner)
            : base(message, inner) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileReadException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="profileName">The name of the profile for which this exception was thrown.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public SoilProfileReadException(string message, string profileName, Exception innerException)
            : base(message, innerException)
        {
            ProfileName = profileName;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SoilProfileReadException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        private SoilProfileReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ProfileName = info.GetString(profileNameKey);
        }

        /// <summary>
        /// The name of the profile for which this exception was thrown.
        /// </summary>
        public string ProfileName
        {
            get
            {
                return (string) Data[profileNameKey];
            }
            private set
            {
                Data[profileNameKey] = value;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(profileNameKey, ProfileName);
        }
    }
}