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
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerDistributionHelperTest
    {
        [Test]
        public void ValidateIsNonShiftedLogNormal_ParameterNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            long distributionType = random.Next();
            double shift = random.NextDouble();

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(distributionType,
                                                                                                shift,
                                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parameterName", exception.ParamName);
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(3, 10)]
        public void ValidateIsNonShiftedLogNormal_InvalidDistributionProperties_ThrowsImportedDataException(
            long distributionType,
            double shift)
        {
            // Setup
            const string parameterName = "Just a name";

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(distributionType,
                                                                                                shift,
                                                                                                parameterName);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual($"Parameter '{parameterName}' is niet lognormaal verdeeld.", exception.Message);
        }

        [Test]
        public void ValidateIsNonShiftedLogNormal_ValidDistribution_DoesNotThrowException()
        {
            // Setup
            const long distributionType = 3;
            const double shift = 0;

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(distributionType,
                                                                                                shift,
                                                                                                string.Empty);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateIsNonShiftedLogNormal_DistributionTypeNull_DoesNotThrowException()
        {
            // Setup
            var random = new Random(21);
            double shift = random.NextDouble();
            const string parameterName = "Just a name";

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsNonShiftedLogNormal(null,
                                                                                                shift,
                                                                                                parameterName);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateIsLogNormal_ParameterNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            long distributionType = random.Next();

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsLogNormal(distributionType,
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parameterName", exception.ParamName);
        }

        [Test]
        public void ValidateIsLogNormal_InvalidDistributionProperties_ThrowsImportedDataException()
        {
            // Setup
            const long invalidDistributionType = -1;
            const string parameterName = "Just a name";

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsLogNormal(invalidDistributionType,
                                                                                      parameterName);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            Assert.AreEqual($"Parameter '{parameterName}' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        public void ValidateIsLogNormal_ValidDistribution_DoesNotThrowException()
        {
            // Setup
            const long distributionType = 3;

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsLogNormal(distributionType,
                                                                                      string.Empty);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateIsLogNormal_DistributionTypeNull_DoesNotThrowException()
        {
            // Setup
            const string parameterName = "Just a name";

            // Call
            TestDelegate call = () => SoilLayerDistributionHelper.ValidateIsLogNormal(null,
                                                                                      parameterName);

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}