// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Assembly.Kernel.Exceptions;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Helper class for <see cref="AssemblyErrorMessage"/> that can be used in tests.
    /// </summary>
    public static class AssemblyErrorMessageTestHelper
    {
        /// <summary>
        /// Creates an <see cref="AssemblyErrorMessage"/>.
        /// </summary>
        /// <param name="entityId">The id of the entity on which the error occurred.</param>
        /// <param name="errorCode">The code of the error.</param>
        /// <returns>The created <see cref="AssemblyErrorMessage"/>.</returns>
        public static AssemblyErrorMessage Create(string entityId, EAssemblyErrors errorCode)
        {
            return (AssemblyErrorMessage) Activator.CreateInstance(typeof(AssemblyErrorMessage), entityId, errorCode);
        }
    }
}