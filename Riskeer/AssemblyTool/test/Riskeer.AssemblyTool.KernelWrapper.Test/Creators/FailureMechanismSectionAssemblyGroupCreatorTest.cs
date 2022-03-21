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
using System.Collections.Generic;
using System.ComponentModel;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupCreatorTest
    {
        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_InterpretationCategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyGroupCreator.CreateFailureMechanismSectionAssemblyGroupBoundaries(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("interpretationCategories", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_WithValidInterpretationCategories_ReturnsExpectedFailureMechanismSectionAssemblyGroups()
        {
            // Setup
            var random = new Random(11);

            var groups = new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0), new Probability(0.25)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.25), new Probability(0.5)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.5), new Probability(0.75)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.75), new Probability(1))
            });

            // Call
            IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> assemblyGroups =
                FailureMechanismSectionAssemblyGroupCreator.CreateFailureMechanismSectionAssemblyGroupBoundaries(groups);

            // Assert
            FailureMechanismSectionAssemblyGroupBoundariesAssert.AssertFailureMechanismSectionAssemblyGroupBoundaries(groups, assemblyGroups);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_InvalidInterpretationCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var groups = new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory((EInterpretationCategory) 99, new Probability(0), new Probability(1))
            });

            // Call
            void Call() => FailureMechanismSectionAssemblyGroupCreator.CreateFailureMechanismSectionAssemblyGroupBoundaries(groups);

            // Assert
            var exceptionMessage = $"The value of argument 'interpretationCategory' (99) is invalid for Enum type '{nameof(EInterpretationCategory)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, exceptionMessage);
        }
    }
}