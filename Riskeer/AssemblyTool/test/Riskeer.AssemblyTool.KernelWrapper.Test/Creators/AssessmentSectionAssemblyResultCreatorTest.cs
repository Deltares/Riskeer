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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssessmentSectionAssemblyResultCreatorTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyResult_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyResultCreator.CreateAssessmentSectionAssemblyResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelper), nameof(AssessmentGradeConversionTestHelper.AssessmentGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyResult_WithResult_ReturnsExpectedResult(
            EAssessmentGrade categoryGroup,
            AssessmentSectionAssemblyGroup expectedCategoryGroup)
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var result = new AssessmentSectionResult(new Probability(probability), categoryGroup);

            // Call
            AssessmentSectionAssemblyResult createdResult =
                AssessmentSectionAssemblyResultCreator.CreateAssessmentSectionAssemblyResult(result);

            // Assert
            Assert.AreEqual(probability, createdResult.Probability);
            Assert.AreEqual(expectedCategoryGroup, createdResult.AssemblyCategoryGroup);
        }
    }
}