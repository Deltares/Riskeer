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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
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
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 1, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 8, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 10, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 11, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 12, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 17, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 120, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 123, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 1, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 8, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 10, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 11, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 12, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 17, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 120, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 123, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 23, 6000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 42, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 43, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 44, 200)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 45, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 46, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 47, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 48, 3000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 49, 200)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 50, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 51, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 52, 600)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 53, 999999)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 54, 99000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 55, 600)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 56, 180)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 58, 99000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 124, 300)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 127, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 20, 900)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 21, 900)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 23, 6000)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 58, 99000)]
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
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 107, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 108, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 23, 6000)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 58, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 62, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 63, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 64, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 65, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 68, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 69, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 71, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 93, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 94, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 95, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 96, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 97, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 103, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 104, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 105, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 106, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 108, 999999)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 129, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 20, 900)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 21, 900)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 23, 6000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 43, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 58, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 60, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 61, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 62, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 63, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 64, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 65, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 80, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 82, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 83, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 85, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 86, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 87, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 88, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 89, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 90, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 91, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 92, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 93, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 94, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 95, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 96, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 97, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 103, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 104, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 105, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 106, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 108, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 130, 6000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 131, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 132, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 133, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 134, 50)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 135, 99000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 136, 99000)]
        public void GetVariableDefaults_ReturnsExpectedVariableDefaults(HydraRingFailureMechanismType failureMechanismType, int variableId, int expectedCorrelationLength)
        {
            // Setup
            var variableDefaultsProvider = new VariableDefaultsProvider();

            // Call
            var variableDefaults = variableDefaultsProvider.GetVariableDefaults(failureMechanismType, variableId);

            // Assert
            Assert.AreEqual(expectedCorrelationLength, variableDefaults.CorrelationLength);
        }
    }
}