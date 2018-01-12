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
using System.ComponentModel;
using System.Linq;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.CategoriesOutput;
using AssemblyTool.Kernel.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyCategoryBoundariesResultCreatorTest
    {
        [Test]
        public void CreateAssessmentSectionResult_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryBoundariesResultCreator.CreateAssessmentSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionResult_WithOutput_ReturnAssessmentSectionAssemblyCategoryResult()
        {
            // Setup
            var random = new Random(11);

            var output = new CalculationOutput<AssessmentSectionCategoriesOutput[]>(new[]
            {
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2))
            });

            // Call
            AssemblyCategoryBoundariesResult<AssessmentSectionAssemblyCategoryResult> result = AssemblyCategoryBoundariesResultCreator.CreateAssessmentSectionResult(output);

            // Assert
            AssemblyCategoryBoundariesResultAssert.AssertAssessmentSectionAssemblyCategoryBoundariesResult(output, result);
        }

        [Test]
        public void CreateAssessmentSectionResult_CategoryWithInvalidAssessmentSectionAssemblyCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var output = new CalculationOutput<AssessmentSectionCategoriesOutput[]>(new[]
            {
                new AssessmentSectionCategoriesOutput((AssessmentSectionAssemblyCategory) 99, 0, 0)
            });

            // Call
            TestDelegate test = () => AssemblyCategoryBoundariesResultCreator.CreateAssessmentSectionResult(output);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'AssessmentSectionAssemblyCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategory.APlus, AssessmentSectionAssemblyCategoryResultType.APlus)]
        [TestCase(AssessmentSectionAssemblyCategory.A, AssessmentSectionAssemblyCategoryResultType.A)]
        [TestCase(AssessmentSectionAssemblyCategory.B, AssessmentSectionAssemblyCategoryResultType.B)]
        [TestCase(AssessmentSectionAssemblyCategory.C, AssessmentSectionAssemblyCategoryResultType.C)]
        [TestCase(AssessmentSectionAssemblyCategory.D, AssessmentSectionAssemblyCategoryResultType.D)]
        public void CreateAssessmentSectionResult_CategoryWithValidAssessmentSectionAssemblyCategory_ExpectedAssessmentSectionAssemblyCategoryResultType(
            AssessmentSectionAssemblyCategory category,
            AssessmentSectionAssemblyCategoryResultType expectedCategoryResultType)
        {
            // Setup
            var output = new CalculationOutput<AssessmentSectionCategoriesOutput[]>(new[]
            {
                new AssessmentSectionCategoriesOutput(category, 0, 0)
            });

            // Call
            AssemblyCategoryBoundariesResult<AssessmentSectionAssemblyCategoryResult> result = AssemblyCategoryBoundariesResultCreator.CreateAssessmentSectionResult(output);

            // Assert
            Assert.AreEqual(1, result.Categories.Count());
            AssessmentSectionAssemblyCategoryResult categoryResult = result.Categories.First();

            Assert.AreEqual(expectedCategoryResultType, categoryResult.Category);
        }
    }
}