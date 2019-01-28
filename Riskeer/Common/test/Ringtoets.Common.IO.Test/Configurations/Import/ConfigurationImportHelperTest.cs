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
using Core.Common.Base;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Import
{
    public class ConfigurationImportHelperTest
    {
        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TrySetStandardDeviationStochast_ValidStochastConfiguration_ReturnsTrueParametersSet(bool setMean, bool setStandardDeviation)
        {
            // Setup
            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            var configuration = new StochastConfiguration();
            var input = new TestInputWithStochasts();

            var random = new Random(21);
            double mean = random.NextDouble();
            double standardDeviation = random.NextDouble();
            if (setMean)
            {
                configuration.Mean = mean;
            }

            if (setStandardDeviation)
            {
                configuration.StandardDeviation = standardDeviation;
            }

            // Call
            bool valid = ConfigurationImportHelper.TrySetStandardDeviationStochast(
                "some stochast name",
                "some calculation name",
                input,
                configuration, i => i.Distribution,
                (i, s) => i.Distribution = s,
                log);

            // Assert
            Assert.IsTrue(valid);
            var defaultLogNormal = new LogNormalDistribution();
            Assert.AreEqual(
                setMean ? mean : defaultLogNormal.Mean,
                input.Distribution.Mean,
                input.Distribution.Mean.GetAccuracy());
            Assert.AreEqual(
                setStandardDeviation ? standardDeviation : defaultLogNormal.StandardDeviation,
                input.Distribution.StandardDeviation,
                input.Distribution.StandardDeviation.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetStandardDeviationStochast_StochastConfigurationWithStandardDeviation_LogsErrorReturnsFalse()
        {
            // Setup
            const string stochastName = "some stochast name";
            const string calculationName = "some calculation name";

            const string expectedFormat = "{0} Berekening '{1}' is overgeslagen.";
            string expectedError = $"Indien voor parameter '{stochastName}' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                                   $"Voor berekening '{calculationName}' is een variatiecoëfficiënt gevonden.";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat(expectedFormat, expectedError, calculationName));
            mocks.ReplayAll();

            var configuration = new StochastConfiguration
            {
                VariationCoefficient = new Random(21).NextDouble()
            };

            var input = new TestInputWithStochasts();

            // Call
            bool valid = ConfigurationImportHelper.TrySetStandardDeviationStochast(
                stochastName,
                calculationName,
                input, configuration,
                i => i.Distribution,
                (i, s) => i.Distribution = s,
                log);

            // Assert
            Assert.IsFalse(valid);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TrySetVariationCoefficientStochast_ValidStochastConfiguration_ReturnsTrueParametersSet(bool setMean, bool setVariationCoefficient)
        {
            // Setup
            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            mocks.ReplayAll();

            var configuration = new StochastConfiguration();

            var random = new Random(21);
            double mean = random.NextDouble();
            double variationCoefficient = random.NextDouble();
            if (setMean)
            {
                configuration.Mean = mean;
            }

            if (setVariationCoefficient)
            {
                configuration.VariationCoefficient = variationCoefficient;
            }

            var input = new TestInputWithStochasts();

            // Call
            bool valid = ConfigurationImportHelper.TrySetVariationCoefficientStochast(
                "some stochast name",
                "some calculation name",
                input, configuration,
                i => i.VariationCoefficientDistribution,
                (i, s) => i.VariationCoefficientDistribution = s,
                log);

            // Assert
            Assert.IsTrue(valid);
            var defaultLogNormal = new VariationCoefficientLogNormalDistribution();
            Assert.AreEqual(
                setMean ? mean : defaultLogNormal.Mean,
                input.VariationCoefficientDistribution.Mean,
                input.VariationCoefficientDistribution.Mean.GetAccuracy());
            Assert.AreEqual(
                setVariationCoefficient ? variationCoefficient : defaultLogNormal.CoefficientOfVariation,
                input.VariationCoefficientDistribution.CoefficientOfVariation,
                input.VariationCoefficientDistribution.CoefficientOfVariation.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void TrySetVariationCoefficientStochast_StochastConfigurationWithStandardDeviation_LogsErrorReturnsFalse()
        {
            // Setup
            const string stochastName = "some stochast name";
            const string calculationName = "some calculation name";

            const string expectedFormat = "{0} Berekening '{1}' is overgeslagen.";
            string expectedError = $"Indien voor parameter '{stochastName}' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                                   $"Voor berekening '{calculationName}' is een standaardafwijking gevonden.";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat(expectedFormat, expectedError, calculationName));
            mocks.ReplayAll();

            var configuration = new StochastConfiguration
            {
                StandardDeviation = new Random(21).NextDouble()
            };

            var input = new TestInputWithStochasts();

            // Call
            bool valid = ConfigurationImportHelper.TrySetVariationCoefficientStochast(
                stochastName,
                calculationName,
                input, configuration,
                i => i.VariationCoefficientDistribution,
                (i, s) => i.VariationCoefficientDistribution = s,
                log);

            // Assert
            Assert.IsFalse(valid);

            mocks.VerifyAll();
        }

        private class TestInputWithStochasts : CloneableObservable, ICalculationInput
        {
            public TestInputWithStochasts()
            {
                Distribution = new LogNormalDistribution();
                VariationCoefficientDistribution = new VariationCoefficientLogNormalDistribution();
            }

            public IDistribution Distribution { get; set; }
            public IVariationCoefficientDistribution VariationCoefficientDistribution { get; set; }
        }
    }
}