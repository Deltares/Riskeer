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
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;

namespace Riskeer.HydraRing.Calculation.Test.Data
{
    [TestFixture]
    public class HydraRingFailureMechanismTypeTest
    {
        [Test]
        public void Values_ExpectedValues()
        {
            Assert.AreEqual(12, Enum.GetValues(typeof(HydraRingFailureMechanismType)).Length);

            const string message = "Value no longer corresponds to id in Hydra-Ring settings database files.";
            Assert.AreEqual(0, (int) HydraRingFailureMechanismType.AssessmentLevel, message);
            Assert.AreEqual(1, (int) HydraRingFailureMechanismType.QVariant, message);
            Assert.AreEqual(2, (int) HydraRingFailureMechanismType.WaveHeight, message);
            Assert.AreEqual(3, (int) HydraRingFailureMechanismType.WavePeakPeriod, message);
            Assert.AreEqual(4, (int) HydraRingFailureMechanismType.WaveSpectralPeriod, message);
            Assert.AreEqual(5, (int) HydraRingFailureMechanismType.DikeHeight, message);
            Assert.AreEqual(6, (int) HydraRingFailureMechanismType.DikesOvertopping, message);
            Assert.AreEqual(7, (int) HydraRingFailureMechanismType.StructuresOvertopping, message);
            Assert.AreEqual(8, (int) HydraRingFailureMechanismType.StructuresClosure, message);
            Assert.AreEqual(9, (int) HydraRingFailureMechanismType.StructuresStructuralFailure, message);
            Assert.AreEqual(10, (int) HydraRingFailureMechanismType.DunesBoundaryConditions, message);
            Assert.AreEqual(11, (int) HydraRingFailureMechanismType.OvertoppingRate, message);
        }
    }
}