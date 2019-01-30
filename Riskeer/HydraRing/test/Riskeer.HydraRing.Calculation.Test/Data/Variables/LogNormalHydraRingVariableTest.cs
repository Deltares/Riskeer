// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Test.Data.Variables
{
    [TestFixture]
    public class LogNormalHydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int variableId = 1;
            const HydraRingDeviationType deviationType = HydraRingDeviationType.Standard;
            const double mean = 2.2;
            const double variance = 3.3;
            const double shift = 4.4;

            // Call
            var hydraRingVariable = new LogNormalHydraRingVariable(variableId, deviationType, mean, variance, shift);

            // Assert
            Assert.IsInstanceOf<RandomHydraRingVariable>(hydraRingVariable);
            Assert.AreEqual(HydraRingDistributionType.LogNormal, hydraRingVariable.DistributionType);
            Assert.AreEqual(deviationType, hydraRingVariable.DeviationType);
            Assert.AreEqual(variableId, hydraRingVariable.VariableId);
            Assert.AreEqual(mean, hydraRingVariable.Parameter1);
            Assert.AreEqual(variance, hydraRingVariable.Parameter2);
            Assert.AreEqual(shift, hydraRingVariable.Parameter3);
        }

        [Test]
        public void Constructor_ShiftNotSet_Parameter3NaN()
        {
            // Call
            var hydraRingVariable = new LogNormalHydraRingVariable(1, HydraRingDeviationType.Standard, 2.2, 3.3);

            // Assert
            Assert.IsNaN(hydraRingVariable.Parameter3);
        }
    }
}