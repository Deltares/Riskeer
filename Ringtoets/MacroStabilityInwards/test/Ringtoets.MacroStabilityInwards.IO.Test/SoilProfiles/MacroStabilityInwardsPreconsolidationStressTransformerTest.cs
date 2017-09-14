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
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

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
                PreconsolidationStressDistributionType = 3,
                PreconsolidationStressMean = random.NextDouble(),
                PreconsolidationStressCoefficientOfVariation = random.NextDouble(),
                PreconsolidationStressShift = 0
            };

            // Call
            MacroStabilityInwardsPreconsolidationStress transformedStress =
                MacroStabilityInwardsPreconsolidationStressTransformer.Transform(preconsolidationStress);

            // Assert
            Assert.AreEqual(preconsolidationStress.XCoordinate, transformedStress.XCoordinate);
            Assert.AreEqual(preconsolidationStress.ZCoordinate, transformedStress.ZCoordinate);
            Assert.AreEqual(preconsolidationStress.PreconsolidationStressMean, transformedStress.PreconsolidationStressMean);
            Assert.AreEqual(preconsolidationStress.PreconsolidationStressCoefficientOfVariation,
                            transformedStress.PreconsolidationStressCoefficientOfVariation);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidPreconsolidationStress))]
        public void Transform_InvalidPreconsolidationStressValues_ThrowsImportedDataTransformException(
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
                PreconsolidationStressDistributionType = random.Next(),
                PreconsolidationStressShift = 0
            }).SetName("Invalid DistributionType");

            yield return new TestCaseData(new PreconsolidationStress
            {
                PreconsolidationStressDistributionType = 3,
                PreconsolidationStressShift = random.NextDouble()
            }).SetName("Invalid Shift");
        }

        private static IEnumerable<TestCaseData> GetInvalidPreconsolidationStress()
        {
            var random = new Random(21);
            double xCoordinate = random.NextDouble();
            double zCoordinate = random.NextDouble();
            double preconsolidationStressMean = random.NextDouble();
            double preconsolidationStressCoefficientOfVariation = random.NextDouble();

            yield return new TestCaseData(new PreconsolidationStress
            {
                ZCoordinate = zCoordinate,
                PreconsolidationStressMean = preconsolidationStressMean,
                PreconsolidationStressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }, "X-coördinaat").SetName("Invalid XCoordinate");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = xCoordinate,
                PreconsolidationStressMean = preconsolidationStressMean,
                PreconsolidationStressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }, "Z-coördinaat").SetName("Invalid ZCoordinate");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = xCoordinate,
                ZCoordinate = zCoordinate,
                PreconsolidationStressCoefficientOfVariation = preconsolidationStressCoefficientOfVariation
            }, "gemiddelde").SetName("Invalid Mean");
            yield return new TestCaseData(new PreconsolidationStress
            {
                XCoordinate = xCoordinate,
                ZCoordinate = zCoordinate,
                PreconsolidationStressMean = preconsolidationStressMean
            }, "variatiecoëfficient").SetName("Invalid Coefficient of Variation");
        }
    }
}