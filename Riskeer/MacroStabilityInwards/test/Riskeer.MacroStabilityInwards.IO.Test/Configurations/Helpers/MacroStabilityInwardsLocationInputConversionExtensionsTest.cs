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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class MacroStabilityInwardsLocationInputConversionExtensionsTest
    {
        [Test]
        public void ToMacroStabilityInwardsLocationInputConfiguration_MacroStabilityInwardsLocationInputDailyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IMacroStabilityInwardsLocationInputDaily) null).ToMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("inputDaily", exception.ParamName);
        }

        [Test]
        public void ToMacroStabilityInwardsLocationInputConfiguration_ValidMacroStabilityInwardsLocationInputDaily_ReturnsNewMacroStabilityInwardsLocationInputConfigurationWithParametersSet()
        {
            // Setup
            var random = new Random(31);

            bool useDefaultOffsets = random.NextBoolean();
            RoundedDouble waterLevelPolder = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();

            var mockRepository = new MockRepository();
            var inputDaily = mockRepository.Stub<IMacroStabilityInwardsLocationInputDaily>();
            inputDaily.UseDefaultOffsets = useDefaultOffsets;
            inputDaily.WaterLevelPolder = waterLevelPolder;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = phreaticLineOffsetBelowDikeTopAtRiver;
            inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = phreaticLineOffsetBelowDikeTopAtPolder;
            inputDaily.PhreaticLineOffsetBelowShoulderBaseInside = phreaticLineOffsetBelowShoulderBaseInside;
            inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = phreaticLineOffsetBelowDikeToeAtPolder;
            mockRepository.ReplayAll();

            // Call
            MacroStabilityInwardsLocationInputConfiguration configuration = inputDaily.ToMacroStabilityInwardsLocationInputConfiguration();

            // Assert
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver,
                            configuration.PhreaticLineOffsetBelowDikeTopAtRiver,
                            phreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder,
                            configuration.PhreaticLineOffsetBelowDikeTopAtPolder,
                            phreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside,
                            configuration.PhreaticLineOffsetBelowShoulderBaseInside,
                            phreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder,
                            configuration.PhreaticLineOffsetBelowDikeToeAtPolder,
                            phreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());
            mockRepository.VerifyAll();
        }

        [Test]
        public void ToMacroStabilityInwardsLocationInputExtremeConfiguration_MacroStabilityInwardsLocationInputExtremeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IMacroStabilityInwardsLocationInputExtreme) null).ToMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("inputExtreme", exception.ParamName);
        }

        [Test]
        public void ToMacroStabilityInwardsLocationInputExtremeConfiguration_ValidMacroStabilityInwardsLocationInputExtreme_ReturnsNewMacroStabilityInwardsLocationInputExtremeConfigurationWithParametersSet()
        {
            // Setup
            var random = new Random(31);

            RoundedDouble penetrationLength = random.NextRoundedDouble();
            bool useDefaultOffsets = random.NextBoolean();
            RoundedDouble waterLevelPolder = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeTopAtRiver = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeTopAtPolder = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowShoulderBaseInside = random.NextRoundedDouble();
            RoundedDouble phreaticLineOffsetBelowDikeToeAtPolder = random.NextRoundedDouble();

            var mockRepository = new MockRepository();
            var inputExtreme = mockRepository.Stub<IMacroStabilityInwardsLocationInputExtreme>();
            inputExtreme.PenetrationLength = penetrationLength;
            inputExtreme.UseDefaultOffsets = useDefaultOffsets;
            inputExtreme.WaterLevelPolder = waterLevelPolder;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = phreaticLineOffsetBelowDikeTopAtRiver;
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = phreaticLineOffsetBelowDikeTopAtPolder;
            inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = phreaticLineOffsetBelowShoulderBaseInside;
            inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = phreaticLineOffsetBelowDikeToeAtPolder;
            mockRepository.ReplayAll();

            // Call
            MacroStabilityInwardsLocationInputExtremeConfiguration configuration = inputExtreme.ToMacroStabilityInwardsLocationInputExtremeConfiguration();

            // Assert
            Assert.AreEqual(penetrationLength,
                            configuration.PenetrationLength,
                            penetrationLength.GetAccuracy());
            Assert.AreEqual(useDefaultOffsets, configuration.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver,
                            configuration.PhreaticLineOffsetBelowDikeTopAtRiver,
                            phreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder,
                            configuration.PhreaticLineOffsetBelowDikeTopAtPolder,
                            phreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside,
                            configuration.PhreaticLineOffsetBelowShoulderBaseInside,
                            phreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder,
                            configuration.PhreaticLineOffsetBelowDikeToeAtPolder,
                            phreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());
            mockRepository.VerifyAll();
        }
    }
}