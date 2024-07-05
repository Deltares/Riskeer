﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Service.Test
{
    [TestFixture]
    public class PipingDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = true
                    },
                    Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
                },
                new SemiProbabilisticPipingCalculationScenario(),
                new ProbabilisticPipingCalculationScenario(),
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                }
            });

            SemiProbabilisticPipingCalculationScenario[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                                                        .OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                                        .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput)
                                                                                                        .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearAllSemiProbabilisticCalculationOutputWithoutManualAssessmentLevel(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .OfType<SemiProbabilisticPipingCalculationScenario>()
                                          .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                          .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllProbabilisticCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearAllProbabilisticCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllProbabilisticCalculationOutput_WithVariousCalculations_ClearsProbabilisticCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                },
                new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                },
                new ProbabilisticPipingCalculationScenario()
            });

            ProbabilisticPipingCalculationScenario[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                                                    .OfType<ProbabilisticPipingCalculationScenario>()
                                                                                                    .Where(c => c.HasOutput)
                                                                                                    .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearAllProbabilisticCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .OfType<ProbabilisticPipingCalculationScenario>()
                                          .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.AddRange(new IPipingCalculationScenario<PipingInput>[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = false,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    },
                    Output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties())
                },
                new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    },
                    Output = new ProbabilisticPipingOutput(new TestPartialProbabilisticPipingOutput(0, new TestGeneralResult<TestTopLevelIllustrationPoint>()),
                                                           new TestPartialProbabilisticPipingOutput(0, new TestGeneralResult<TestTopLevelIllustrationPoint>()))
                },
                new TestPipingCalculationScenario(new TestPipingInput
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                })
            });

            IPipingCalculationScenario<PipingInput>[] calculations = failureMechanism.Calculations.Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                     .ToArray();

            var expectedAffectedCalculations = new List<IPipingCalculationScenario<PipingInput>>();
            expectedAffectedCalculations.AddRange(calculations.OfType<SemiProbabilisticPipingCalculationScenario>()
                                                              .Where(c => !c.InputParameters.UseAssessmentLevelManualInput
                                                                          && c.InputParameters.HydraulicBoundaryLocation == hydraulicBoundaryLocation
                                                                          && c.HasOutput));
            expectedAffectedCalculations.AddRange(calculations.OfType<ProbabilisticPipingCalculationScenario>()
                                                              .Where(c => c.InputParameters.HydraulicBoundaryLocation == hydraulicBoundaryLocation
                                                                          && c.HasOutput));

            var expectedAffectedItems = new List<IObservable>(expectedAffectedCalculations);
            expectedAffectedItems.AddRange(calculations.Select(c => c.InputParameters)
                                                       .Where(i => i.HydraulicBoundaryLocation == hydraulicBoundaryLocation));

            // Call
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(
                failureMechanism, new[]
                {
                    hydraulicBoundaryLocation
                });

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(expectedAffectedCalculations.All(c => !c.HasOutput && c.InputParameters.HydraulicBoundaryLocation == null));
            Assert.IsTrue(calculations.All(c => c.InputParameters.HydraulicBoundaryLocation != hydraulicBoundaryLocation));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveReferenceLineDependentDataAndReturnAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.SectionConfigurations)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.StochasticSoilModels)
                                                              .Concat(failureMechanism.SurfaceLines)
                                                              .ToArray();

            // Call
            ClearResults results = PipingDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.SectionConfigurations);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(6, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.SectionConfigurations);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(array, failureMechanism.SurfaceLines);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        [Test]
        public void RemoveSurfaceLine_PipingFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            // Call
            void Call() => PipingDataSynchronizationService.RemoveSurfaceLine(null, surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveSurfaceLine_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void RemoveSurfaceLine_FullyConfiguredPipingFailureMechanism_RemoveSurfaceLineAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            PipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines[0];
            IPipingCalculationScenario<PipingInput>[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                                    .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                                    .Where(c => ReferenceEquals(c.InputParameters.SurfaceLine, surfaceLine))
                                                                                                    .ToArray();
            IPipingCalculationScenario<PipingInput>[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, surfaceLine);
            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithSurfaceLine.Select(calc => calc.InputParameters)
                                           .Cast<IObservable>()
                                           .Concat(calculationsWithOutput)
                                           .Concat(new IObservable[]
                                           {
                                               failureMechanism.SurfaceLines
                                           });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjectsArray);
        }

        [Test]
        public void RemoveAllSurfaceLine_PipingFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.RemoveAllSurfaceLines(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllSurfaceLines_FullyConfiguredPipingFailureMechanism_RemoveAllSurfaceLinesAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IPipingCalculationScenario<PipingInput>[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                                    .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                                    .Where(calc => calc.InputParameters.SurfaceLine != null)
                                                                                                    .ToArray();
            IPipingCalculationScenario<PipingInput>[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveAllSurfaceLines(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithSurfaceLine.Select(calc => calc.InputParameters)
                                           .Cast<IObservable>()
                                           .Concat(calculationsWithOutput)
                                           .Concat(new IObservable[]
                                           {
                                               failureMechanism.SurfaceLines
                                           });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjectsArray);
        }

        [Test]
        public void RemoveStochasticSoilModel_PipingFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Call
            void Call() => PipingDataSynchronizationService.RemoveStochasticSoilModel(null, soilModel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilModel_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilModel", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilModel_FullyConfiguredPipingFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            PipingStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[1];
            IPipingCalculationScenario<PipingInput>[] calculationsWithSoilModel = failureMechanism.Calculations
                                                                                                  .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                                  .Where(c => ReferenceEquals(c.InputParameters.StochasticSoilModel, soilModel))
                                                                                                  .ToArray();
            IPipingCalculationScenario<PipingInput>[] calculationsWithOutput = calculationsWithSoilModel.Where(c => c.HasOutput)
                                                                                                        .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSoilModel);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, soilModel);
            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithSoilModel.Select(calc => calc.InputParameters)
                                         .Cast<IObservable>()
                                         .Concat(calculationsWithOutput)
                                         .Concat(new IObservable[]
                                         {
                                             failureMechanism.StochasticSoilModels
                                         });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjectsArray);
        }

        [Test]
        public void RemoveAllStochasticSoilModel_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.RemoveAllStochasticSoilModels(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllStochasticSoilModel_FullyConfiguredPipingFailureMechanism_RemovesAllSoilModelAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IPipingCalculationScenario<PipingInput>[] calculationsWithStochasticSoilModel = failureMechanism.Calculations
                                                                                                            .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                                                            .Where(calc => calc.InputParameters.StochasticSoilModel != null)
                                                                                                            .ToArray();
            IPipingCalculationScenario<PipingInput>[] calculationsWithOutput = calculationsWithStochasticSoilModel.Where(c => c.HasOutput)
                                                                                                                  .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithStochasticSoilModel);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveAllStochasticSoilModels(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithStochasticSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithStochasticSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithStochasticSoilModel.Select(calc => calc.InputParameters)
                                                   .Cast<IObservable>()
                                                   .Concat(calculationsWithOutput)
                                                   .Concat(new IObservable[]
                                                   {
                                                       failureMechanism.StochasticSoilModels
                                                   });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjectsArray);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            void Call() => PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(null, stochasticSoilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    input.StochasticSoilProfile = null;
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.IsEmpty(affected);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithOutputWithProfile_ReturnInputWithoutProfile()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<PipingInput>();

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    calculation.ClearOutput();
                    expectedInputs.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedInputs, affected);
            CollectionAssert.IsEmpty(affected.Cast<PipingInput>().Where(a => a.StochasticSoilProfile != null));
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_CalculationWithOutputWithProfile_ReturnInputWithoutProfileAndCalculation()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();

            var expectedAffectedObjects = new List<IObservable>();

            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    if (calculation.HasOutput)
                    {
                        expectedAffectedObjects.Add(calculation);
                    }

                    expectedAffectedObjects.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affected);
            CollectionAssert.IsEmpty(affected.OfType<PipingInput>().Where(a => a.StochasticSoilProfile != null));
            CollectionAssert.IsEmpty(affected.OfType<IPipingCalculationScenario<PipingInput>>().Where(a => a.HasOutput));
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            void Call() => PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(null, stochasticSoilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    input.StochasticSoilProfile = null;
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.IsEmpty(affected);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithOutputWithProfile_ReturnInput()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<PipingInput>();

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    calculation.ClearOutput();
                    expectedInputs.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedInputs, affected);
            CollectionAssert.IsEmpty(affected.Cast<PipingInput>().Where(a => a.StochasticSoilProfile == null));
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_CalculationWithOutputWithProfile_ReturnInputAndCalculation()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = failureMechanism.Calculations
                                                                                                .Cast<IPipingCalculationScenario<PipingInput>>();

            var expectedAffectedObjects = new List<IObservable>();

            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
            {
                PipingInput input = calculation.InputParameters;
                PipingStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    if (calculation.HasOutput)
                    {
                        expectedAffectedObjects.Add(calculation);
                    }

                    expectedAffectedObjects.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affected);
            CollectionAssert.IsEmpty(affected.OfType<PipingInput>().Where(a => a.StochasticSoilProfile == null));
            CollectionAssert.IsEmpty(affected.OfType<IPipingCalculationScenario<PipingInput>>().Where(a => a.HasOutput));
        }
    }
}