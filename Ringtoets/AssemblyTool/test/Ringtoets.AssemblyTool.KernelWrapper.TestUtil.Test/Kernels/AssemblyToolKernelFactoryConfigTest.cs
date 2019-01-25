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
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels
{
    [TestFixture]
    public class AssemblyToolKernelFactoryConfigTest
    {
        [Test]
        public void Constructor_NewInstanceCanBeDisposed()
        {
            // Call
            var factory = new AssemblyToolKernelFactoryConfig();

            // Assert
            Assert.IsInstanceOf<IDisposable>(factory);
            Assert.DoesNotThrow(() => factory.Dispose());
        }

        [Test]
        public void Constructor_SetsTestFactoryForAssemblyToolKernelFactory()
        {
            // Call
            using (new AssemblyToolKernelFactoryConfig())
            {
                // Assert
                Assert.IsInstanceOf<TestAssemblyToolKernelFactory>(AssemblyToolKernelFactory.Instance);
            }
        }

        [Test]
        public void Dispose_Always_ResetsFactoryToPreviousValue()
        {
            // Setup
            IAssemblyToolKernelFactory expectedFactory = AssemblyToolKernelFactory.Instance;

            // Call
            using (new AssemblyToolKernelFactoryConfig()) {}

            // Assert
            Assert.AreSame(expectedFactory, AssemblyToolKernelFactory.Instance);
        }
    }
}