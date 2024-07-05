// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GeneralGrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var fbFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 4.75,
                StandardDeviation = (RoundedDouble) 0.5,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var fnFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 2.6,
                StandardDeviation = (RoundedDouble) 0.35,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var fshallow = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 0.92,
                StandardDeviation = (RoundedDouble) 0.24,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            var frunupModelFactor = new TruncatedNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.07,
                LowerBoundary = (RoundedDouble) 0.0,
                UpperBoundary = (RoundedDouble) 99.0
            };

            // Call
            var inputParameters = new GeneralGrassCoverErosionInwardsInput();

            // Assert
            DistributionAssert.AreEqual(fbFactor, inputParameters.FbFactor);
            DistributionAssert.AreEqual(fnFactor, inputParameters.FnFactor);
            DistributionAssert.AreEqual(fshallow, inputParameters.FshallowModelFactor);
            DistributionAssert.AreEqual(frunupModelFactor, inputParameters.FrunupModelFactor);

            Assert.AreEqual(1, inputParameters.CriticalOvertoppingModelFactor);
            Assert.AreEqual(1, inputParameters.OvertoppingModelFactor);
        }
    }
}