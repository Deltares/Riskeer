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

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class GeneralHeightStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralHeightStructuresInput();

            // Assert
            Assert.AreEqual(9.81, inputParameters.GravitationalAcceleration, 1e-6);

            var modelfactorOvertopping = new NormalDistribution(3)
            {
                Mean = new RoundedDouble(3, 0.09),
                StandardDeviation = new RoundedDouble(3, 0.06)
            };
            Assert.AreEqual(modelfactorOvertopping.Mean, inputParameters.ModelFactorOvertoppingFlow.Mean, 1e-6);
            Assert.AreEqual(modelfactorOvertopping.StandardDeviation, inputParameters.ModelFactorOvertoppingFlow.StandardDeviation, 1e-6);

            var modelFactorForStorageVolume = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1.00),
                StandardDeviation = new RoundedDouble(2, 0.20)
            };
            Assert.AreEqual(modelFactorForStorageVolume.Mean, inputParameters.ModelFactorForStorageVolume.Mean, 1e-6);
            Assert.AreEqual(modelFactorForStorageVolume.StandardDeviation, inputParameters.ModelFactorForStorageVolume.StandardDeviation, 1e-6);

            Assert.AreEqual(1, inputParameters.ModelFactorForIncomingFlowVolume, 1e-6);
        }
    }
}