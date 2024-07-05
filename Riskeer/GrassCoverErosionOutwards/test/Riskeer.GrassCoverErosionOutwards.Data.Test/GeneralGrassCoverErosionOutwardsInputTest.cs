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

using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GeneralGrassCoverErosionOutwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new GeneralGrassCoverErosionOutwardsInput();

            // Assert
            GeneralWaveConditionsInput generalWaveImpactWaveConditionsInput = inputParameters.GeneralWaveImpactWaveConditionsInput;
            Assert.AreEqual(1.0, generalWaveImpactWaveConditionsInput.A, generalWaveImpactWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(0.67, generalWaveImpactWaveConditionsInput.B, generalWaveImpactWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(0.0, generalWaveImpactWaveConditionsInput.C, generalWaveImpactWaveConditionsInput.C.GetAccuracy());

            GeneralWaveConditionsInput generalWaveRunUpWaveConditionsInput = inputParameters.GeneralWaveRunUpWaveConditionsInput;
            Assert.AreEqual(1.0, generalWaveRunUpWaveConditionsInput.A, generalWaveRunUpWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(1.7, generalWaveRunUpWaveConditionsInput.B, generalWaveRunUpWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(0.3, generalWaveRunUpWaveConditionsInput.C, generalWaveRunUpWaveConditionsInput.C.GetAccuracy());

            GeneralWaveConditionsInput generalWaveImpactWithWaveDirectionWaveConditionsInput = inputParameters.GeneralWaveImpactWithWaveDirectionWaveConditionsInput;
            Assert.AreEqual(1.0, generalWaveImpactWithWaveDirectionWaveConditionsInput.A, generalWaveImpactWithWaveDirectionWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(0.67, generalWaveImpactWithWaveDirectionWaveConditionsInput.B, generalWaveImpactWithWaveDirectionWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(0.67, generalWaveImpactWithWaveDirectionWaveConditionsInput.C, generalWaveImpactWithWaveDirectionWaveConditionsInput.C.GetAccuracy());
        }
    }
}