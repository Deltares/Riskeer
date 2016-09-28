﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresStabilityPointCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            var hydraRingSection = new HydraRingSection(1, double.NaN, double.NaN);
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();

            // Call
            var input = new TestStructuresStabilityPointCalculationInput(hydraulicBoundaryLocationId, hydraRingSection, forelandPoints);

            // Assert
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);
            Assert.AreSame(hydraRingSection, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
        }

        private class TestStructuresStabilityPointCalculationInput : StructuresStabilityPointCalculationInput
        {
            public TestStructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                                IEnumerable<HydraRingForelandPoint> forelandPoints)
                : base(hydraulicBoundaryLocationId, hydraRingSection, forelandPoints) {}

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                throw new NotImplementedException();
            }
        }
    }
}