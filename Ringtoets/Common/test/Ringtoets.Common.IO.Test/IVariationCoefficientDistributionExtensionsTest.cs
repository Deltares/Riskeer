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
    public class IVariationCoefficientDistributionExtensionsTest
    {
        [Test]
        public void TrySetMean_DistributionNull_ThrownArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;

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
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
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
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
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
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
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
        public void TrySetVariationCoefficient_DistributionNull_ThrownArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;

            const double mean = 1.1;

            // Call
            TestDelegate call = () => distribution.TrySetVariationCoefficient(mean, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        public void TrySetVariationCoefficient_VariationCoefficientNull_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            bool result = distribution.TrySetVariationCoefficient(null, "A", "B");

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetVariationCoefficient_VariationCoefficientValid_SetVariationCoefficientAndReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            const double variationCoefficient = 1.1;

            // Call
            bool result = distribution.TrySetVariationCoefficient(variationCoefficient, "A", "B");

            // Assert
            Assert.AreEqual(variationCoefficient, distribution.CoefficientOfVariation);
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetVariationCoefficient_SettingVariationCoefficientThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            distribution.Expect(d => d.CoefficientOfVariation)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            mocks.ReplayAll();

            const int variationCoefficient = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetVariationCoefficient(variationCoefficient, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een variatiecoëfficiënt van '{variationCoefficient}' is ongeldig voor stochast '{stochastName}'. " +
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
            IVariationCoefficientDistribution distribution = null;

            const double mean = 1.1;
            const double variationCoefficient = 2.2;

            // Call
            TestDelegate call = () => distribution.TrySetDistributionProperties(mean, variationCoefficient, "A", "B");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("distribution", paramName);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(1.0, null)]
        [TestCase(null, 2.0)]
        [TestCase(3.0, 4.0)]
        public void TrySetMean_ValidValue_ReturnTrue(double? mean, double? variationCoefficient)
        {
            // Setup
            var defaultMean = new RoundedDouble(2, -1.0);
            var defaultVariationCoefficient = new RoundedDouble(2, -2.0);

            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            distribution.Mean = defaultMean;
            distribution.CoefficientOfVariation = defaultVariationCoefficient;

            // Call
            bool result = distribution.TrySetDistributionProperties(mean, variationCoefficient, "A", "B");

            // Assert
            Assert.IsTrue(result);

            Assert.AreEqual(mean ?? defaultMean.Value, distribution.Mean.Value);
            Assert.AreEqual(variationCoefficient ?? defaultVariationCoefficient.Value, distribution.CoefficientOfVariation.Value);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_SettingMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            distribution.Expect(d => d.Mean)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            distribution.Stub(d => d.CoefficientOfVariation)
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
        public void TrySetDistributionProperties_SettingVariationCoefficientThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
        {
            // Setup
            const string exceptionMessage = "A";
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            distribution.Expect(d => d.CoefficientOfVariation)
                        .SetPropertyAndIgnoreArgument()
                        .Throw(new ArgumentOutOfRangeException(null, exceptionMessage));
            distribution.Stub(d => d.Mean)
                        .SetPropertyAndIgnoreArgument()
                        .Repeat.Any();
            mocks.ReplayAll();

            const int variationCoefficient = 5;
            const string stochastName = "B";
            const string calculationName = "C";

            // Call
            bool result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(1.1, variationCoefficient, "B", "C");

            // Assert
            var expectedMessage = Tuple.Create($"Een variatiecoëfficiënt van '{variationCoefficient}' is ongeldig voor stochast '{stochastName}'. " +
                                               exceptionMessage +
                                               $" Berekening '{calculationName}' is overgeslagen.",
                                               LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }
    }
}