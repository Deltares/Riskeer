// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;

namespace Ringtoets.DuneErosion.Data.TestUtil.Test
{
    [TestFixture]
    public class DuneLocationCalculationsTestHelperTest
    {
        [Test]
        public void GetAllDuneLocationCalculationsWithOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneLocationCalculationsTestHelper.GetAllDuneLocationCalculationsWithOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void GetAllDuneLocationCalculationsWithOutput_FailureMechanismWithoutDuneLocationCalculations_ReturnsEmpty()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            IEnumerable<IObservable> calculations =
                DuneLocationCalculationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(calculations);
        }

        [Test]
        public void GetAllDuneLocationCalculationsWithOutput_FailureMechanismWithDuneLocationCalculations_ReturnsCalculationsWithOutput()
        {
            // Setup
            var duneLocations = new[]
            {
                new TestDuneLocation
                {
                    Calculation =
                    {
                        Output = new TestDuneLocationCalculationOutput()
                    }
                },
                new TestDuneLocation()
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);

            DuneLocationCalculation calculation1 = failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.First();
            DuneLocationCalculation calculation2 = failureMechanism.CalculationsForMechanismSpecificSignalingNorm.First();
            DuneLocationCalculation calculation3 = failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.First();
            DuneLocationCalculation calculation4 = failureMechanism.CalculationsForLowerLimitNorm.First();
            DuneLocationCalculation calculation5 = failureMechanism.CalculationsForFactorizedLowerLimitNorm.First();

            calculation1.Output = new TestDuneLocationCalculationOutput();
            calculation2.Output = new TestDuneLocationCalculationOutput();
            calculation3.Output = new TestDuneLocationCalculationOutput();
            calculation4.Output = new TestDuneLocationCalculationOutput();
            calculation5.Output = new TestDuneLocationCalculationOutput();

            // Call
            IEnumerable<IObservable> actualObjects =
                DuneLocationCalculationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Assert
            var expectedObjects = new IObservable[]
            {
                duneLocations[0],
                calculation1,
                calculation2,
                calculation3,
                calculation4,
                calculation5
            };
            CollectionAssert.AreEquivalent(expectedObjects, actualObjects);
        }

        [Test]
        public void AssertDuneLocationCalculationsHaveNoOutputs_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneLocationCalculationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }
    }
}