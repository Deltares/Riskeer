// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Kernels
{
    [TestFixture]
    public class AssemblyToolKernelWrapperFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IAssemblyToolKernelFactory factory = AssemblyToolKernelWrapperFactory.Instance;

            // Assert
            Assert.IsInstanceOf<AssemblyToolKernelWrapperFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IAssemblyToolKernelFactory firstFactory = AssemblyToolKernelWrapperFactory.Instance;
            AssemblyToolKernelWrapperFactory.Instance = null;

            // Call
            IAssemblyToolKernelFactory secondFactory = AssemblyToolKernelWrapperFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestAssemblyToolKernelFactory();
            AssemblyToolKernelWrapperFactory.Instance = firstFactory;

            // Call
            IAssemblyToolKernelFactory secondFactory = AssemblyToolKernelWrapperFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateAssemblyCategoriesKernel_Always_ReturnsAssemblyCategoriesKernelWrapper()
        {
            // Setup
            IAssemblyToolKernelFactory factory = AssemblyToolKernelWrapperFactory.Instance;

            // Call
            IAssemblyCategoriesKernel assemblyCategoriesKernel = factory.CreateAssemblyCategoriesKernel();

            // Assert
            Assert.IsInstanceOf<AssemblyCategoriesKernelWrapper>(assemblyCategoriesKernel);
        }
    }
}