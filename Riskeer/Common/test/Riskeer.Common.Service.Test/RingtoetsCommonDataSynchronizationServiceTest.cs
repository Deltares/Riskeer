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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataSynchronizationServiceTest
    {
        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_CalculationsWithAndWithoutOutput_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var random = new Random(21);

            var calculationWithOutput1 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };

            var calculationWithOutput2 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput1,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput2,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculations);

            // Assert
            Assert.IsTrue(calculations.All(c => c.Output == null));
            CollectionAssert.AreEqual(new[]
            {
                calculationWithOutput1,
                calculationWithOutput2
            }, affectedCalculations);
        }

        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation>();
            calculation.Expect(c => c.HasOutput).Return(true);
            calculation.Expect(c => c.ClearOutput());
            mocks.ReplayAll();

            // Call
            IEnumerable<IObservable> changedObjects = RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            mocks.VerifyAll();

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation>();
            calculation.Expect(c => c.HasOutput).Return(false);
            mocks.ReplayAll();

            // Call
            IEnumerable<IObservable> changedObjects = RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);

            mocks.VerifyAll();
        }

        [Test]
        public void ClearForeshoreProfile_CalculationsWithForeshoreProfile_ClearForeshoreProfileAndReturnAffectedInputs()
        {
            // Setup
            var foreshoreProfileToBeRemoved = new TestForeshoreProfile(new Point2D(0, 0));
            var foreshoreProfile = new TestForeshoreProfile(new Point2D(1, 1));

            var calculation1 = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            var calculation2 = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileToBeRemoved
                }
            };
            var calculation3 = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileToBeRemoved
                },
                Output = new TestStructuresOutput()
            };
            StructuresCalculation<TestStructureInput>[] calculations =
            {
                calculation1,
                calculation2,
                calculation3
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearForeshoreProfile<TestStructureInput, TestStructure>(
                calculations, foreshoreProfileToBeRemoved);

            // Assert
            Assert.IsNull(calculation2.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation3.InputParameters.ForeshoreProfile);
            Assert.IsFalse(calculation3.HasOutput);
            Assert.IsNotNull(calculation1.InputParameters.ForeshoreProfile);

            CollectionAssert.AreEqual(new IObservable[]
            {
                calculation2.InputParameters,
                calculation3,
                calculation3.InputParameters
            }, affectedObjects);
        }

        private class TestStructureInput : StructuresInputBase<TestStructure>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }
    }
}