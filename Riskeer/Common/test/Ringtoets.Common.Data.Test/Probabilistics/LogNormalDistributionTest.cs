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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class LogNormalDistributionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new LogNormalDistribution();

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);

            const int numberOfDecimalPlaces = RoundedDouble.MaximumNumberOfDecimalPlaces;
            double expectedAccuracy = Math.Pow(10.0, -numberOfDecimalPlaces);

            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)), distribution.StandardDeviation, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void Constructor_WithParameter_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);

            double expectedAccuracy = Math.Pow(10.0, -numberOfDecimalPlaces);

            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)), distribution.StandardDeviation, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowArgumentOutOfRangeException()
        {
            // Call
            TestDelegate call = () => new LogNormalDistribution(0);

            // Assert
            const string expectedMessage = "Value must be in range [1, 15].";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            // Setup
            const double value = 1.23456789;
            const int numberOfDecimalPlaces = 2;
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(value, distribution.Mean, distribution.Mean.GetAccuracy());
        }

        [Test]
        [TestCase(0)]
        [TestCase(0 + 1e-3, Description = "Invalid mean due to rounding to 0.0")]
        [TestCase(-123.45)]
        public void Mean_SettingToLessThanOrEqualTo0_ThrowArgumentOutOfRangeException(double mean)
        {
            // Setup
            var distribution = new LogNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.Mean = (RoundedDouble) mean;

            // Assert
            const string expectedMessage = "Gemiddelde moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Mean_SettingToLessThanShift_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var distribution = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 20),
                Shift = new RoundedDouble(2, 10)
            };

            // Call
            TestDelegate test = () => distribution.Mean = (RoundedDouble) 5;

            // Assert
            const string expectedMessage = "De verschuiving mag niet groter zijn dan de verwachtingswaarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(1.23456789)]
        [TestCase(0 - 1e-3, Description = "Valid standard deviation due to rounding to 0.0")]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(double standardDeviation)
        {
            // Setup
            const int numberOfDecimalPlaces = 2;
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = (RoundedDouble) standardDeviation;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(standardDeviation, distribution.StandardDeviation, distribution.StandardDeviation.GetAccuracy());
        }

        [Test]
        [TestCase(-4)]
        [TestCase(0 - 1e-2)]
        public void StandardDeviation_SettingToLessThan0_ThrowArgumentOutOfRangeException(double standardDeviation)
        {
            // Setup
            var distribution = new LogNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble) standardDeviation;

            // Assert
            const string expectedMessage = "Standaardafwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(1, 5.6)]
        [TestCase(3, 5.647)]
        [TestCase(4, 5.6473)]
        [TestCase(15, 5.647300000000000)]
        public void Shift_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedShift)
        {
            // Setup
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = new RoundedDouble(2, 10.0)
            };

            // Call
            distribution.Shift = new RoundedDouble(4, 5.6473);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedShift, distribution.Shift.Value);
        }

        [Test]
        public void Shift_SetIllegalValue_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var distribution = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 10.0),
                StandardDeviation = new RoundedDouble(2, 1.0)
            };

            // Call
            TestDelegate call = () => distribution.Shift = new RoundedDouble(2, 100.0);

            // Assert
            const string expectedMessage = "De verschuiving mag niet groter zijn dan de verwachtingswaarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new LogNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, DistributionAssert.AreEqual);
        }

        [TestFixture]
        private class LogNormalDistributionEqualsTest : EqualsTestFixture<LogNormalDistribution, DerivedLogNormalDistribution>
        {
            protected override LogNormalDistribution CreateObject()
            {
                return CreateFullyDefinedDistribution();
            }

            protected override DerivedLogNormalDistribution CreateDerivedObject()
            {
                LogNormalDistribution baseDistribution = CreateFullyDefinedDistribution();
                return new DerivedLogNormalDistribution(baseDistribution);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                LogNormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(otherMean)
                    .SetName("Mean");

                LogNormalDistribution otherStandardDeviation = CreateFullyDefinedDistribution();
                otherStandardDeviation.StandardDeviation = (RoundedDouble) 0.987;
                yield return new TestCaseData(otherStandardDeviation)
                    .SetName("StandardDeviation");

                LogNormalDistribution otherShift = CreateFullyDefinedDistribution();
                otherShift.Shift = (RoundedDouble) 0.987;
                yield return new TestCaseData(otherShift)
                    .SetName("Shift");
            }

            private static LogNormalDistribution CreateFullyDefinedDistribution()
            {
                return new LogNormalDistribution(5)
                {
                    Mean = (RoundedDouble) 1,
                    StandardDeviation = (RoundedDouble) 0.1,
                    Shift = (RoundedDouble) 0.2
                };
            }
        }

        private class DerivedLogNormalDistribution : LogNormalDistribution
        {
            public DerivedLogNormalDistribution(LogNormalDistribution distribution)
            {
                Mean = distribution.Mean;
                StandardDeviation = distribution.StandardDeviation;
                Shift = distribution.Shift;
            }
        }
    }
}