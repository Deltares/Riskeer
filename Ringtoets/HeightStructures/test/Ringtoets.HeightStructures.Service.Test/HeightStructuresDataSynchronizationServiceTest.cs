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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithOutput_ClearsCalculationsOutput()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
                Assert.IsNull(calculation.Output);
            }

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation1,
                calculation1.InputParameters,
                calculation2,
                calculation2.InputParameters
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationNoOutput_ClearsHydraulicBoundaryLocationAndReturnsAffectedInputs()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            }

            CollectionAssert.AreEqual(new[]
            {
                calculation1.InputParameters,
                calculation2.InputParameters
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithOutputAndNoHydraulicBoundaryLocation_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationWithoutOutputAndHydraulicBoundaryLocation_ReturnNoAffectedCalculations()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<HeightStructuresInput>();
            var calculation2 = new StructuresCalculation<HeightStructuresInput>();
            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .Concat(failureMechanism.HeightStructures)
                                                              .ToArray();

            // Call
            ClearResults results = HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(array, failureMechanism.HeightStructures);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        [Test]
        public void RemoveHeightStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            HeightStructure structure = failureMechanism.HeightStructures[0];
            StructuresCalculation<HeightStructuresInput>[] calculationsWithStructure = failureMechanism.Calculations
                                                                                                       .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                                                                       .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                                                                                                       .ToArray();
            HeightStructuresFailureMechanismSectionResult[] sectionResultsWithStructure = failureMechanism.SectionResults
                                                                                                          .Where(sr => calculationsWithStructure.Contains(sr.Calculation))
                                                                                                          .ToArray();
            StructuresCalculation<HeightStructuresInput>[] calculationsWithOutput = calculationsWithStructure.Where(c => c.HasOutput)
                                                                                                             .ToArray();

            int originalNumberOfSectionResultAssignments = failureMechanism.SectionResults.Count(sr => sr.Calculation != null);
            HeightStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults
                                                                                             .Where(sr => calculationsWithStructure.Contains(sr.Calculation))
                                                                                             .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithOutput);
            CollectionAssert.IsNotEmpty(calculationsWithStructure);
            CollectionAssert.IsNotEmpty(sectionResultsWithStructure);

            // Call
            IEnumerable<IObservable> affectedObjects = RingtoetsCommonDataSynchronizationService.RemoveStructure(
                structure,
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(),
                failureMechanism.HeightStructures,
                failureMechanism.SectionResults);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, structure);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithStructure)
            {
                Assert.IsNull(calculation.InputParameters.Structure);
            }
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }
            foreach (HeightStructuresFailureMechanismSectionResult sectionResult in sectionResultsWithStructure)
            {
                Assert.IsNull(sectionResult.Calculation);
            }

            IObservable[] array = affectedObjects.ToArray();
            Assert.AreEqual(1 + calculationsWithOutput.Length + calculationsWithStructure.Length + sectionResultsWithStructure.Length,
                            array.Length);
            CollectionAssert.Contains(array, failureMechanism.HeightStructures);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithStructure)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithOutput)
            {
                CollectionAssert.Contains(array, calculation);
            }
            foreach (HeightStructuresFailureMechanismSectionResult result in sectionResultsWithStructure)
            {
                CollectionAssert.Contains(array, result);
            }
            Assert.AreEqual(originalNumberOfSectionResultAssignments - sectionResults.Length, failureMechanism.SectionResults.Count(sr => sr.Calculation != null),
                            "Other section results with a different calculation/structure should still have their association.");
        }

        [Test]
        public void RemoveAllStructures_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                null,
                new StructureCollection<HeightStructure>(),
                Enumerable.Empty<StructuresFailureMechanismSectionResult<HeightStructuresInput>>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void RemoveAllStructures_StructuresNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures<HeightStructure, HeightStructuresInput>(
                Enumerable.Empty<StructuresCalculation<HeightStructuresInput>>(),
                null,
                Enumerable.Empty<StructuresFailureMechanismSectionResult<HeightStructuresInput>>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structures", paramName);
        }

        [Test]
        public void RemoveAllStructures_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                Enumerable.Empty<StructuresCalculation<HeightStructuresInput>>(),
                new StructureCollection<HeightStructure>(),
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllHeightStructuresAndClearDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            StructuresCalculation<HeightStructuresInput>[] calculationsWithStructure = failureMechanism.Calculations
                                                                                                       .Cast<StructuresCalculation<HeightStructuresInput>>()
                                                                                                       .Where(calc => calc.InputParameters.Structure != null)
                                                                                                       .ToArray();
            StructuresCalculation<HeightStructuresInput>[] calculationsWithOutput = calculationsWithStructure.Where(c => c.HasOutput)
                                                                                                             .ToArray();

            HeightStructuresFailureMechanismSectionResult[] sectionResultsWithStructure = failureMechanism.SectionResults
                                                                                                          .Where(sr => calculationsWithStructure.Contains(sr.Calculation))
                                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithStructure);

            // Call
            IEnumerable<IObservable> observables = RingtoetsCommonDataSynchronizationService.RemoveAllStructures(
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>(),
                failureMechanism.HeightStructures,
                failureMechanism.SectionResults);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);
            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithStructure)
            {
                Assert.IsNull(calculation.InputParameters.Structure);
            }

            foreach (StructuresCalculation<HeightStructuresInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithStructure.Select(calc => calc.InputParameters)
                                         .Cast<IObservable>()
                                         .Concat(calculationsWithOutput)
                                         .Concat(sectionResultsWithStructure)
                                         .Concat(new IObservable[]
                                         {
                                             failureMechanism.HeightStructures
                                         });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, observables);
        }

        private HeightStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(-1, 0),
                new Point2D(2, 0)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(2, 0),
                new Point2D(4, 0)
            });
            var structure1 = new TestHeightStructure(new Point2D(1, 0), "Id 1,0");
            var structure2 = new TestHeightStructure(new Point2D(3, 0), "Id 3,0");
            var profile = new TestForeshoreProfile();
            StructuresCalculation<HeightStructuresInput> calculation1 = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            StructuresCalculation<HeightStructuresInput> calculation2 = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure2
                }
            };
            StructuresCalculation<HeightStructuresInput> calculation3 = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation1,
                        new CalculationGroup("B", true)
                        {
                            Children =
                            {
                                calculation2
                            }
                        },
                        calculation3
                    }
                }
            };
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, "path");

            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "someLocation");

            failureMechanism.AddSection(section1);
            failureMechanism.AddSection(section2);
            HeightStructuresFailureMechanismSectionResult result1 = failureMechanism.SectionResults
                                                                                    .First(sr => ReferenceEquals(sr.Section, section1));
            HeightStructuresFailureMechanismSectionResult result2 = failureMechanism.SectionResults
                                                                                    .First(sr => ReferenceEquals(sr.Section, section2));
            result1.Calculation = calculation1;
            result2.Calculation = calculation2;

            return failureMechanism;
        }
    }
}