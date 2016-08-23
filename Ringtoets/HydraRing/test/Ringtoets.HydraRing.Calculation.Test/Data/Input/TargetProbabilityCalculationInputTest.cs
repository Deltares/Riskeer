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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class TargetProbabilityCalculationInputTest
    {
        [Test]
        [TestCase(2, 10000)]
        [TestCase(-50, 1)]
        [TestCase(0, -90)]
        [TestCase(200000, double.NaN)]
        public void Constructed_UsingDifferentNormAndLocationId_ReturnDifferentBetaAndDefaultValues(int locationId, double norm)
        {
            // Call
            var targetProbabilityCalculationInputImplementation = new SimpleTargetProbabilityCalculationInput(locationId, norm);

            // Assert
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.AreEqual(locationId, targetProbabilityCalculationInputImplementation.HydraulicBoundaryLocationId);
            Assert.AreEqual(2, targetProbabilityCalculationInputImplementation.CalculationTypeId);
            CollectionAssert.IsEmpty(targetProbabilityCalculationInputImplementation.Variables);
            CollectionAssert.IsEmpty(targetProbabilityCalculationInputImplementation.ProfilePoints);
            CollectionAssert.IsEmpty(targetProbabilityCalculationInputImplementation.ForelandsPoints);
            Assert.IsNull(targetProbabilityCalculationInputImplementation.BreakWater);
            Assert.AreEqual(expectedBeta, targetProbabilityCalculationInputImplementation.Beta);
        }

        private class SimpleTargetProbabilityCalculationInput : TargetProbabilityCalculationInput
        {
            public SimpleTargetProbabilityCalculationInput(int i, double norm)
                : base(i, norm) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int VariableId
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override HydraRingSection Section
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}