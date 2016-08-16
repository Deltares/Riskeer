﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Piping.KernelWrapper
{
    /// <summary>
    /// Exception thrown when something went wrong in the <see cref="PipingCalculator"/>
    /// </summary>
    [Serializable]
    public class PipingCalculatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculatorException"/> class.
        /// </summary>
        public PipingCalculatorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculatorException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PipingCalculatorException(string message) : base (message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public PipingCalculatorException(string message, Exception innerException) : base(message, innerException) {}
    }
}