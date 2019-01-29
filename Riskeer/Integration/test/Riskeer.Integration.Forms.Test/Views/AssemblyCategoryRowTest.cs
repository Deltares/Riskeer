// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyCategoryRowTest
    {
        [Test]
        public void Constructor_AssemblyCategoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => new AssemblyCategoryRow<TestAssemblyCategoryGroup>(null,
                                                                                         Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                                                                                         random.NextEnumValue<TestAssemblyCategoryGroup>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assemblyCategory", paramName);
        }

        [Test]
        public void Constructor_WithAssessmentSectionAssemblyCategory_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double lowerBoundary = random.NextDouble();
            double upperBoundary = random.NextDouble();
            var categoryGroup = random.NextEnumValue<TestAssemblyCategoryGroup>();
            Color categoryColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            var category = new TestAssemblyCategory(lowerBoundary, upperBoundary);

            // Call
            var categoryRow = new AssemblyCategoryRow<TestAssemblyCategoryGroup>(category, categoryColor, categoryGroup);

            // Assert
            Assert.AreEqual(categoryGroup, categoryRow.Group);
            Assert.AreEqual(categoryColor, categoryRow.Color);
            Assert.AreEqual(lowerBoundary, categoryRow.LowerBoundary);
            Assert.AreEqual(upperBoundary, categoryRow.UpperBoundary);

            TestHelper.AssertTypeConverter<AssemblyCategoryRow<TestAssemblyCategoryGroup>,
                EnumTypeConverter>(nameof(AssemblyCategoryRow<TestAssemblyCategoryGroup>.Group));
            TestHelper.AssertTypeConverter<AssemblyCategoryRow<TestAssemblyCategoryGroup>,
                NoProbabilityValueDoubleConverter>(
                nameof(AssemblyCategoryRow<TestAssemblyCategoryGroup>.LowerBoundary));
            TestHelper.AssertTypeConverter<AssemblyCategoryRow<TestAssemblyCategoryGroup>,
                NoProbabilityValueDoubleConverter>(
                nameof(AssemblyCategoryRow<TestAssemblyCategoryGroup>.UpperBoundary));
        }

        private class TestAssemblyCategory : AssemblyCategory
        {
            public TestAssemblyCategory(double lowerBoundary, double upperBoundary)
                : base(lowerBoundary, upperBoundary) {}
        }

        private enum TestAssemblyCategoryGroup
        {
            I = 1
        }
    }
}