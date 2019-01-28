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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class DistributionConversionExtensionsTest
    {
        [Test]
        public void ToStochastConfiguration_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfiguration_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();
            RoundedDouble standardDeviation = random.NextRoundedDouble();

            distribution.Mean = mean;
            distribution.StandardDeviation = standardDeviation;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfiguration();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.AreEqual(standardDeviation, configuration.StandardDeviation);
        }

        [Test]
        public void ToStochastConfigurationWithMean_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithMean();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithMean_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();

            distribution.Mean = mean;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfigurationWithMean();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.IsNull(configuration.StandardDeviation);
        }

        [Test]
        public void ToStochastConfigurationWithStandardDeviation_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithStandardDeviation();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithStandardDeviation_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble standardDeviation = random.NextRoundedDouble();

            distribution.StandardDeviation = standardDeviation;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfigurationWithStandardDeviation();

            // Assert
            Assert.IsNull(configuration.Mean);
            Assert.AreEqual(standardDeviation, configuration.StandardDeviation);
        }

        [Test]
        public void ToStochastConfiguration_VariationCoefficientDistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfiguration_WithVariationCoefficientDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();
            RoundedDouble variationCoefficient = random.NextRoundedDouble();

            distribution.Mean = mean;
            distribution.CoefficientOfVariation = variationCoefficient;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfiguration();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.AreEqual(variationCoefficient, configuration.VariationCoefficient);
        }

        [Test]
        public void ToStochastConfigurationWithMean_VariationCoefficientDistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithMean();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithMean_WithVariationCoefficientDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble mean = random.NextRoundedDouble();

            distribution.Mean = mean;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfigurationWithMean();

            // Assert
            Assert.AreEqual(mean, configuration.Mean);
            Assert.IsNull(configuration.VariationCoefficient);
        }

        [Test]
        public void ToStochastConfigurationWithVariationCoefficient_DistributionNull_ThrowsArgumentNullException()
        {
            // Setup
            IVariationCoefficientDistribution distribution = null;

            // Call
            TestDelegate test = () => distribution.ToStochastConfigurationWithVariationCoefficient();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void ToStochastConfigurationWithVariationCoefficient_WithDistribution_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble variationCoefficient = random.NextRoundedDouble();

            distribution.CoefficientOfVariation = variationCoefficient;

            // Call
            StochastConfiguration configuration = distribution.ToStochastConfigurationWithVariationCoefficient();

            // Assert
            Assert.IsNull(configuration.Mean);
            Assert.AreEqual(variationCoefficient, configuration.VariationCoefficient);
        }

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
        public void TrySetMean_DistributionMeanNull_ReturnTrue()
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
        public void TrySetMean_DistributionMeanValid_SetMeanAndReturnTrue()
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
        public void TrySetMean_SettingDistributionMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetMean(mean, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
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
        public void TrySetStandardDeviation_DistributionStandardDeviationNull_ReturnTrue()
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
        public void TrySetStandardDeviation_DistributionStandardDeviationValid_SetStandardDeviationAndReturnTrue()
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
        public void TrySetStandardDeviation_SettingDistributionStandardDeviationThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetStandardDeviation(standardDeviation, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een standaardafwijking van '{standardDeviation}' is ongeldig voor stochast '{stochastName}'. " +
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
        public void TrySetMean_DistributionMeanValidValue_ReturnTrue(double? mean, double? standardDeviation)
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
        public void TrySetDistributionProperties_SettingDistributionMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(mean, 2.2, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_SettingDistributionStandardDeviationThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(1.1, standardDeviation, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een standaardafwijking van '{standardDeviation}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetMean_VariationCoefficientDistributionNull_ThrownArgumentNullException()
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
        public void TrySetMean_VariationCoefficientDistributionMeanNull_ReturnTrue()
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
        public void TrySetMean_VariationCoefficientDistributionMeanValid_SetMeanAndReturnTrue()
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
        public void TrySetMean_SettingVariationCoefficientDistributionMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetMean(mean, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetVariationCoefficient_VariationCoefficientDistributionNull_ThrownArgumentNullException()
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
        public void TrySetVariationCoefficient_VariationCoefficientDistributionVariationCoefficientNull_ReturnTrue()
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
        public void TrySetVariationCoefficient_VariationCoefficientDistributionVariationCoefficientValid_SetVariationCoefficientAndReturnTrue()
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
        public void TrySetVariationCoefficient_SettingVariationCoefficientDistributionVariationCoefficientThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetVariationCoefficient(variationCoefficient, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een variatiecoëfficiënt van '{variationCoefficient}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_VariationCoefficientDistributionNull_ThrownArgumentNullException()
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
        public void TrySetMean_VariationCoefficientDistributionMeanValidValue_ReturnTrue(double? mean, double? variationCoefficient)
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
        public void TrySetDistributionProperties_SettingVariationCoefficientDistributionMeanThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(mean, 2.2, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een gemiddelde van '{mean}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetDistributionProperties_SettingVariationCoefficientDistributionVariationCoefficientThrowArgumentOutOfRangeException_LogErrorAndReturnFalse()
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
            var result = true;
            Action call = () => result = distribution.TrySetDistributionProperties(1.1, variationCoefficient, "B", "C");

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Een variatiecoëfficiënt van '{variationCoefficient}' is ongeldig voor stochast '{stochastName}'. " +
                exceptionMessage +
                $" Berekening '{calculationName}' is overgeslagen.",
                LogLevelConstant.Error);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(result);

            mocks.VerifyAll();
        }
    }
}