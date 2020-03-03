// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> tailorMadeWaveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            var output = new GrassCoverErosionOutwardsWaveConditionsOutput(waveRunUpOutput, waveImpactOutput, tailorMadeWaveImpactOutput);

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.AreSame(waveImpactOutput, output.WaveImpactOutput);
            Assert.AreSame(tailorMadeWaveImpactOutput, output.TailorMadeWaveImpactOutput);
        }

        [Test]
        public void Clone_WithOutputSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new GrassCoverErosionOutwardsWaveConditionsOutput(new[]
                                                                             {
                                                                                 WaveConditionsTestDataGenerator.GetRandomWaveConditionsOutput()
                                                                             },
                                                                             new[]
                                                                             {
                                                                                 WaveConditionsTestDataGenerator.GetRandomWaveConditionsOutput()
                                                                             },
                                                                             new[]
                                                                             {
                                                                                 WaveConditionsTestDataGenerator.GetRandomWaveConditionsOutput()
                                                                             }
            );

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionOutwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_OutputNull_ReturnsNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new GrassCoverErosionOutwardsWaveConditionsOutput(null, null, null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionOutwardsCloneAssert.AreClones);
        }
    }
}