﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnsOutput()
        {
            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();

            // Assert
            CollectionAssert.IsEmpty(output.WaveRunUpOutput);
            CollectionAssert.IsEmpty(output.WaveImpactOutput);
        }

        [Test]
        public void Create_WithArguments_ReturnsExpectedOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(waveRunUpOutput, waveImpactOutput);

            // Assert
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.AreSame(waveImpactOutput, output.WaveImpactOutput);
        }
    }
}