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

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GeneralGrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralGrassCoverErosionInwardsInput();

            // Assert
            var fbFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 4.75), StandardDeviation = new RoundedDouble(2, 0.5)
            };
            Assert.AreEqual(fbFactor.Mean, inputParameters.FbFactor.Mean);
            Assert.AreEqual(fbFactor.StandardDeviation, inputParameters.FbFactor.StandardDeviation);

            var fnFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 2.6), StandardDeviation = new RoundedDouble(2, 0.35)
            };
            Assert.AreEqual(fnFactor.Mean, inputParameters.FnFactor.Mean);
            Assert.AreEqual(fnFactor.StandardDeviation, inputParameters.FnFactor.StandardDeviation);

            var fshallow = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 0.92), StandardDeviation = new RoundedDouble(2, 0.24)
            };
            Assert.AreEqual(fshallow.Mean, inputParameters.FshallowModelFactor.Mean);
            Assert.AreEqual(fshallow.StandardDeviation, inputParameters.FshallowModelFactor.StandardDeviation);

            var frunupModelFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1), StandardDeviation = new RoundedDouble(2, 0.07)
            };
            Assert.AreEqual(frunupModelFactor.Mean, inputParameters.FrunupModelFactor.Mean);
            Assert.AreEqual(frunupModelFactor.StandardDeviation, inputParameters.FrunupModelFactor.StandardDeviation);

            Assert.AreEqual(1, inputParameters.CriticalOvertoppingModelFactor);
            Assert.AreEqual(1, inputParameters.OvertoppingModelFactor);
        }
    }
}