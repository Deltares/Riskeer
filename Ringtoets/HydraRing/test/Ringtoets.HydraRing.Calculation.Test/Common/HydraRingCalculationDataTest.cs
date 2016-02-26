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
using Ringtoets.HydraRing.Calculation.Common;
using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Test.Common
{
    [TestFixture]
    public class HydraRingCalculationDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingCalculationData = new HydraRingCalculationDataImplementation(1);

            // Assert
            Assert.AreEqual(1, hydraRingCalculationData.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, hydraRingCalculationData.FailureMechanismType);
            Assert.IsTrue(!hydraRingCalculationData.Variables.Any());
            Assert.IsNaN(hydraRingCalculationData.Beta);
        }

        private class HydraRingCalculationDataImplementation : HydraRingCalculationData
        {
            public HydraRingCalculationDataImplementation(int hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.QVariant;
                }
            }
        }
    }
}