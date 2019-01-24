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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionListCreatorTest
    {
        [Test]
        public void Create_FailureMechanismSectionsCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionListCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionsCollection", exception.ParamName);
        }

        [Test]
        public void Create_WithFailureMechanism_ReturnFailureMechanismSectionLists()
        {
            // Setup
            var random = new Random(21);
            var combinedAssemblyFailureMechanismInputs = new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new CombinedAssemblyFailureMechanismSection(1, 2, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                }
            };

            // Call
            IEnumerable<FailureMechanismSectionList> failureMechanismSectionLists = FailureMechanismSectionListCreator.Create(combinedAssemblyFailureMechanismInputs);

            // Assert
            CombinedFailureMechanismSectionsInputAssert.AssertCombinedFailureMechanismInput(combinedAssemblyFailureMechanismInputs, failureMechanismSectionLists);
        }

        [Test]
        public void Create_InvalidGroup_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionListCreator.Create(new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, (FailureMechanismSectionAssemblyCategoryGroup) 99)
                }
            });

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EFmSectionCategory.Gr)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EFmSectionCategory.VIIv)]
        public void Create_ValidGroup_ReturnEFmSectionCategory(FailureMechanismSectionAssemblyCategoryGroup originalGroup, EFmSectionCategory expectedGroup)
        {
            // Call
            IEnumerable<FailureMechanismSectionList> output = FailureMechanismSectionListCreator.Create(new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, originalGroup)
                }
            });

            // Assert
            Assert.AreEqual(expectedGroup, ((FmSectionWithDirectCategory) output.Single().Sections.Single()).Category);
        }
    }
}