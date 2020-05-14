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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;

namespace Riskeer.DuneErosion.Data.TestUtil.Test
{
    [TestFixture]
    public class DuneLocationsTestHelperTest
    {
        [Test]
        public void GetAllDuneLocationCalculationsWithOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(null);

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
            IEnumerable<IObservable> calculations = DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(calculations);
        }

        [Test]
        public void GetAllDuneLocationCalculationsWithOutput_FailureMechanismWithDuneLocationCalculations_ReturnsCalculationsWithOutput()
        {
            // Setup
            var duneLocations = new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.AddDuneLocations(duneLocations);

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
            IEnumerable<IObservable> actualObjects = DuneLocationsTestHelper.GetAllDuneLocationCalculationsWithOutput(failureMechanism);

            // Assert
            var expectedObjects = new IObservable[]
            {
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
            TestDelegate call = () => DuneLocationsTestHelper.AssertDuneLocationCalculationsHaveNoOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetDuneLocationCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DuneLocationsTestHelper.SetDuneLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void SetDuneLocationCalculationOutput_FailureMechanismWithDuneLocations_SetsAllDuneLocationCalculationsWithOutput()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            failureMechanism.AddDuneLocations(new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            });

            // Precondition
            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(calc => !HasDuneLocationCalculationOutput(calc)));

            // Call
            DuneLocationsTestHelper.SetDuneLocationCalculationOutput(failureMechanism);

            // Assert
            Assert.True(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.All(HasDuneLocationCalculationOutput));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.All(HasDuneLocationCalculationOutput));
            Assert.True(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.All(HasDuneLocationCalculationOutput));
            Assert.True(failureMechanism.CalculationsForLowerLimitNorm.All(HasDuneLocationCalculationOutput));
            Assert.True(failureMechanism.CalculationsForFactorizedLowerLimitNorm.All(HasDuneLocationCalculationOutput));
        }

        private static bool HasDuneLocationCalculationOutput(DuneLocationCalculation calculation)
        {
            return calculation.Output != null;
        }
    }
}