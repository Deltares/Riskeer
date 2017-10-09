// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTransformerTest
    {
        [Test]
        public void Transform_PreconsolidationStressNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStress", exception.ParamName);
        }

        [Test]
        public void Transform_ValidPreconsolidationStress_ReturnMacroStabilityInwardsPreconsolidationStress()
        {
            // Setup
            var random = new Random(21);
            var preconsolidationStress = new PreconsolidationStress
            {
                XCoordinate = random.NextDouble(),
                ZCoordinate = random.NextDouble(),
                StressDistributionType = 3,
                StressMean = random.NextDouble(),
                StressCoefficientOfVariation = random.NextDouble(),
                StressShift = 0
            };

            // Call
            MacroStabilityInwardsPreconsolidationStress transformedStress =
                MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            Assert.AreEqual(preconsolidationStress.XCoordinate, transformedStress.Location.X);
            Assert.AreEqual(preconsolidationStress.ZCoordinate, transformedStress.Location.Y);

            VariationCoefficientLogNormalDistribution transformedPreconsolidationStressDistribution = transformedStress.Stress;
            Assert.AreEqual(preconsolidationStress.StressMean, transformedPreconsolidationStressDistribution.Mean,
                            transformedPreconsolidationStressDistribution.GetAccuracy());
            Assert.AreEqual(preconsolidationStress.StressCoefficientOfVariation,
                            transformedPreconsolidationStressDistribution.CoefficientOfVariation,
                            transformedPreconsolidationStressDistribution.GetAccuracy());
        }

        [Test]
        [TestCaseSource(nameof(GetPreconsolidationStressCombinationWithNaNValues))]
        public void Transform_PreconsolidationStressValuesNaN_ThrowsImportedDataTransformException(
            PreconsolidationStress preconsolidationStress,
            string parameterName)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = $"De waarde voor parameter '{parameterName}' voor de grensspanning moet een concreet getal zijn.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        [TestCaseSource(nameof(GetPreconsolidationStressInvalidDistributionValues))]
        public void Transform_InvalidPreconsolidationStressDistributionValues_ThrowsImportedDataTransformException(PreconsolidationStress preconsolidationStress)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual("Parameter 'Grensspanning' is niet lognormaal verdeeld.", exception.Message);
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(exception.InnerException);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidStochastConfiguration))]
        public void Transform_InvalidStochasticDistributionProperties_ThrowsImportedDataTransformException(
            PreconsolidationStress preconsolidationStress)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual("Parameter 'Grensspanning' is niet lognormaal verdeeld.", exception.Message);
        }

        private static IEnumerable<TestCaseData> GetInvalidStochastConfiguration()
        {
            var random = new Random(21);

            yield return new TestCaseData(new PreconsolidationStress
            {
                StressDistributionType = random.Next(3, int.MaxValue),
                StressShift = 0
            }).SetName("Invalid DistributionType");

            yield return new TestCaseData(new PreconsolidationStress
            {
                StressDistributionType = 3,
                StressShift = random.NextDouble()
            }).SetName("Invalid Shift");
        }

        private static IEnumerable<TestCaseData> GetPreconsolidationStressInvalidDistributionValues()
        {
            var random = new Random(21);
            const double preconsolidationStressMean = 0.005;

            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = random.NextDouble(),
                ZCoordinate = random.NextDouble(),
                StressMean = -1,
                StressCoefficientOfVariation = random.NextDouble()
            }).SetName("Invalid Mean");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = random.NextDouble(),
                ZCoordinate = random.NextDouble(),
                StressMean = preconsolidationStressMean,
                StressCoefficientOfVariation = -1
            }).SetName("Invalid Coefficient of Variation");
        }

        private static IEnumerable<TestCaseData> GetPreconsolidationStressCombinationWithNaNValues()
        {
            var random = new Random(21);
            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            const double preconsolidationStressMean = 0.005;
            double preconsolidationStressCoefficientOfVariation = random.NextDouble();

            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = double.NaN,
                ZCoordinate = zCoordinate,
                StressMean = preconsolidationStressMean,
                StressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }, "X-coördinaat").SetName("XCoordinate NaN");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = xCoordinate,
                ZCoordinate = double.NaN,
                StressMean = preconsolidationStressMean,
                StressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }, "Z-coördinaat").SetName("ZCoordinate NaN");
        }
    }
}