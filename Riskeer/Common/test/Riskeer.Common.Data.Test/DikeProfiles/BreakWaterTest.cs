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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class BreakWaterTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;

            // Call
            var breakWater = new BreakWater(type, height);

            // Assert
            Assert.IsInstanceOf<ICloneable>(breakWater);
            Assert.AreEqual(type, breakWater.Type);
            Assert.AreEqual(height, breakWater.Height, 1e-6);
            Assert.AreEqual(2, breakWater.Height.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall)]
        public void Properties_Type_ReturnsExpectedValue(BreakWaterType newType)
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;
            var breakWater = new BreakWater(type, height);

            // Call
            breakWater.Type = newType;

            // Assert
            Assert.AreEqual(newType, breakWater.Type);
        }

        [Test]
        public void Properties_Height_ReturnsExpectedValue()
        {
            // Setup
            const BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;
            var breakWater = new BreakWater(type, height);

            // Call
            breakWater.Height = (RoundedDouble) 10.00;

            // Assert
            Assert.AreEqual(10.0, breakWater.Height.Value);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new BreakWater(random.NextEnumValue<BreakWaterType>(), random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }

        [TestFixture]
        private class BreakWaterEqualsTest : EqualsTestFixture<BreakWater, DerivedBreakWater>
        {
            protected override BreakWater CreateObject()
            {
                return CreateBreakWater();
            }

            protected override DerivedBreakWater CreateDerivedObject()
            {
                return new DerivedBreakWater(CreateBreakWater());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                BreakWater baseBreakWater = CreateBreakWater();

                var random = new Random(21);
                double offset = random.NextDouble();

                yield return new TestCaseData(new BreakWater(BreakWaterType.Dam, baseBreakWater.Height))
                    .SetName("Type");
                yield return new TestCaseData(new BreakWater(baseBreakWater.Type, baseBreakWater.Height + offset))
                    .SetName("Height");
            }

            private static BreakWater CreateBreakWater()
            {
                return new BreakWater(BreakWaterType.Caisson, 3.14);
            }
        }

        private class DerivedBreakWater : BreakWater
        {
            public DerivedBreakWater(BreakWater breakWater)
                : base(breakWater.Type, breakWater.Height) {}
        }
    }
}