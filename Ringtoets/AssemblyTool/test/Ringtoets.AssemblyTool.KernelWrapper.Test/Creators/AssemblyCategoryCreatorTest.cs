﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyCategoryCreatorTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoryLimitsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categoryLimits", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithAssessmentSectionCategoryLimits_ReturnAssessmentSectionAssemblyCategories()
        {
            // Setup
            var random = new Random(11);

            var categoryLimits = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.75, 1.00),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.5, 0.75),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.25, 0.5),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0, 0.25)
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(categoryLimits, result);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithInvalidAssessmentGrade_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var categoryLimits = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory((EAssessmentGrade) 99, 0, 0)
            });

            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelper), nameof(AssessmentGradeConversionTestHelper.AsssementGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithValidAssessmentGrade_ExpectedAssessmentSectionAssemblyCategoryResultType(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Setup
            var categoryLimits = new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(categoryGroup, 0, 1)
            });

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result.Single().Group);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategoryGroup_WithInvalidAssessmentGrade_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory((EAssessmentGrade) 99);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelper), nameof(AssessmentGradeConversionTestHelper.AsssementGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyCategory_WithValidAssessmentGrade_ExpectedAssessmentSectionAssemblyCategoryResultType(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Call
            AssessmentSectionAssemblyCategoryGroup categoryResult = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory(categoryGroup);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, categoryResult);
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_CategoryLimitsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateFailureMechanismAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categoryLimits", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_WithCategoryLimits_ReturnFailureMechanismAssemblyCategoryResult()
        {
            // Setup
            var random = new Random(11);

            var categoryLimits = new CategoriesList<FailureMechanismCategory>(new[]
            {
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.75, 1.00),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.5, 0.75),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.25, 0.5),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0, 0.25)
            });

            // Call
            IEnumerable<FailureMechanismAssemblyCategory> result = AssemblyCategoryCreator.CreateFailureMechanismAssemblyCategories(categoryLimits);

            // Assert
            AssemblyCategoryAssert.AssertFailureMechanismAssemblyCategories(categoryLimits, result);
        }

        [Test]
        public void CreateFailureMechanismAssemblyCategories_CategoryWithInvalidFailureMechanismCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var categoryLimit = new CategoriesList<FailureMechanismCategory>(new[]
            {
                new FailureMechanismCategory((EFailureMechanismCategory) 99, 0, 0)
            });

            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateFailureMechanismAssemblyCategories(categoryLimit);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EFailureMechanismCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCase(EFailureMechanismCategory.It, FailureMechanismAssemblyCategoryGroup.It)]
        [TestCase(EFailureMechanismCategory.IIt, FailureMechanismAssemblyCategoryGroup.IIt)]
        [TestCase(EFailureMechanismCategory.IIIt, FailureMechanismAssemblyCategoryGroup.IIIt)]
        [TestCase(EFailureMechanismCategory.IVt, FailureMechanismAssemblyCategoryGroup.IVt)]
        [TestCase(EFailureMechanismCategory.Vt, FailureMechanismAssemblyCategoryGroup.Vt)]
        [TestCase(EFailureMechanismCategory.VIt, FailureMechanismAssemblyCategoryGroup.VIt)]
        [TestCase(EFailureMechanismCategory.VIIt, FailureMechanismAssemblyCategoryGroup.VIIt)]
        [TestCase(EFailureMechanismCategory.Nvt, FailureMechanismAssemblyCategoryGroup.NotApplicable)]
        [TestCase(EFailureMechanismCategory.Gr, FailureMechanismAssemblyCategoryGroup.None)]
        public void CreateFailureMechanismAssemblyCategories_CategoryWithValidFailureMechanismCategory_ExpectedFailureMechanismAssemblyCategoryResultType(
            EFailureMechanismCategory categoryGroup,
            FailureMechanismAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Setup
            var categoryLimits = new CategoriesList<FailureMechanismCategory>(new[]
            {
                new FailureMechanismCategory(categoryGroup, 0, 0)
            });

            // Call
            IEnumerable<FailureMechanismAssemblyCategory> result = AssemblyCategoryCreator.CreateFailureMechanismAssemblyCategories(categoryLimits);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result.Single().Group);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_CategoryLimitsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categoryLimits", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_WithCategoryLimits_ReturnFailureMechanismSectionAssemblyCategoryResult()
        {
            // Setup
            var random = new Random(11);

            var categoryLimits = new[]
            {
                new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1))
            };

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(categoryLimits);

            // Assert
            AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(categoryLimits, result);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_CategoryWithInvalidFailureMechanismSectionCategory_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var categoryLimit = new[]
            {
                new FmSectionCategoryLimits((EFmSectionCategory) 99, 0, 0)
            };

            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(categoryLimit);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EFmSectionCategory'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCase(EFmSectionCategory.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(EFmSectionCategory.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(EFmSectionCategory.IIIv, FailureMechanismSectionAssemblyCategoryGroup.IIIv)]
        [TestCase(EFmSectionCategory.IVv, FailureMechanismSectionAssemblyCategoryGroup.IVv)]
        [TestCase(EFmSectionCategory.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(EFmSectionCategory.VIv, FailureMechanismSectionAssemblyCategoryGroup.VIv)]
        [TestCase(EFmSectionCategory.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        [TestCase(EFmSectionCategory.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(EFmSectionCategory.Gr, FailureMechanismSectionAssemblyCategoryGroup.None)]
        public void CreateFailureMechanismSectionAssemblyCategories_CategoryWithValidFailureMechanismSectionCategory_ExpectedFailureMechanismSectionAssemblyCategoryResultType(
            EFmSectionCategory categoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Setup
            var categoryLimits = new[]
            {
                new FmSectionCategoryLimits(categoryGroup, 0, 0)
            };

            // Call
            IEnumerable<FailureMechanismSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(categoryLimits);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result.Single().Group);
        }
    }
}