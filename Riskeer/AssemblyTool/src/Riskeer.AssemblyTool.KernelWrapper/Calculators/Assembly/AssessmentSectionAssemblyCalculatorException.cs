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

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// The exception that is thrown when an error occurs while performing an assessment section assembly.
    /// </summary>
    public class AssessmentSectionAssemblyCalculatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionAssemblyCalculatorException"/> class.
        /// </summary>
        public AssessmentSectionAssemblyCalculatorException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionAssemblyCalculatorException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AssessmentSectionAssemblyCalculatorException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionAssemblyCalculatorException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public AssessmentSectionAssemblyCalculatorException(string message, Exception innerException) : base(message, innerException) {}
    }
}