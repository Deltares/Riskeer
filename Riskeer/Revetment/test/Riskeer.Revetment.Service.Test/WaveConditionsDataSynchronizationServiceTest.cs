// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                null, NormType.Signaling);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_InvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                new TestFailureMechanism(), normType);

            // Assert
            var expectedMessage = $"The value of argument 'normType' ({normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        [TestCase(NormType.LowerLimit)]
        [TestCase(NormType.Signaling)]
        public void ClearAllWaveConditionsCalculationOutputWithNormType_WithAllData_ClearsOutputAndReturnsAffectedObjects(NormType normType)
        {
            // Setup
            var calculation1 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.Signaling
            }, true);
            var calculation2 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
            }, true);
            var calculation3 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.None
            }, true);
            var calculation4 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
            }, true);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4
            });
            mocks.ReplayAll();

            TestWaveConditionsCalculation<WaveConditionsInput>[] expectedAffectedCalculations =
            {
                normType == NormType.LowerLimit
                    ? calculation2
                    : calculation1
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, TestWaveConditionsCalculation<WaveConditionsInput>>(
                failureMechanism, normType);

            // Assert
            CollectionAssert.AreEqual(expectedAffectedCalculations, affectedCalculations);
            Assert.IsTrue(expectedAffectedCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(failureMechanism.Calculations.Except(expectedAffectedCalculations).All(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAllWaveConditionsCalculationsOutputWithUserDefinedTargetProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                null, new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationsOutputWithUserDefinedTargetProbability_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                new TestFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationsOutputWithUserDefinedTargetProbability_WithAllData_ClearsOutputAndReturnsAffectedObjects()
        {
            // Setup
            var calculationsForTargetProbabilityToClear = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            var otherCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            var calculation1 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.Signaling,
                CalculationsTargetProbability = calculationsForTargetProbabilityToClear
            }, true);
            var calculation2 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                CalculationsTargetProbability = calculationsForTargetProbabilityToClear
            }, true);
            var calculation3 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.None,
                CalculationsTargetProbability = calculationsForTargetProbabilityToClear
            }, true);
            var calculation4 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                CalculationsTargetProbability = calculationsForTargetProbabilityToClear
            }, true);
            var calculation5 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                CalculationsTargetProbability = otherCalculationsForTargetProbability
            }, true);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4,
                calculation5
            });
            mocks.ReplayAll();

            TestWaveConditionsCalculation<WaveConditionsInput>[] expectedAffectedCalculations =
            {
                calculation4
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = WaveConditionsDataSynchronizationService.ClearAllWaveConditionsCalculationOutput<IFailureMechanism, TestWaveConditionsCalculation<WaveConditionsInput>>(
                failureMechanism, calculationsForTargetProbabilityToClear);

            // Assert
            CollectionAssert.AreEqual(expectedAffectedCalculations, affectedCalculations);
            Assert.IsTrue(expectedAffectedCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(failureMechanism.Calculations.Except(expectedAffectedCalculations).All(c => c.HasOutput));
            mocks.VerifyAll();
        }

        [Test]
        public void ClearWaveConditionsCalculationOutputAndRemoveTargetProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearWaveConditionsCalculationOutputAndRemoveTargetProbability<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                null, new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculationOutputAndRemoveTargetProbability_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsDataSynchronizationService.ClearWaveConditionsCalculationOutputAndRemoveTargetProbability<IFailureMechanism, ICalculation<WaveConditionsInput>>(
                new TestFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculationOutputAndRemoveTargetProbability_WithAllData_ClearsOutputAndRemovesTargetProbabilityAndReturnsAffectedObjects()
        {
            // Setup
            var calculationsForTargetProbabilityToClear = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            var otherCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            var calculation1 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.Signaling
            }, true);
            var calculation2 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
            }, true);
            var calculation3 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.None
            }, true);
            var calculation4 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                CalculationsTargetProbability = calculationsForTargetProbabilityToClear
            }, true);
            var calculation5 = new TestWaveConditionsCalculation<WaveConditionsInput>(new TestWaveConditionsInput
            {
                WaterLevelType = WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                CalculationsTargetProbability = otherCalculationsForTargetProbability
            }, true);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4,
                calculation5
            });
            mocks.ReplayAll();

            TestWaveConditionsCalculation<WaveConditionsInput>[] expectedAffectedCalculations =
            {
                calculation4
            };
            
            // Call
            IEnumerable<IObservable> affectedCalculations = WaveConditionsDataSynchronizationService.ClearWaveConditionsCalculationOutputAndRemoveTargetProbability<IFailureMechanism, TestWaveConditionsCalculation<WaveConditionsInput>>(
                failureMechanism, calculationsForTargetProbabilityToClear);

            // Assert
            CollectionAssert.AreEqual(expectedAffectedCalculations, affectedCalculations);
            Assert.IsTrue(expectedAffectedCalculations.All(
                              c => !c.HasOutput
                                   && c.InputParameters.WaterLevelType == WaveConditionsInputWaterLevelType.None));
            Assert.IsTrue(expectedAffectedCalculations.All(c => c.InputParameters.CalculationsTargetProbability == null));
            
            Assert.IsTrue(failureMechanism.Calculations.Except(expectedAffectedCalculations).All(c => c.HasOutput));
            Assert.IsNotNull(calculation5.InputParameters.CalculationsTargetProbability);
            mocks.VerifyAll();
        }
    }
}