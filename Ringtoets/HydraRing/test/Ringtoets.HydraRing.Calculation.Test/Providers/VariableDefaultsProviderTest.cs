// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Defaults;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Test.Providers
{
    [TestFixture]
    public class VariableDefaultsProviderTest
    {
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 26, 300)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 28, 300)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 29, 300)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 29, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 113, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 114, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 115, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 116, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 117, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 118, 300)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 119, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 1, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 8, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 10, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 11, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 12, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 17, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 120, 300)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 123, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 1, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 8, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 10, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 11, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 12, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 17, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 120, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 123, 300)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 58, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 59, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 60, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 61, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 62, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 94, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 95, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 96, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 97, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 103, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 104, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 105, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 106, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 107, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 108, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 58, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 59, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 61, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 62, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 63, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 65, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 66, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 67, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 68, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 69, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 71, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 72, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 93, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 94, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 95, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 96, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 97, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 103, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 104, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 105, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 106, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 107, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 108, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 125, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 129, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 43, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 58, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 60, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 61, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 63, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 65, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 66, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 67, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 80, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 81, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 82, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 83, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 84, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 85, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 86, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 87, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 88, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 89, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 90, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 91, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 92, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 93, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 94, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 95, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 96, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 97, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 103, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 104, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 105, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 106, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 108, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 125, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 130, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 131, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 132, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 133, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 134, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 135, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 136, 999999)]
        [TestCase(HydraRingFailureMechanismType.DunesBoundaryConditions, 26, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 1, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 8, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 10, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 11, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 12, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 17, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 120, 300)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 123, 300)]
        public void GetVariableDefaults_ReturnsExpectedVariableDefaults(HydraRingFailureMechanismType failureMechanismType, int variableId, int expectedCorrelationLength)
        {
            // Setup
            var variableDefaultsProvider = new VariableDefaultsProvider();

            // Call
            VariableDefaults variableDefaults = variableDefaultsProvider.GetVariableDefaults(failureMechanismType, variableId);

            // Assert
            Assert.AreEqual(expectedCorrelationLength, variableDefaults.CorrelationLength);
        }
    }
}