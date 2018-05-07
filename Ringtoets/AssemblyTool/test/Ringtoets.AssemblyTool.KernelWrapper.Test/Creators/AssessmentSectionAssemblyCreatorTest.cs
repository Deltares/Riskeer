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
using Assembly.Kernel.Model;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssessmentSectionAssemblyCreatorTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyWithProbability_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionAssemblyCreator.CreateAssessmentSectionAssembly(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void CreateAssessmentSectionAssemblyWithProbability_WithInvalidEnum_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            var assemblyResult = new AssessmentSectionAssemblyResult((EAssessmentGrade) 99, random.NextDouble());

            // Call
            TestDelegate test = () => AssessmentSectionAssemblyCreator.CreateAssessmentSectionAssembly(assemblyResult);

            // Assert
            const string exceptionMessage = "The value of argument 'category' (99) is invalid for Enum type 'EAssessmentGrade'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, exceptionMessage);
        }

        [Test]
        [TestCaseSource(typeof(AssessmentGradeConversionTestHelper), nameof(AssessmentGradeConversionTestHelper.AsssementGradeConversionCases))]
        public void CreateAssessmentSectionAssemblyWithProbability_WithValidEnum_ReturnsExpectedValues(EAssessmentGrade originalGroup,
                                                                                                       AssessmentSectionAssemblyCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var assemblyResult = new AssessmentSectionAssemblyResult(originalGroup, probability);

            // Call
            AssessmentSectionAssembly result = AssessmentSectionAssemblyCreator.CreateAssessmentSectionAssembly(assemblyResult);

            // Assert
            Assert.AreEqual(probability, result.Probability);
            Assert.AreEqual(expectedGroup, result.Group);
        }
    }
}