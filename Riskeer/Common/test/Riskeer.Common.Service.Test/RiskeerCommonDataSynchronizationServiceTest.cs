// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using NSubstitute;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class RiskeerCommonDataSynchronizationServiceTest
    {
        [Test]
        public void ClearHydraulicBoundaryLocation_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocation_InputWithoutHydraulicBoundaryLocation_ReturnsNoAffectedObjects()
        {
            // Setup
            var input = Substitute.For<ICalculationInputWithHydraulicBoundaryLocation>();
            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(input);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocation_InputWithHydraulicBoundaryLocation_ClearsLocationAndReturnsAffectedInput()
        {
            // Setup
            var input = Substitute.For<ICalculationInputWithHydraulicBoundaryLocation>();
            input.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocation(input);

            // Assert
            Assert.IsNull(input.HydraulicBoundaryLocation);
            CollectionAssert.AreEqual(new[]
            {
                input
            }, affectedObjects);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationOutput_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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

            HydraulicBoundaryLocationCalculation[] calculations =
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput1,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput2,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(calculations);

            // Assert
            Assert.IsTrue(calculations.All(c => c.Output == null));
            CollectionAssert.AreEqual(new[]
            {
                calculationWithOutput1,
                calculationWithOutput2
            }, affectedCalculations);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationIllustrationPoints_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocationCalculationIllustrationPoints_CalculationsWithAndWithoutIllustrationPoints_ClearsIllustrationPointAndReturnsAffectedCalculations()
        {
            // Setup
            var random = new Random(21);

            var originalOutputWithIllustrationPoints1 = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                                                                           new TestGeneralResultSubMechanismIllustrationPoint());
            var calculationWithIllustrationPoints1 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = originalOutputWithIllustrationPoints1
            };

            var originalOutputWithIllustrationPoints2 = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                                                                           new TestGeneralResultSubMechanismIllustrationPoint());
            var calculationWithIllustrationPoints2 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = originalOutputWithIllustrationPoints2
            };

            var originalOutput1 = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            var calculationWithOutput1 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = originalOutput1
            };

            var originalOutput2 = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            var calculationWithOutput2 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = originalOutput2
            };

            HydraulicBoundaryLocationCalculation[] calculations =
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput1,
                calculationWithIllustrationPoints1,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                calculationWithOutput2,
                calculationWithIllustrationPoints2,
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            };

            // Call
            IEnumerable<IObservable> affectedCalculations = RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(calculations);

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithIllustrationPoints1,
                calculationWithIllustrationPoints2
            }, affectedCalculations);

            HydraulicBoundaryLocationCalculation[] calculationWithOutputs =
            {
                calculationWithOutput1,
                calculationWithIllustrationPoints1,
                calculationWithOutput2,
                calculationWithIllustrationPoints2
            };
            Assert.IsTrue(calculationWithOutputs.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationWithOutputs.All(calc => !calc.Output.HasGeneralResult));

            AssertHydraulicBoundaryLocationOutput(originalOutput1, calculationWithOutput1.Output);
            AssertHydraulicBoundaryLocationOutput(originalOutput2, calculationWithOutput2.Output);

            AssertHydraulicBoundaryLocationOutput(originalOutputWithIllustrationPoints1, calculationWithIllustrationPoints1.Output);
            AssertHydraulicBoundaryLocationOutput(originalOutputWithIllustrationPoints2, calculationWithIllustrationPoints2.Output);
        }

        [Test]
        public void ClearStructuresCalculationIllustrationPoints_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerCommonDataSynchronizationService.ClearStructuresCalculationIllustrationPoints(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void ClearStructuresCalculationIllustrationPoints_CalculationsWithAndWithoutIllustrationPoints_ClearsIllustrationPointAndReturnsAffectedCalculations()
        {
            // Setup
            var originalOutputWithIllustrationPoints1 = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint());
            var calculationWithIllustrationPoints1 = new TestStructuresCalculation
            {
                Output = originalOutputWithIllustrationPoints1
            };

            var originalOutputWithIllustrationPoints2 = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint());
            var calculationWithIllustrationPoints2 = new TestStructuresCalculation
            {
                Output = originalOutputWithIllustrationPoints2
            };

            var originalOutput1 = new TestStructuresOutput();
            var calculationWithOutput1 = new TestStructuresCalculation
            {
                Output = originalOutput1
            };

            var originalOutput2 = new TestStructuresOutput();
            var calculationWithOutput2 = new TestStructuresCalculation
            {
                Output = originalOutput2
            };

            TestStructuresCalculation[] calculations =
            {
                new TestStructuresCalculation(),
                calculationWithOutput1,
                calculationWithIllustrationPoints1,
                new TestStructuresCalculation(),
                calculationWithOutput2,
                calculationWithIllustrationPoints2,
                new TestStructuresCalculation()
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerCommonDataSynchronizationService.ClearStructuresCalculationIllustrationPoints(calculations);

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithIllustrationPoints1,
                calculationWithIllustrationPoints2
            }, affectedObjects);

            TestStructuresCalculation[] calculationsWithOutput =
            {
                calculationWithOutput1,
                calculationWithIllustrationPoints1,
                calculationWithOutput2,
                calculationWithIllustrationPoints2
            };

            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
            Assert.IsTrue(calculationsWithOutput.All(calc => !calc.Output.HasGeneralResult));

            AssertStructuresOutput(originalOutput1, calculationWithOutput1.Output);
            AssertStructuresOutput(originalOutput2, calculationWithOutput2.Output);

            AssertStructuresOutput(originalOutputWithIllustrationPoints1, calculationWithIllustrationPoints1.Output);
            AssertStructuresOutput(originalOutputWithIllustrationPoints2, calculationWithIllustrationPoints2.Output);
        }

        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerCommonDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var calculation = Substitute.For<ICalculation>();
            calculation.HasOutput.Returns(true);
            // Call
            IEnumerable<IObservable> changedObjects = RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            calculation.Received().ClearOutput();
            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
            
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = Substitute.For<ICalculation>();
            calculation.HasOutput.Returns(false);
            // Call
            IEnumerable<IObservable> changedObjects = RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        [Test]
        public void ClearForeshoreProfile_CalculationsWithForeshoreProfile_ClearForeshoreProfileAndReturnAffectedInputs()
        {
            // Setup
            var foreshoreProfileToBeRemoved = new TestForeshoreProfile(new Point2D(0, 0));
            var foreshoreProfile = new TestForeshoreProfile(new Point2D(1, 1));

            var calculation1 = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            var calculation2 = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileToBeRemoved
                }
            };
            var calculation3 = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileToBeRemoved
                },
                Output = new TestStructuresOutput()
            };
            StructuresCalculation<TestStructuresInput>[] calculations =
            {
                calculation1,
                calculation2,
                calculation3
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RiskeerCommonDataSynchronizationService.ClearForeshoreProfile<TestStructuresInput, TestStructure>(
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

        private static void AssertHydraulicBoundaryLocationOutput(HydraulicBoundaryLocationCalculationOutput expectedOutput,
                                                                  HydraulicBoundaryLocationCalculationOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.Result, actualOutput.Result);
            Assert.AreEqual(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability);
            Assert.AreEqual(expectedOutput.TargetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(expectedOutput.TargetReliability, actualOutput.TargetReliability);
            Assert.AreEqual(expectedOutput.CalculationConvergence, actualOutput.CalculationConvergence);
        }

        private static void AssertStructuresOutput(StructuresOutput expectedOutput, StructuresOutput actualOutput)
        {
            Assert.AreEqual(expectedOutput.Reliability, actualOutput.Reliability);
        }
    }
}