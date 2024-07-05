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

namespace Riskeer.HeightStructures.Data.Test
{
    [TestFixture]
    public class GeneralHeightStructuresInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var modelFactorStorageVolume = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1.00,
                StandardDeviation = (RoundedDouble) 0.20
            };

            var modelFactorOvertoppingFlow = new LogNormalDistribution(3)
            {
                Mean = (RoundedDouble) 0.09,
                StandardDeviation = (RoundedDouble) 0.06
            };

            // Call
            var generalHeightStructuresInput = new GeneralHeightStructuresInput();

            // Assert
            Assert.AreEqual(2, generalHeightStructuresInput.GravitationalAcceleration.NumberOfDecimalPlaces);
            Assert.AreEqual(9.81, generalHeightStructuresInput.GravitationalAcceleration, generalHeightStructuresInput.GravitationalAcceleration.GetAccuracy());

            DistributionAssert.AreEqual(modelFactorOvertoppingFlow, generalHeightStructuresInput.ModelFactorOvertoppingFlow);
            DistributionAssert.AreEqual(modelFactorStorageVolume, generalHeightStructuresInput.ModelFactorStorageVolume);

            Assert.AreEqual(2, generalHeightStructuresInput.ModelFactorInflowVolume.NumberOfDecimalPlaces);
            Assert.AreEqual(1, generalHeightStructuresInput.ModelFactorInflowVolume, generalHeightStructuresInput.ModelFactorInflowVolume.GetAccuracy());
        }
    }
}