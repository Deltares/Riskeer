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
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionAssemblyCategoryRowTest
    {
        [Test]
        public void Constructor_AssessmentSectionAssemblyCategoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionAssemblyCategoryRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSectionAssemblyCategory", paramName);
        }

        [Test]
        public void Constructor_WithAssessmentSectionAssemblyCategory_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double lowerBoundary = random.NextDouble();
            double upperBoundary = random.NextDouble();
            var categoryGroup = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();
            var category = new AssessmentSectionAssemblyCategory(lowerBoundary, upperBoundary, categoryGroup);

            // Call
            var categoryRow = new AssessmentSectionAssemblyCategoryRow(category);

            // Assert
            Assert.AreEqual(categoryGroup, categoryRow.Group);
            Assert.AreEqual(AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(categoryGroup), categoryRow.Color);
            Assert.AreEqual(lowerBoundary, categoryRow.LowerBoundary);
            Assert.AreEqual(upperBoundary, categoryRow.UpperBoundary);

            TestHelper.AssertTypeConverter<AssessmentSectionAssemblyCategoryRow,
                EnumTypeConverter>(nameof(AssessmentSectionAssemblyCategoryRow.Group));
            TestHelper.AssertTypeConverter<AssessmentSectionAssemblyCategoryRow,
                NoProbabilityValueDoubleConverter>(
                nameof(AssessmentSectionAssemblyCategoryRow.LowerBoundary));
            TestHelper.AssertTypeConverter<AssessmentSectionAssemblyCategoryRow,
                NoProbabilityValueDoubleConverter>(
                nameof(AssessmentSectionAssemblyCategoryRow.UpperBoundary));
        }
    }
}