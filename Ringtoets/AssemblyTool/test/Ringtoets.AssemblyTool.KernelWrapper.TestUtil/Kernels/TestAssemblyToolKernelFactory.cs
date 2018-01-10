﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.CategoryBoundaries;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.CategoryBoundaries;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// Factory that creates assembly tool kernel stubs for testing purposes.
    /// </summary>
    public class TestAssemblyToolKernelFactory : IAssemblyToolKernelFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolKernelFactory"/>.
        /// </summary>
        public TestAssemblyToolKernelFactory()
        {
            LastCreatedAssemblyCategoryBoundariesKernel = new AssemblyCategoryBoundariesKernelStub();
        }

        /// <summary>
        /// The last created assembly category boundaries kernel.
        /// </summary>
        public IAssemblyCategoryBoundariesKernel LastCreatedAssemblyCategoryBoundariesKernel { get; }

        public IAssemblyCategoryBoundariesKernel CreateAssemblyCategoryBoundariesKernel()
        {
            return LastCreatedAssemblyCategoryBoundariesKernel;
        }
    }
}