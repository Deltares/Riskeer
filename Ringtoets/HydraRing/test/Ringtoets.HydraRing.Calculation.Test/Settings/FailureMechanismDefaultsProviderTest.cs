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

using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Settings;

namespace Ringtoets.HydraRing.Calculation.Test.Settings
{
    [TestFixture]
    public class FailureMechanismDefaultsProviderTest
    {
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, 2, 26, new[]
        {
            1
        })]
        [TestCase(HydraRingFailureMechanismType.QVariant, 3, 6, 114, new[]
        {
            3,
            4,
            5
        })]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, 2, 28, new[]
        {
            11
        })]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 11, 2, 29, new[]
        {
            14
        })]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 11, 2, 29, new[]
        {
            16
        })]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 101, 1, 1, new[]
        {
            102,
            103
        })]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 103, 1, 44, new[]
        {
            311,
            313,
            314
        })]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 110, 1, 60, new[]
        {
            421,
            422,
            423
        })]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 111, 1, 65, new[]
        {
            422,
            424,
            425,
            426,
            427
        })]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 112, 1, 65, new[]
        {
            422,
            424,
            425,
            430,
            431,
            432,
            433,
            434,
            435
        })]
        public void GetFailureMechanismDefaults_ReturnsExpectedFailureMechanismDefaults(HydraRingFailureMechanismType failureMechanismType, int expectedMechanismId, int expectedCalculationTypeId, int expectedVariableId, IEnumerable<int> expectedSubMechanismIds)
        {
            // Setup
            var failureMechanismDefaultsProvider = new FailureMechanismDefaultsProvider();

            // Call
            var failureMechanismDefaults = failureMechanismDefaultsProvider.GetFailureMechanismDefaults(failureMechanismType);

            // Assert
            Assert.AreEqual(expectedMechanismId, failureMechanismDefaults.MechanismId);
            Assert.AreEqual(expectedCalculationTypeId, failureMechanismDefaults.CalculationTypeId);
            Assert.AreEqual(expectedVariableId, failureMechanismDefaults.VariableId);
            Assert.AreEqual(expectedSubMechanismIds, failureMechanismDefaults.SubMechanismIds);
        }
    }
}
