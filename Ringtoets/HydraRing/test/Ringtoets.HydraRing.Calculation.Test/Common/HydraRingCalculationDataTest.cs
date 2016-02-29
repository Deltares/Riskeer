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
            CollectionAssert.IsEmpty(hydraRingCalculationData.Variables);
            CollectionAssert.IsEmpty(hydraRingCalculationData.ProfilePoints);
            Assert.IsNaN(hydraRingCalculationData.Beta);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var hydraRingCalculationData = new HydraRingCalculationDataImplementation(1);

            // Assert
            Assert.AreEqual(10, hydraRingCalculationData.GetSubMechanismModelId(1));
            Assert.AreEqual(20, hydraRingCalculationData.GetSubMechanismModelId(2));
            Assert.IsNull(hydraRingCalculationData.GetSubMechanismModelId(3));
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

            public override HydraRingDikeSection DikeSection
            {
                get
                {
                    return new HydraRingDikeSection(1, "Name", 2.2, 3.3, 4.4, 5.5, 6.6, 7.7);
                }
            }

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                switch (subMechanismId)
                {
                    case 1:
                        return 10;
                    case 2:
                        return 20;
                    default:
                        return null;
                }
            }
        }
    }
}