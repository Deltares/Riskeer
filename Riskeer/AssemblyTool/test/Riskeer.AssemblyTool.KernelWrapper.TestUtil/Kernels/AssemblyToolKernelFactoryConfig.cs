// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// This class can be used to set a temporary <see cref="TestAssemblyToolKernelFactory"/> 
    /// for <see cref="AssemblyToolKernelFactory.Instance"/> while testing. 
    /// Disposing an instance of this class will revert the <see cref="AssemblyToolKernelFactory.Instance"/>.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new AssemblyToolKernelFactoryConfig())
    /// {
    ///     var testFactory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
    /// 
    ///     // Perform tests with testFactory
    /// }
    /// </code>
    /// </example>
    public class AssemblyToolKernelFactoryConfig : IDisposable
    {
        private readonly IAssemblyToolKernelFactory previousFactory;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyToolKernelFactoryConfig"/>.
        /// Sets a <see cref="TestAssemblyToolKernelFactory"/> to 
        /// <see cref="AssemblyToolKernelFactory.Instance"/>
        /// </summary>
        public AssemblyToolKernelFactoryConfig()
        {
            previousFactory = AssemblyToolKernelFactory.Instance;
            AssemblyToolKernelFactory.Instance = new TestAssemblyToolKernelFactory();
        }

        /// <summary>
        /// Reverts the <see cref="AssemblyToolKernelFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="AssemblyToolKernelFactoryConfig"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AssemblyToolKernelFactory.Instance = previousFactory;
            }
        }
    }
}