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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Output;

namespace Ringtoets.AssemblyTool.Data.Test.Output
{
    [TestFixture]
    public class AssessmentSectionAssemblyCategoryResultTest
    {
        [Test]
        public void Constructor_InvalidAssessmentSectionAssemblyCategoryResultType_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionAssemblyCategoryResult((AssessmentSectionAssemblyCategoryResultType) 99, 0, 0);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'AssessmentSectionAssemblyCategoryResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundary = random.Next();
            double upperBoundary = random.Next();
            var category = random.NextEnumValue<AssessmentSectionAssemblyCategoryResultType>();

            // Call
            var categoryResult = new AssessmentSectionAssemblyCategoryResult(category, lowerBoundary, upperBoundary);

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoryResult>(categoryResult);
            Assert.AreEqual(category, categoryResult.Category);
            Assert.AreEqual(lowerBoundary, categoryResult.LowerBoundary);
            Assert.AreEqual(upperBoundary, categoryResult.UpperBoundary);
        }
    }
}