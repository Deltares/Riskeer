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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Variables
{
    [TestFixture]
    public class RayleighNHydraRingVariableTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int variableId = 1;
            const HydraRingDeviationType deviationType = HydraRingDeviationType.Standard;
            const double n = 2.2;
            const double variance = 3.3;

            // Call
            var hydraRingVariable = new RayleighNHydraRingVariable(variableId, deviationType, n, variance);

            // Assert
            Assert.IsInstanceOf<RandomHydraRingVariable>(hydraRingVariable);
            Assert.AreEqual(HydraRingDistributionType.RayleighN, hydraRingVariable.DistributionType);
            Assert.AreEqual(deviationType, hydraRingVariable.DeviationType);
            Assert.AreEqual(variableId, hydraRingVariable.VariableId);
            Assert.AreEqual(variance, hydraRingVariable.Parameter1);
            Assert.AreEqual(n, hydraRingVariable.Parameter2);
        }
    }
}