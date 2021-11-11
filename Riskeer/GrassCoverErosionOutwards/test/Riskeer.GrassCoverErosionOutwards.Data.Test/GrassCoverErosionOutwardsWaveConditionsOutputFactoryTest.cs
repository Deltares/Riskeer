// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputFactoryTest
    {
        [Test]
        public void CreateOutputWithWaveRunUp_WaveRunUpOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUp(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveRunUpOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUp_WithWaveRunUpOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUp(waveRunUpOutput);

            // Assert
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.IsNull(output.WaveImpactOutput);
            Assert.IsNull(output.WaveImpactWithWaveDirectionOutput);
        }

        [Test]
        public void CreateOutputWithWaveImpact_WaveImpactOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpact(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveImpact_WithWaveImpactOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpact(waveImpactOutput);

            // Assert
            Assert.AreSame(waveImpactOutput, output.WaveImpactOutput);
            Assert.IsNull(output.WaveRunUpOutput);
            Assert.IsNull(output.WaveImpactWithWaveDirectionOutput);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpact_WaveRunUpOutputNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpact(null,
                                                                                                            waveImpactOutput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveRunUpOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpact_WaveImpactOutputNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpact(waveRunUpOutput,
                                                                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpact_WithOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpact(waveRunUpOutput,
                                                                                                            waveImpactOutput);

            // Assert
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.AreSame(waveImpactOutput, output.WaveImpactOutput);
            Assert.IsNull(output.WaveImpactWithWaveDirectionOutput);
        }

        [Test]
        public void CreateOutputWithWaveImpactWithWaveDirection_WaveImpactWithWaveDirectionOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpactWithWaveDirection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactWithWaveDirectionOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveImpactWithWaveDirection_WithOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirectionOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpactWithWaveDirection(waveImpactWithWaveDirectionOutput);

            // Assert
            Assert.AreSame(waveImpactWithWaveDirectionOutput, output.WaveImpactWithWaveDirectionOutput);
            Assert.IsNull(output.WaveRunUpOutput);
            Assert.IsNull(output.WaveImpactOutput);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection_WaveRunUpOutputNull_ThrowsArgumentNullException()
        {
            //Setup
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirection = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection(null,
                                                                                                                             waveImpactWithWaveDirection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveRunUpOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection_WaveImpactWithWaveDirectionOutputNull_ThrowsArgumentNullException()
        {
            //Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection(waveRunUpOutput,
                                                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactWithWaveDirectionOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection_WithOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirectionOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpactWithWaveDirection(waveRunUpOutput,
                                                                                                                             waveImpactWithWaveDirectionOutput);

            // Assert
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.AreSame(waveImpactWithWaveDirectionOutput, output.WaveImpactWithWaveDirectionOutput);
            Assert.IsNull(output.WaveImpactOutput);
        }

        [Test]
        public void CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection_WaveRunUpOutputNull_ThrowsArgumentNullException()
        {
            //Setup
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirectionOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection(null, 
                                                                                                                                       waveImpactOutput,
                                                                                                                                       waveImpactWithWaveDirectionOutput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveRunUpOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection_WaveImpactOutputNull_ThrowsArgumentNullException()
        {
            //Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirectionOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection(waveRunUpOutput,
                                                                                                                                       null,
                                                                                                                                       waveImpactWithWaveDirectionOutput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection_WaveImpactWithWaveDirectionOutputNull_ThrowsArgumentNullException()
        {
            //Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection(waveRunUpOutput,
                                                                                                                                       waveImpactOutput,
                                                                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveImpactWithWaveDirectionOutput", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection_WithOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> waveRunUpOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> waveImpactWithWaveDirectionOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpWaveImpactAndWaveImpactWithWaveDirection(waveRunUpOutput,
                                                                                                                                       waveImpactOutput,
                                                                                                                                       waveImpactWithWaveDirectionOutput);

            // Assert
            Assert.AreSame(waveRunUpOutput, output.WaveRunUpOutput);
            Assert.AreSame(waveImpactOutput, output.WaveImpactOutput);
            Assert.AreSame(waveImpactWithWaveDirectionOutput, output.WaveImpactWithWaveDirectionOutput);
        }
    }
}