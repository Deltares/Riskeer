﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Creators;

namespace Riskeer.Integration.IO.Test.Creators
{
    [TestFixture]
    public class SerializableAssemblyMethodCreatorTest
    {
        [Test]
        public void Create_InvalidAssemblyMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const ExportableAssemblyMethod assemblyMethod = (ExportableAssemblyMethod) 999;

            // Call
            void Call() => SerializableAssemblyMethodCreator.Create(assemblyMethod);

            // Assert
            var message = $"The value of argument 'assemblyMethod' ({assemblyMethod}) is invalid for Enum type '{nameof(ExportableAssemblyMethod)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(ExportableAssemblyMethod.WBI1B1, SerializableAssemblyMethod.WBI1B1)]
        [TestCase(ExportableAssemblyMethod.WBI2C1, SerializableAssemblyMethod.WBI2C1)]
        [TestCase(ExportableAssemblyMethod.WBI3A1, SerializableAssemblyMethod.WBI3A1)]
        [TestCase(ExportableAssemblyMethod.WBI3B1, SerializableAssemblyMethod.WBI3B1)]
        [TestCase(ExportableAssemblyMethod.WBI3C1, SerializableAssemblyMethod.WBI3C1)]
        public void Create_WithAssemblyMethod_ReturnsExpectedValues(ExportableAssemblyMethod assemblyMethod,
                                                                    SerializableAssemblyMethod expectedAssemblyMethod)
        {
            // Call
            SerializableAssemblyMethod serializableAssemblyMethod = SerializableAssemblyMethodCreator.Create(assemblyMethod);

            // Assert
            Assert.AreEqual(expectedAssemblyMethod, serializableAssemblyMethod);
        }
    }
}