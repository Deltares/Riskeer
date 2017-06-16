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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
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
        public void ClearHydraulicBoundaryLocationOutput_LocationWithData_ClearsDataAndReturnsAffectedObjects(
            [Values(3.4, double.NaN)] double designWaterLevel,
            [Values(5.3, double.NaN)] double waveHeight)
        {
            // Setup
            HydraulicBoundaryLocation location = new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(designWaterLevel)
                },
                WaveHeightCalculation =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(waveHeight)
                }
            };
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(locations);

            // Assert
            Assert.IsFalse(location.DesignWaterLevelCalculation.HasOutput);
            Assert.IsFalse(location.WaveHeightCalculation.HasOutput);
            Assert.IsNaN(location.DesignWaterLevel);
            Assert.IsNaN(location.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
            Assert.AreEqual(CalculationConvergence.NotCalculated, location.WaveHeightCalculationConvergence);

            CollectionAssert.AreEqual(new[]
            {
                location
            }, affectedObjects);
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
        public void ClearHydraulicBoundaryLocationOutput_LocationWithoutWaveHeightAndDesignWaterLevel_ReturnsNoAffectedObjects()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                new TestHydraulicBoundaryLocation()
            };

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
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
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

        [Test]
        public void RemoveStructure_WithoutStructure_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.RemoveStructure(
                null,
                Enumerable.Empty<StructuresCalculation<TestStructureInput>>(),
                new StructureCollection<TestStructure>(),
                Enumerable.Empty<TestSectionResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structure", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_WithoutCalculations_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.RemoveStructure(
                new TestStructure(new Point2D(0, 0)),
                null,
                new StructureCollection<TestStructure>(),
                Enumerable.Empty<TestSectionResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_WithoutStructures_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.RemoveStructure(
                new TestStructure(new Point2D(0, 0)),
                Enumerable.Empty<StructuresCalculation<TestStructureInput>>(),
                null,
                Enumerable.Empty<TestSectionResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_WithoutSectionResults_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsCommonDataSynchronizationService.RemoveStructure(
                new TestStructure(new Point2D(0, 0)),
                Enumerable.Empty<StructuresCalculation<TestStructureInput>>(),
                new StructureCollection<TestStructure>(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            var locationStructureToRemove = new Point2D(0, 0);
            var locationStructureToKeep = new Point2D(5, 5);

            var structureToRemove = new TestStructure("ToRemove", locationStructureToRemove);
            var structureToKeep = new TestStructure("ToKeep", locationStructureToKeep);

            var structures = new StructureCollection<TestStructure>();
            structures.AddRange(new[]
            {
                structureToRemove,
                structureToKeep
            }, "path/to/structures");
            var calculationWithOutput = new StructuresCalculation<TestStructureInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithStructureToRemove = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureToRemove
                }
            };
            var calculationWithStructureToKeepAndOutput = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureToKeep
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithStructureToRemoveAndOutput = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureToRemove
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            StructuresCalculation<TestStructureInput>[] calculations =
            {
                calculationWithOutput,
                calculationWithStructureToRemove,
                calculationWithStructureToKeepAndOutput,
                calculationWithStructureToRemoveAndOutput
            };
            var sectionWithCalculationAtStructureToRemove = new TestSectionResult(locationStructureToRemove)
            {
                Calculation = calculationWithStructureToRemove
            };
            var sectionWithCalculationAtStructureToKeep = new TestSectionResult(locationStructureToKeep)
            {
                Calculation = calculationWithStructureToKeepAndOutput
            };
            StructuresFailureMechanismSectionResult<TestStructureInput>[] sectionResults =
            {
                sectionWithCalculationAtStructureToRemove,
                sectionWithCalculationAtStructureToKeep
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.RemoveStructure(
                structureToRemove,
                calculations,
                structures,
                sectionResults);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(structures, structureToRemove);
            Assert.IsNull(calculationWithStructureToRemove.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureToRemoveAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureToRemoveAndOutput.Output);
            Assert.IsNull(sectionWithCalculationAtStructureToRemove.Calculation);
            Assert.IsNotNull(calculationWithOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.InputParameters.Structure);
            Assert.AreSame(sectionWithCalculationAtStructureToKeep.Calculation, calculationWithStructureToKeepAndOutput);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureToRemove.InputParameters,
                calculationWithStructureToRemoveAndOutput,
                calculationWithStructureToRemoveAndOutput.InputParameters,
                sectionWithCalculationAtStructureToRemove,
                structures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveAllStructures_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                null,
                new StructureCollection<TestStructure>(),
                Enumerable.Empty<StructuresFailureMechanismSectionResult<TestStructureInput>>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void RemoveAllStructures_StructuresNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures<TestStructure, TestStructureInput>(
                Enumerable.Empty<StructuresCalculation<TestStructureInput>>(),
                null,
                Enumerable.Empty<StructuresFailureMechanismSectionResult<TestStructureInput>>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structures", paramName);
        }

        [Test]
        public void RemoveAllStructures_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                Enumerable.Empty<StructuresCalculation<TestStructureInput>>(),
                new StructureCollection<TestStructure>(),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllHeightStructuresAndClearDependentData()
        {
            // Setup
            var locationStructureA = new Point2D(0, 0);
            var locationStructureB = new Point2D(5, 5);

            var structureA = new TestStructure("A", locationStructureA);
            var structureB = new TestStructure("B", locationStructureB);

            var structures = new StructureCollection<TestStructure>();
            structures.AddRange(new[]
            {
                structureA,
                structureB
            }, "path/to/structures");
            var calculationWithOutput = new StructuresCalculation<TestStructureInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithStructureA = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureA
                }
            };
            var calculationWithStructureBAndOutput = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureB
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithStructureAAndOutput = new StructuresCalculation<TestStructureInput>
            {
                InputParameters =
                {
                    Structure = structureA
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            StructuresCalculation<TestStructureInput>[] calculations =
            {
                calculationWithOutput,
                calculationWithStructureA,
                calculationWithStructureBAndOutput,
                calculationWithStructureAAndOutput
            };
            var sectionWithCalculationAtStructureA = new TestSectionResult(locationStructureA)
            {
                Calculation = calculationWithStructureA
            };
            var sectionWithCalculationAtStructureB = new TestSectionResult(locationStructureB)
            {
                Calculation = calculationWithStructureBAndOutput
            };
            StructuresFailureMechanismSectionResult<TestStructureInput>[] sectionResults =
            {
                sectionWithCalculationAtStructureA,
                sectionWithCalculationAtStructureB
            };

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                calculations,
                structures,
                sectionResults);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(structures, structureA);
            Assert.IsNull(calculationWithStructureA.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureAAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureBAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureAAndOutput.Output);
            Assert.IsNull(calculationWithStructureBAndOutput.Output);
            Assert.IsNull(sectionWithCalculationAtStructureA.Calculation);
            Assert.IsNull(sectionWithCalculationAtStructureB.Calculation);
            Assert.IsNotNull(calculationWithOutput.Output);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureA.InputParameters,
                calculationWithStructureAAndOutput,
                calculationWithStructureAAndOutput.InputParameters,
                calculationWithStructureBAndOutput,
                calculationWithStructureBAndOutput.InputParameters,
                sectionWithCalculationAtStructureA,
                sectionWithCalculationAtStructureB,
                structures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private class TestSectionResult : StructuresFailureMechanismSectionResult<TestStructureInput>
        {
            public TestSectionResult(Point2D point2D) : base(new FailureMechanismSection($"Location {point2D}", new[]
            {
                point2D,
                point2D
            })) {}
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