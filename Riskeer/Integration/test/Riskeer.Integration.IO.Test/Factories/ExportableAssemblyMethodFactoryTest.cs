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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssemblyMethodFactoryTest
    {
        [Test]
        public void Create_InvalidAssemblyMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const AssemblyMethod assemblyMethod = (AssemblyMethod) 999;

            // ExportableAssemblyMethodFactory
            void Call() => ExportableAssemblyMethodFactory.Create(assemblyMethod);

            // Assert
            var message = $"The value of argument 'assemblyMethod' ({assemblyMethod}) is invalid for Enum type '{nameof(AssemblyMethod)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(AssemblyMethod.BOI0A1, ExportableAssemblyMethod.BOI0A1)]
        [TestCase(AssemblyMethod.BOI0A2, ExportableAssemblyMethod.BOI0A2)]
        [TestCase(AssemblyMethod.BOI0B1, ExportableAssemblyMethod.BOI0B1)]
        [TestCase(AssemblyMethod.BOI0C1, ExportableAssemblyMethod.BOI0C1)]
        [TestCase(AssemblyMethod.BOI1A1, ExportableAssemblyMethod.BOI1A1)]
        [TestCase(AssemblyMethod.BOI1A2, ExportableAssemblyMethod.BOI1A2)]
        [TestCase(AssemblyMethod.Manual, ExportableAssemblyMethod.Manual)]
        [TestCase(AssemblyMethod.BOI2A1, ExportableAssemblyMethod.BOI2A1)]
        [TestCase(AssemblyMethod.BOI2B1, ExportableAssemblyMethod.BOI2B1)]
        [TestCase(AssemblyMethod.BOI3A1, ExportableAssemblyMethod.BOI3A1)]
        [TestCase(AssemblyMethod.BOI3B1, ExportableAssemblyMethod.BOI3B1)]
        [TestCase(AssemblyMethod.BOI3C1, ExportableAssemblyMethod.BOI3C1)]
        public void Create_WithAssemblyMethod_ReturnsExpectedValues(AssemblyMethod assemblyMethod,
                                                                    ExportableAssemblyMethod expectedAssemblyMethod)
        {
            // Call
            ExportableAssemblyMethod exportableAssemblyMethod = ExportableAssemblyMethodFactory.Create(assemblyMethod);

            // Assert
            Assert.AreEqual(expectedAssemblyMethod, exportableAssemblyMethod);
        }
    }
}