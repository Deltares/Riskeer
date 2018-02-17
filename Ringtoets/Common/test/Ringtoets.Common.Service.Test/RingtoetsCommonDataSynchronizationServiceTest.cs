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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataSynchronizationServiceTest
    {
        [Test]
        public void ClearHydraulicBoundaryLocationOutput_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        [Combinatorial]
        public void ClearHydraulicBoundaryLocationOutput_LocationWithOrWithoutOutput_ClearsAnyOutputAndReturnsExpectedAffectedObjects(
            [Values(true, false)] bool setOutputForDesignWaterLevelCalculation1,
            [Values(true, false)] bool setOutputForDesignWaterLevelCalculation2,
            [Values(true, false)] bool setOutputForDesignWaterLevelCalculation3,
            [Values(true, false)] bool setOutputForDesignWaterLevelCalculation4,
            [Values(true, false)] bool setOutputForWaveHeightCalculation1,
            [Values(true, false)] bool setOutputForWaveHeightCalculation2,
            [Values(true, false)] bool setOutputForWaveHeightCalculation3,
            [Values(true, false)] bool setOutputForWaveHeightCalculation4)
        {
            // Setup
            var random = new Random(32);

            HydraulicBoundaryLocation location = new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation1 =
                {
                    Output = setOutputForDesignWaterLevelCalculation1 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                DesignWaterLevelCalculation2 =
                {
                    Output = setOutputForDesignWaterLevelCalculation2 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                DesignWaterLevelCalculation3 =
                {
                    Output = setOutputForDesignWaterLevelCalculation3 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                DesignWaterLevelCalculation4 =
                {
                    Output = setOutputForDesignWaterLevelCalculation4 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                WaveHeightCalculation1 =
                {
                    Output = setOutputForWaveHeightCalculation1 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                WaveHeightCalculation2 =
                {
                    Output = setOutputForWaveHeightCalculation2 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                WaveHeightCalculation3 =
                {
                    Output = setOutputForWaveHeightCalculation3 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                },
                WaveHeightCalculation4 =
                {
                    Output = setOutputForWaveHeightCalculation4 ? new TestHydraulicBoundaryLocationOutput(random.NextDouble()) : null
                }
            };
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };
            bool hasOutput = setOutputForDesignWaterLevelCalculation1
                             || setOutputForDesignWaterLevelCalculation2
                             || setOutputForDesignWaterLevelCalculation3
                             || setOutputForDesignWaterLevelCalculation4
                             || setOutputForWaveHeightCalculation1
                             || setOutputForWaveHeightCalculation2
                             || setOutputForWaveHeightCalculation3
                             || setOutputForWaveHeightCalculation4;

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsFalse(location.DesignWaterLevelCalculation1.HasOutput);
            Assert.IsFalse(location.DesignWaterLevelCalculation2.HasOutput);
            Assert.IsFalse(location.DesignWaterLevelCalculation3.HasOutput);
            Assert.IsFalse(location.DesignWaterLevelCalculation4.HasOutput);
            Assert.IsFalse(location.WaveHeightCalculation1.HasOutput);
            Assert.IsFalse(location.WaveHeightCalculation2.HasOutput);
            Assert.IsFalse(location.WaveHeightCalculation3.HasOutput);
            Assert.IsFalse(location.WaveHeightCalculation4.HasOutput);

            HydraulicBoundaryLocation[] expectedAffectedObjects = hasOutput
                                                                      ? new[]
                                                                      {
                                                                          location
                                                                      }
                                                                      : new HydraulicBoundaryLocation[0];
            CollectionAssert.AreEqual(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationOutput_HydraulicBoundaryDatabaseWithoutLocations_ReturnsNoAffectedObjects()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> locations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
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
            var calculations = new[]
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