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

using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class DistributionValidationTest
    {
        private const string paramName = "<a very nice parametername>";

        [Test]
        public void ValidateDistribution_ValidNormalDistribution_NoErrorMessage()
        {
            // Call
            string[] message = DistributionValidation.ValidateDistribution(new NormalDistribution(2), paramName);

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        public void ValidateDistribution_ValidLogNormalDistribution_NoErrorMessage()
        {
            // Call
            string[] message = DistributionValidation.ValidateDistribution(new LogNormalDistribution(2), paramName);

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        public void ValidateDistribution_ValidVariationCoefficientNormalDistribution_NoErrorMessage()
        {
            // Call
            string[] message = DistributionValidation.ValidateDistribution(new VariationCoefficientNormalDistribution(2), paramName);

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        public void ValidateDistribution_ValidVariationCoefficientLogNormalDistribution_NoErrorMessage()
        {
            // Call
            string[] message = DistributionValidation.ValidateDistribution(new VariationCoefficientLogNormalDistribution(2), paramName);

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidMeanNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", paramName);
            var distribution = new NormalDistribution(2)
            {
                Mean = (RoundedDouble) value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidMeanLogNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De verwachtingswaarde voor '{0}' moet een positief getal zijn.", paramName);
            var distribution = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidMeanVariationCoefficientNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De verwachtingswaarde voor '{0}' moet een concreet getal zijn.", paramName);
            var distribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidMeanVariationCoefficientLogNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De verwachtingswaarde voor '{0}' moet een positief getal zijn.", paramName);
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidStandardDeviationNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", paramName);
            var distribution = new NormalDistribution(2)
            {
                StandardDeviation = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidStandardDeviationLogNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De standaard afwijking voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", paramName);
            var distribution = new LogNormalDistribution(2)
            {
                StandardDeviation = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidVariationCoefficientNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", paramName);
            var distribution = new VariationCoefficientNormalDistribution(2)
            {
                CoefficientOfVariation = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidVariationCoefficientLogNormalDistribution_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", paramName);
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                CoefficientOfVariation = (RoundedDouble)value
            };

            // Call
            string[] message = DistributionValidation.ValidateDistribution(distribution, paramName);

            // Assert
            Assert.AreEqual(1, message.Length);
            StringAssert.StartsWith(expectedMessage, message[0]);
        }
    }
}