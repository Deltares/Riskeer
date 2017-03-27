// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class IDistributionExtensionsTest
    {
        [Test]
        public void TrySetMean_DistributionNull_ThrownArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            const double mean = 1.1;

            // Call
            TestDelegate call = () => distribution.TrySetMean(mean, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        public void TrySetMean_MeanNull_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            // Call
            bool result = distribution.TrySetMean(null, "A", "B");

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetMean_MeanValid_SetMeanAndReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            const double mean = 1.1;

            // Call
            bool result = distribution.TrySetMean(mean, "A", "B");

            // Assert
            Assert.AreEqual(mean, distribution.Mean);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetMean_SettingMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            distribution.Expect(d => d.Mean)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            mocks.ReplayAll();

            const int mean = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetMean(mean, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
                                               exceptionMessage +
                                               $" Berekening '{calculationName}' is overgeslagen.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetStandardDeviation_DistributionNull_ThrownArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            const double mean = 1.1;

            // Call
            TestDelegate call = () => distribution.TrySetStandardDeviation(mean, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        public void TrySetStandardDeviation_StandardDeviationNull_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            // Call
            bool result = distribution.TrySetStandardDeviation(null, "A", "B");

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetStandardDeviation_StandardDeviationValid_SetStandardDeviationAndReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            const double standardDeviation = 1.1;

            // Call
            bool result = distribution.TrySetStandardDeviation(standardDeviation, "A", "B");

            // Assert
            Assert.AreEqual(standardDeviation, distribution.StandardDeviation);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetStandardDeviation_SettingStandardDeviationThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            distribution.Expect(d => d.StandardDeviation)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            mocks.ReplayAll();

            const int standardDeviation = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetStandardDeviation(standardDeviation, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een standaardafwijking van '{standardDeviation}' is ongeldig voor stochast '{stochastName}'. " +
                                               exceptionMessage +
                                               $" Berekening '{calculationName}' is overgeslagen.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_DistributionNull_ThrownArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            const double mean = 1.1;
            const double standardDeviation = 2.2;

            // Call
            TestDelegate call = () => distribution.TrySetDistributionProperties(mean, standardDeviation, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(1.0, null)]
        [TestCase(null, 2.0)]
        [TestCase(3.0, 4.0)]
        public void TrySetMean_ValidValue_ReturnTrue(double? mean, double? standardDeviation)
        {
            // Setup
            var defaultMean = new RoundedDouble(2, -1.0);
            var defaultStandardDeviation = new RoundedDouble(2, -2.0);

            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            distribution.Mean = defaultMean;
            distribution.StandardDeviation = defaultStandardDeviation;

            // Call
            bool result = distribution.TrySetDistributionProperties(mean, standardDeviation, "A", "B");

            // Assert
            Assert.IsTrue(result);

            Assert.AreEqual(mean ?? defaultMean.Value, distribution.Mean.Value);
            Assert.AreEqual(standardDeviation ?? defaultStandardDeviation.Value, distribution.StandardDeviation.Value);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_SettingMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            distribution.Expect(d => d.Mean)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            distribution.Stub(d => d.StandardDeviation)
                        .SetPropertyAndIgnoreArgument()
                        .Repeat.Any();
            mocks.ReplayAll();

            const int mean = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(mean, 2.2, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
                                               exceptionMessage +
                                               $" Berekening '{calculationName}' is overgeslagen.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_SettingStandardDeviationThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            distribution.Expect(d => d.StandardDeviation)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            distribution.Stub(d => d.Mean)
                        .SetPropertyAndIgnoreArgument()
                        .Repeat.Any();
            mocks.ReplayAll();

            const int standardDeviation = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(1.1, standardDeviation, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een standaardafwijking van '{standardDeviation}' is ongeldig voor stochast '{stochastName}'. " +
                                               exceptionMessage +
                                               $" Berekening '{calculationName}' is overgeslagen.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }
    }
}