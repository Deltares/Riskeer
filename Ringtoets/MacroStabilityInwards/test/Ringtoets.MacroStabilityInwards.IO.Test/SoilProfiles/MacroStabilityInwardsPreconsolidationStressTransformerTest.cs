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
using Core.Common.Base.Geometry;
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
            PreconsolidationStress preconsolidationStress)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
        }

        [Test]
        [SetCulture("NL-nl")]
        [TestCaseSource(nameof(GetPreconsolidationStressInvalidDistributionValues))]
        public void Transform_InvalidPreconsolidationStressDistributionValues_ThrowsImportedDataTransformException(PreconsolidationStress preconsolidationStress)
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(innerException);
            var coordinate = new Point2D(preconsolidationStress.XCoordinate, preconsolidationStress.ZCoordinate);
            string expectedMessage = CreateExpectedErrorMessage(coordinate, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_InvalidStochasticDistributionType_ThrowsImportedDataTransformException()
        {
            // Setup
            var random = new Random(21);
            var preconsolidationStress = new PreconsolidationStress
            {
                StressDistributionType = random.Next(3, int.MaxValue),
                StressShift = 0
            };

            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            var coordinate = new Point2D(preconsolidationStress.XCoordinate, preconsolidationStress.ZCoordinate);
            string expectedMessage = CreateExpectedErrorMessage(coordinate, "Parameter moet lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_InvalidStochasticDistributionShift_ThrowsImportedDataTransformException()
        {
            // Setup
            var random = new Random(21);
            var preconsolidationStress = new PreconsolidationStress
            {
                StressDistributionType = SoilLayerConstants.LogNormalDistributionValue,
                StressShift = random.NextDouble()
            };

            // Call
            TestDelegate call = () => MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            var coordinate = new Point2D(preconsolidationStress.XCoordinate, preconsolidationStress.ZCoordinate);
            string expectedMessage = CreateExpectedErrorMessage(coordinate, "Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        private static string CreateExpectedErrorMessage(Point2D location, string errorMessage)
        {
            return $"Grensspanning op locatie {location} heeft een ongeldige waarde. {errorMessage}";
        }

        #region Test data

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
            }).SetName("XCoordinate NaN");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = xCoordinate,
                ZCoordinate = double.NaN,
                StressMean = preconsolidationStressMean,
                StressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }).SetName("ZCoordinate NaN");
        }

        #endregion
    }
}