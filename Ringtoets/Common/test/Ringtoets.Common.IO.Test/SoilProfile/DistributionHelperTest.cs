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
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class DistributionHelperTest
    {
        [Test]
        public void ValidateLogNormalDistribution_InvalidDistributionType_ThrowsDistributionValidationException()
        {
            // Setup
            const long distributionType = -1;

            var random = new Random(21);
            double shift = random.NextDouble();

            // Call
            TestDelegate call = () => DistributionHelper.ValidateLogNormalDistribution(distributionType,
                                                                                       shift);

            // Assert
            var exception = Assert.Throws<DistributionValidationException>(call);
            Assert.AreEqual("Parameter moet lognormaal verdeeld zijn.", exception.Message);
        }

        [Test]
        public void ValidateLogNormalDistribution_ShiftNonZero_ThrowsDistributionValidationException()
        {
            // Setup
            const long distributionType = SoilLayerConstants.LogNormalDistributionValue;

            var random = new Random(21);
            double shift = random.NextDouble();

            // Call
            TestDelegate call = () => DistributionHelper.ValidateLogNormalDistribution(distributionType,
                                                                                       shift);

            // Assert
            var exception = Assert.Throws<DistributionValidationException>(call);
            Assert.AreEqual("Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.", exception.Message);
        }

        [Test]
        public void ValidateLogNormalDistribution_ValidDistribution_DoesNotThrowException()
        {
            // Setup
            const long distributionType = SoilLayerConstants.LogNormalDistributionValue;
            const double shift = 0;

            // Call
            TestDelegate call = () => DistributionHelper.ValidateLogNormalDistribution(distributionType, shift);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateLogNormalDistribution_DistributionTypeNull_DoesNotThrowException()
        {
            // Setup
            var random = new Random(21);
            double shift = random.NextDouble();

            // Call
            TestDelegate call = () => DistributionHelper.ValidateLogNormalDistribution(null, shift);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateShiftedLogNormalDistribution_InvalidDistributionType_ThrowsDistributionValidationException()
        {
            // Setup
            const long invalidDistributionType = -1;

            // Call
            TestDelegate call = () => DistributionHelper.ValidateShiftedLogNormalDistribution(invalidDistributionType);

            // Assert
            var exception = Assert.Throws<DistributionValidationException>(call);
            Assert.AreEqual("Parameter moet verschoven lognormaal verdeeld zijn.", exception.Message);
        }

        [Test]
        public void ValidateShiftedLogNormalDistribution_ValidDistribution_DoesNotThrowException()
        {
            // Setup
            const long distributionType = SoilLayerConstants.LogNormalDistributionValue;

            // Call
            TestDelegate call = () => DistributionHelper.ValidateShiftedLogNormalDistribution(distributionType);

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}