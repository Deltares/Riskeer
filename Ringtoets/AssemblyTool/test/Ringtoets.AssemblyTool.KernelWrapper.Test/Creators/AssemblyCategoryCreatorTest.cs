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
        public void CreateAssessmentSectionAssemblyCategories_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithAssessmentSectionCategoryLimits_ReturnAssessmentSectionAssemblyCategories()
        {
            // Setup
            var random = new Random(11);

            var categoryLimits = new[]
            {
                new AssessmentSectionCategoryLimits(EAssessmentGrade.APlus, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.A, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.B, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.C, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.D, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.Gr, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.Ngo, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new AssessmentSectionCategoryLimits(EAssessmentGrade.Nvt, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1))
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(categoryLimits, result);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithInvalidAssessmentGrade_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var categoryLimits = new[]
            {
                new AssessmentSectionCategoryLimits((EAssessmentGrade) 99, 0, 0)
            };

            // Call
            TestDelegate test = () => AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCase(EAssessmentGrade.APlus, AssessmentSectionAssemblyCategoryGroup.APlus)]
        [TestCase(EAssessmentGrade.A, AssessmentSectionAssemblyCategoryGroup.A)]
        [TestCase(EAssessmentGrade.B, AssessmentSectionAssemblyCategoryGroup.B)]
        [TestCase(EAssessmentGrade.C, AssessmentSectionAssemblyCategoryGroup.C)]
        [TestCase(EAssessmentGrade.D, AssessmentSectionAssemblyCategoryGroup.D)]
        public void CreateAssessmentSectionAssemblyCategories_CategoryWithValidAssessmentGrade_ExpectedAssessmentSectionAssemblyCategoryResultType(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Setup
            var categoryLimits = new[]
            {
                new AssessmentSectionCategoryLimits(categoryGroup, 0, 0)
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> result = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(categoryLimits);

            // Assert
            AssessmentSectionAssemblyCategory categoryResult = result.Single();

            Assert.AreEqual(expectedCategoryGroup, categoryResult.Group);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategories_WithOutput_ReturnFailureMechanismSectionAssemblyCategoryResult()
        {
            // Setup
            var random = new Random(11);

            var categoryLimits = new[]
            {
                new FmSectionCategoryLimits(EFmSectionCategory.Gr, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.Iv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.IIv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.IIIv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.IVv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.Vv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.VIv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.VIIv, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1)),
                new FmSectionCategoryLimits(EFmSectionCategory.NotApplicable, random.NextDouble(0, 0.5), random.NextDouble(0.5, 1))
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
            FailureMechanismSectionAssemblyCategory categoryResult = result.Single();

            Assert.AreEqual(expectedCategoryGroup, categoryResult.Group);
        }
    }
}