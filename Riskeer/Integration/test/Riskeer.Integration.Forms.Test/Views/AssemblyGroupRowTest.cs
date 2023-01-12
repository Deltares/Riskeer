// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Common.Util.Enums;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyGroupRowTest
    {
        [Test]
        public void Constructor_AssemblyGroupBoundariesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            void Call() => new AssemblyGroupRow<TestEnum>(null,
                                                          Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                                                          random.NextEnumValue<TestEnum>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assemblyGroupBoundaries", paramName);
        }

        [Test]
        public void Constructor_WithAssemblyGroup_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            double lowerBoundary = random.NextDouble();
            double upperBoundary = random.NextDouble();
            var group = random.NextEnumValue<TestEnum>();
            var color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            var assemblyGroupBoundaries = new TestAssemblyGroupBoundaries(lowerBoundary, upperBoundary);

            // Call
            var groupRow = new AssemblyGroupRow<TestEnum>(assemblyGroupBoundaries, color, group);

            // Assert
            Assert.AreEqual(group, groupRow.Group);
            Assert.AreEqual(color, groupRow.Color);
            Assert.AreEqual(lowerBoundary, groupRow.LowerBoundary);
            Assert.AreEqual(upperBoundary, groupRow.UpperBoundary);

            TestHelper.AssertTypeConverter<AssemblyGroupRow<TestEnum>, EnumTypeConverter>(nameof(AssemblyGroupRow<TestEnum>.Group));
            TestHelper.AssertTypeConverter<AssemblyGroupRow<TestEnum>, NoProbabilityValueDoubleConverter>(nameof(AssemblyGroupRow<TestEnum>.LowerBoundary));
            TestHelper.AssertTypeConverter<AssemblyGroupRow<TestEnum>, NoProbabilityValueDoubleConverter>(nameof(AssemblyGroupRow<TestEnum>.UpperBoundary));
        }

        private class TestAssemblyGroupBoundaries : AssemblyGroupBoundaries
        {
            public TestAssemblyGroupBoundaries(double lowerBoundary, double upperBoundary)
                : base(lowerBoundary, upperBoundary) {}
        }
    }
}