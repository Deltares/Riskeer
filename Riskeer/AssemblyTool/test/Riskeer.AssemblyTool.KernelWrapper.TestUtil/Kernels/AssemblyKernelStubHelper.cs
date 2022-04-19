// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Reflection;
using Assembly.Kernel.Exceptions;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// Helper class that can be used in kernel stubs.
    /// </summary>
    public static class AssemblyKernelStubHelper
    {
        /// <summary>
        /// Throws an exception when either <paramref name="throwExceptionOnCalculate"/>
        /// or <paramref name="throwAssemblyExceptionOnCalculate"/> is <c>true</c>.
        /// </summary>
        /// <param name="throwExceptionOnCalculate">Indicator whether an
        /// <see cref="Exception"/> should be thrown.</param>
        /// <param name="throwAssemblyExceptionOnCalculate">Indicator whether an
        /// <see cref="AssemblyException"/> should be thrown.</param>
        /// <param name="assemblyError">The error to use in the <see cref="AssemblyException"/>.</param>
        /// <exception cref="Exception">Thrown when <paramref name="throwExceptionOnCalculate"/> is <c>true</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <paramref name="throwAssemblyExceptionOnCalculate"/>
        /// is <c>true</c>.</exception>
        public static void ThrowException(bool throwExceptionOnCalculate, bool throwAssemblyExceptionOnCalculate,
                                          EAssemblyErrors assemblyError)
        {
            if (throwExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            if (throwAssemblyExceptionOnCalculate)
            {
                const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                throw (AssemblyException) Activator.CreateInstance(
                    typeof(AssemblyException), flags, null, new object[]
                    {
                        "entity",
                        assemblyError
                    }, null);
            }
        }
    }
}