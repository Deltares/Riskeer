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

using System;
using NUnit.Framework;

namespace Ringtoets.HydraRing.Calculation.Test
{
    [TestFixture]
    public class HydraRingFailureMechanismTypeTest
    {
        [Test]
        public void Values_HasTen()
        {
            Assert.AreEqual(10, Enum.GetValues(typeof(HydraRingFailureMechanismType)).Length);
        }

        [Test]
        public void ConvertToInteger_ForAllValues_ReturnsExpectedInteger()
        {
            Assert.AreEqual(1, (int) HydraRingFailureMechanismType.AssessmentLevel);
            Assert.AreEqual(3, (int) HydraRingFailureMechanismType.QVariant);
            Assert.AreEqual(11, (int) HydraRingFailureMechanismType.WaveHeight);
            Assert.AreEqual(11, (int) HydraRingFailureMechanismType.WavePeakPeriod);
            Assert.AreEqual(11, (int) HydraRingFailureMechanismType.WaveSpectralPeriod);
            Assert.AreEqual(101, (int) HydraRingFailureMechanismType.DikesOvertopping);
            Assert.AreEqual(103, (int) HydraRingFailureMechanismType.DikesPiping);
            Assert.AreEqual(110, (int) HydraRingFailureMechanismType.StructuresOvertopping);
            Assert.AreEqual(111, (int) HydraRingFailureMechanismType.StructuresClosure);
            Assert.AreEqual(112, (int) HydraRingFailureMechanismType.StructuresStructuralFailure);
        }
    }
}
