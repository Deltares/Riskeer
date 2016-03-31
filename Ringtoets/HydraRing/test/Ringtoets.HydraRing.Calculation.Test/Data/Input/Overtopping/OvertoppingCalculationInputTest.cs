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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Overtopping
{
    [TestFixture]
    public class OvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 1;
            int hydraulicBoundaryLocationId = 1000;
            HydraRingDikeSection expectedDikeSection = new HydraRingDikeSection(expectedVariableId, "1000", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var expectedRingProfilePoints = new List<HydraRingProfilePoint>
            {
                new HydraRingProfilePoint(1.1, 2.2)
            };
            var expectedRingForelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(2.2, 3.3)
            };

            // Call
            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(hydraulicBoundaryLocationId, expectedDikeSection, expectedRingProfilePoints, expectedRingForelandPoints);

            // Assert
            Assert.AreEqual(expectedCalculationTypeId, overtoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, overtoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, overtoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, overtoppingCalculationInput.VariableId);
            Assert.IsNotNull(overtoppingCalculationInput.DikeSection);
            CollectionAssert.IsEmpty(overtoppingCalculationInput.Variables);
            CollectionAssert.AreEqual(expectedRingProfilePoints, overtoppingCalculationInput.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, overtoppingCalculationInput.ForelandsPoints);
            CollectionAssert.IsEmpty(overtoppingCalculationInput.BreakWaters);
            Assert.IsNaN(overtoppingCalculationInput.Beta);

            var dikeSection = overtoppingCalculationInput.DikeSection;
            Assert.AreEqual(expectedDikeSection, dikeSection);
        }

        [Test]
        [TestCase(101, null)]
        [TestCase(102, 94)]
        [TestCase(103, 95)]
        [TestCase(104, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup 
            HydraRingDikeSection expectedDikeSection = new HydraRingDikeSection(1, "1000", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(1, expectedDikeSection, new List<HydraRingProfilePoint>(), new List<HydraRingForelandPoint>());

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, overtoppingCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }
    }
}