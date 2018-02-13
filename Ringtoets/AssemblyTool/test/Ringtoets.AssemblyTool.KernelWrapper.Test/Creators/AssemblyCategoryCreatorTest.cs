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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyCategoryCreatorTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyCategories_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithOutput_ReturnAssessmentSectionAssemblyCategoryResult()
        {
            // Setup
            var random = new Random(11);

            var output = new CalculationOutput<AssessmentSectionCategory[]>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.Next(1)), new Probability(random.Next(1, 2))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.Next(1)), new Probability(random.Next(1, 2))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.Next(1)), new Probability(random.Next(1, 2))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.Next(1)), new Probability(random.Next(1, 2)))
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(output);

            // Assert
            AssessmentSectionAssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(output, result);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithInvalidAssessmentSectionAssemblyCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var output = new CalculationOutput<AssessmentSectionCategory[]>(new[]
            {
                new AssessmentSectionCategory((AssessmentSectionCategoryGroup) 99, new Probability(0), new Probability(0))
            });

            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(output);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'AssessmentSectionCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCase(AssessmentSectionCategoryGroup.APlus, AssessmentSectionAssemblyCategoryGroup.APlus)]
        [TestCase(AssessmentSectionCategoryGroup.A, AssessmentSectionAssemblyCategoryGroup.A)]
        [TestCase(AssessmentSectionCategoryGroup.B, AssessmentSectionAssemblyCategoryGroup.B)]
        [TestCase(AssessmentSectionCategoryGroup.C, AssessmentSectionAssemblyCategoryGroup.C)]
        [TestCase(AssessmentSectionCategoryGroup.D, AssessmentSectionAssemblyCategoryGroup.D)]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithValidAssessmentSectionAssemblyCategory_ExpectedAssessmentSectionAssemblyCategoryResultType(
            AssessmentSectionCategoryGroup categoryGroup,
            AssessmentSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Setup
            var output = new CalculationOutput<AssessmentSectionCategory[]>(new[]
            {
                new AssessmentSectionCategory(categoryGroup, new Probability(0), new Probability(0))
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(output);

            // Assert
            Assert.AreEqual(1, result.Count());
            AssessmentSectionAssemblyCategory categoryResult = result.First();

            Assert.AreEqual(expectedCategoryGroup, categoryResult.Group);
        }
    }
}