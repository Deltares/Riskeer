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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionClosingSituationTest
    {
        [Test]
        public void Constructor_WithoutWindDirection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WindDirectionClosingSituation(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutClosingSituation_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WindDirectionClosingSituation(new TestWindDirection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ReturnNewInstance()
        {
            // Setup
            string closingSituation = string.Empty;
            var windDirection = new TestWindDirection();

            // Call
            var instance = new WindDirectionClosingSituation(windDirection, closingSituation);

            // Assert
            Assert.NotNull(instance);
            Assert.AreEqual(closingSituation, instance.ClosingSituation);
            Assert.AreSame(windDirection, instance.WindDirection);
        }

        [TestFixture]
        private class WindDirectionClosingSituationEqualsTest : EqualsTestFixture<WindDirectionClosingSituation, DerivedWindDirectionClosingSituation>
        {
            protected override WindDirectionClosingSituation CreateObject()
            {
                return CreateWindDirectionClosingSituation();
            }

            protected override DerivedWindDirectionClosingSituation CreateDerivedObject()
            {
                return new DerivedWindDirectionClosingSituation(CreateWindDirectionClosingSituation());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                WindDirectionClosingSituation baseCombination = CreateWindDirectionClosingSituation();

                yield return new TestCaseData(new WindDirectionClosingSituation(CreateWindDirection(30), baseCombination.ClosingSituation))
                    .SetName("WindDirection");
                yield return new TestCaseData(new WindDirectionClosingSituation(baseCombination.WindDirection, "Different closing situation"))
                    .SetName("ClosingSituation");
            }

            private static WindDirectionClosingSituation CreateWindDirectionClosingSituation()
            {
                return new WindDirectionClosingSituation(CreateWindDirection(21), "WDC");
            }

            private static WindDirection CreateWindDirection(int seed)
            {
                var random = new Random(seed);
                return new WindDirection("Name", random.NextDouble());
            }
        }

        private class DerivedWindDirectionClosingSituation : WindDirectionClosingSituation
        {
            public DerivedWindDirectionClosingSituation(WindDirectionClosingSituation wind)
                : base(wind.WindDirection, wind.ClosingSituation) {}
        }
    }
}