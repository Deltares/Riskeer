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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.Test.Common
{
    [TestFixture]
    public class HydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingVariable = new HydraRingVariableImplementation(1, HydraRingDistributionType.LogNormal, 2.2, HydraRingDeviationType.Variation, 3.3, 4.4, 5.5);

            // Assert
            Assert.AreEqual(1, hydraRingVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.LogNormal, hydraRingVariable.DistributionType);
            Assert.AreEqual(2.2, hydraRingVariable.Value);
            Assert.AreEqual(HydraRingDeviationType.Variation, hydraRingVariable.DeviationType);
            Assert.AreEqual(3.3, hydraRingVariable.Mean);
            Assert.AreEqual(4.4, hydraRingVariable.Variability);
            Assert.AreEqual(5.5, hydraRingVariable.Shift);
        }

        private class HydraRingVariableImplementation : HydraRingVariable
        {
            public HydraRingVariableImplementation(int variableId, HydraRingDistributionType distributionType, double value, HydraRingDeviationType deviationType, double mean, double variability, double shift)
                : base(variableId, distributionType, value, deviationType, mean, variability, shift) { }
        }
    }
}
