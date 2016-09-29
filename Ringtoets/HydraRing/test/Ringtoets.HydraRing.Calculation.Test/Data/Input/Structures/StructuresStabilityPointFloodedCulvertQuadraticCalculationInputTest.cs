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

using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresStabilityPointFloodedCulvertQuadraticCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            var hydraRingSection = new HydraRingSection(1, double.NaN, double.NaN);
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();

            // Call
            var input = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(hydraulicBoundaryLocationId, hydraRingSection, forelandPoints,
                                                                                            1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10,
                                                                                            11.11, 12.12, 13.13, 14.14, 15.15, 16.16, 17.17,
                                                                                            18.18, 19.19, 20.20, 21.21, 22.22, 23.23, 24.24,
                                                                                            25.25, 26.26, 27.27, 28.28, 29.29, 30.30, 31.31,
                                                                                            32.32, 33.33, 34.34, 35.35, 36.36, 37.37, 38.38,
                                                                                            39.39, 40.40, 41.41, 42.42, 43.43, 44.44, 45.45,
                                                                                            46.46, 47.47, 48.48, 49.49, 50.50, 51.51);

            // Assert
            Assert.IsInstanceOf<StructuresStabilityPointCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);
            Assert.AreSame(hydraRingSection, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
        }

        [Test]
        [TestCase(423, null)]
        [TestCase(424, 107)]
        [TestCase(425, 113)]
        [TestCase(430, 115)]
        [TestCase(435, 117)]
        [TestCase(436, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup
            var input = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(111, new HydraRingSection(1, double.NaN, double.NaN),
                                                                                            Enumerable.Empty<HydraRingForelandPoint>(),
                                                                                            1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10,
                                                                                            11.11, 12.12, 13.13, 14.14, 15.15, 16.16, 17.17,
                                                                                            18.18, 19.19, 20.20, 21.21, 22.22, 23.23, 24.24,
                                                                                            25.25, 26.26, 27.27, 28.28, 29.29, 30.30, 31.31,
                                                                                            32.32, 33.33, 34.34, 35.35, 36.36, 37.37, 38.38,
                                                                                            39.39, 40.40, 41.41, 42.42, 43.43, 44.44, 45.45,
                                                                                            46.46, 47.47, 48.48, 49.49, 50.50, 51.51);

            // Call
            int? actualSubmechanismModelId = input.GetSubMechanismModelId(subMechanismModelId);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, actualSubmechanismModelId);
        }
    }
}