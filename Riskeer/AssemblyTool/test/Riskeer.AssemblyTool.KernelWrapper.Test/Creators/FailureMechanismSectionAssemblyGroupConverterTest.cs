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

using System.ComponentModel;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupConverterTest
    {
        [Test]
        public void ConvertTo_InvalidCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const EInterpretationCategory category = (EInterpretationCategory) 99;

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category);

            // Assert
            var expectedMessage = $"The value of argument 'category' ({category}) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(EInterpretationCategory.NotDominant, FailureMechanismSectionAssemblyGroup.NotDominant)]
        [TestCase(EInterpretationCategory.III, FailureMechanismSectionAssemblyGroup.III)]
        [TestCase(EInterpretationCategory.II, FailureMechanismSectionAssemblyGroup.II)]
        [TestCase(EInterpretationCategory.I, FailureMechanismSectionAssemblyGroup.I)]
        [TestCase(EInterpretationCategory.Zero, FailureMechanismSectionAssemblyGroup.Zero)]
        [TestCase(EInterpretationCategory.IMin, FailureMechanismSectionAssemblyGroup.IMin)]
        [TestCase(EInterpretationCategory.IIMin, FailureMechanismSectionAssemblyGroup.IIMin)]
        [TestCase(EInterpretationCategory.IIIMin, FailureMechanismSectionAssemblyGroup.IIIMin)]
        [TestCase(EInterpretationCategory.Dominant, FailureMechanismSectionAssemblyGroup.Dominant)]
        [TestCase(EInterpretationCategory.Gr, FailureMechanismSectionAssemblyGroup.Gr)]
        public void ConvertTo_ValidCategory_ReturnsExpectedValue(EInterpretationCategory category, FailureMechanismSectionAssemblyGroup expectedAssemblyGroup)
        {
            // Call
            FailureMechanismSectionAssemblyGroup convertedAssemblyGroup = FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category);

            // Assert
            Assert.AreEqual(expectedAssemblyGroup, convertedAssemblyGroup);
        }

        [Test]
        public void ConvertFrom_InvalidFailureMechanismSectionAssemblyGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismSectionAssemblyGroup assemblyGroup = (FailureMechanismSectionAssemblyGroup) 99;

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupConverter.ConvertFrom(assemblyGroup);

            // Assert
            var expectedMessage = $"The value of argument 'assemblyGroup' ({assemblyGroup}) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyGroup.NotDominant, EInterpretationCategory.NotDominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.III, EInterpretationCategory.III)]
        [TestCase(FailureMechanismSectionAssemblyGroup.II, EInterpretationCategory.II)]
        [TestCase(FailureMechanismSectionAssemblyGroup.I, EInterpretationCategory.I)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Zero, EInterpretationCategory.Zero)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IMin, EInterpretationCategory.IMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIMin, EInterpretationCategory.IIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.IIIMin, EInterpretationCategory.IIIMin)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Dominant, EInterpretationCategory.Dominant)]
        [TestCase(FailureMechanismSectionAssemblyGroup.Gr, EInterpretationCategory.Gr)]
        public void ConvertFrom_ValidAssemblyGroup_ReturnsExpectedValue(FailureMechanismSectionAssemblyGroup assemblyGroup, EInterpretationCategory expectedCategory)
        {
            // Call
            EInterpretationCategory convertedCategory = FailureMechanismSectionAssemblyGroupConverter.ConvertFrom(assemblyGroup);

            // Assert
            Assert.AreEqual(expectedCategory, convertedCategory);
        }
    }
}