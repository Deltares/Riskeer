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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Service.AssemblyTool;

namespace Ringtoets.Common.Service.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyCategoryConverterTest
    {
        [Test]
        public void ConvertAssessmentSectionAssemblyCategories_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryConverter.ConvertAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void ConvertAssessmentSectionAssemblyCategories_WithResult_ReturnAssessmentSectionAssemblyCategories()
        {
            // Setup
            var random = new Random(11);
            var result = new[]
            {
                new AssessmentSectionAssemblyCategoryResult(random.NextDouble(), random.NextDouble(), random.NextEnumValue<AssessmentSectionAssemblyCategoryResultType>()),
                new AssessmentSectionAssemblyCategoryResult(random.NextDouble(), random.NextDouble(), random.NextEnumValue<AssessmentSectionAssemblyCategoryResultType>()),
                new AssessmentSectionAssemblyCategoryResult(random.NextDouble(), random.NextDouble(), random.NextEnumValue<AssessmentSectionAssemblyCategoryResultType>())
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> categories = AssemblyCategoryConverter.ConvertAssessmentSectionAssemblyCategories(result);

            // Assert
            Assert.AreEqual(result.Length, categories.Count());
            CollectionAssert.AreEqual(result.Select(r => r.LowerBoundary), categories.Select(c => c.LowerBoundary));
            CollectionAssert.AreEqual(result.Select(r => r.UpperBoundary), categories.Select(c => c.UpperBoundary));
        }

        [Test]
        public void ConvertAssessmentSectionAssemblyCategories_CategoryWithInvalidType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var result = new[]
            {
                new AssessmentSectionAssemblyCategoryResult(0, 0, (AssessmentSectionAssemblyCategoryResultType) 99)
            };

            // Call
            TestDelegate test = () => AssemblyCategoryConverter.ConvertAssessmentSectionAssemblyCategories(result);

            // Assert
            const string expectedMessage = "The value of argument 'categoryType' (99) is invalid for Enum type 'AssessmentSectionAssemblyCategoryResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryResultType.APlus, AssessmentSectionAssemblyCategoryType.APlus)]
        [TestCase(AssessmentSectionAssemblyCategoryResultType.A, AssessmentSectionAssemblyCategoryType.A)]
        [TestCase(AssessmentSectionAssemblyCategoryResultType.B, AssessmentSectionAssemblyCategoryType.B)]
        [TestCase(AssessmentSectionAssemblyCategoryResultType.C, AssessmentSectionAssemblyCategoryType.C)]
        [TestCase(AssessmentSectionAssemblyCategoryResultType.D, AssessmentSectionAssemblyCategoryType.D)]
        public void ConvertAssessmentSectionAssemblyCategories_CategoryWithValidType_ExpectedAssessmentSectionAssemblyCategoryType(
            AssessmentSectionAssemblyCategoryResultType resultType,
            AssessmentSectionAssemblyCategoryType expectedType)
        {
            // Setup
            var result = new[]
            {
                new AssessmentSectionAssemblyCategoryResult(0, 0, resultType)
            };

            // Call
            IEnumerable<AssessmentSectionAssemblyCategory> categories = AssemblyCategoryConverter.ConvertAssessmentSectionAssemblyCategories(result);

            // Assert
            Assert.AreEqual(1, categories.Count());
            AssessmentSectionAssemblyCategory category = categories.First();

            Assert.AreEqual(expectedType, category.Type);
        }
    }
}