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
using System.Linq;
using Assembly.Kernel.Old.Model;
using Assembly.Kernel.Old.Model.CategoryLimits;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyCategoryCreatorOldTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithAssessmentSectionCategories_ReturnAssessmentSectionAssemblyCategories()
        {
            // Setup
            var random = new Random(11);

            var categories = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0, 0.25),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.25, 0.5),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.5, 0.75),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.75, 1)
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyGroupBoundaries> result = AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategories(categories);

            // Assert
            AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(categories, result);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithInvalidAssessmentGrade_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var categories = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory((EAssessmentGrade) 99, 0, 1)
            });

            // Call
            TestDelegate test = () => AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategories(categories);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelperOld), nameof(AssessmentGradeConversionTestHelperOld.AssessmentGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithValidAssessmentGrade_ExpectedAssessmentSectionAssemblyCategoryResultType(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyGroup expectedCategoryGroup)
        {
            // Setup
            var categories = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(categoryGroup, 0, 1)
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyGroupBoundaries> result = AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategories(categories);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result.Single().Group);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategoryGroup_WithInvalidAssessmentGrade_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategory((EAssessmentGrade) 99);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelperOld), nameof(AssessmentGradeConversionTestHelperOld.AssessmentGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyCategory_WithValidAssessmentGrade_ExpectedAssessmentSectionAssemblyCategoryResultType(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyGroup expectedCategoryGroup)
        {
            // Call
            AssessmentSectionAssemblyGroup categoryResult = AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategory(categoryGroup);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, categoryResult);
        }
    }
}