﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
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
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutputAndReturnAffectedCalculations()
        {
            // Setup
            var calculation = new TestPipingCalculation(true);

            // Call
            IEnumerable<IObservable> changedObjects = PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsFalse(calculation.HasOutput);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new TestPipingCalculation();

            // Call
            IEnumerable<IObservable> changedObjects = PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IPipingCalculation<PipingInput>[] calculations = failureMechanism.Calculations.Cast<IPipingCalculation<PipingInput>>().ToArray();
            IObservable[] expectedAffectedCalculations = calculations.Where(c => c.HasOutput)
                                                                     .Cast<IObservable>()
                                                                     .ToArray();
            IObservable[] expectedAffectedCalculationInputs = calculations.Select(c => c.InputParameters)
                                                                          .Where(i => i.HydraulicBoundaryLocation != null)
                                                                          .Cast<IObservable>()
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .Cast<IPipingCalculation<PipingInput>>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveReferenceLineDependentDataAndReturnAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
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
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(5, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
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
            TestDelegate call = () => PipingDataSynchronizationService.RemoveSurfaceLine(null, surfaceLine);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveSurfaceLine_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("surfaceLine", paramName);
        }

        [Test]
        public void RemoveSurfaceLine_FullyConfiguredPipingFailureMechanism_RemoveSurfaceLineAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            PipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines[0];
            IPipingCalculation<PipingInput>[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                            .Cast<IPipingCalculation<PipingInput>>()
                                                                                            .Where(c => ReferenceEquals(c.InputParameters.SurfaceLine, surfaceLine))
                                                                                            .ToArray();
            IPipingCalculation<PipingInput>[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                  .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, surfaceLine);
            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithOutput)
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
            TestDelegate call = () => PipingDataSynchronizationService.RemoveAllSurfaceLines(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveAllSurfaceLines_FullyConfiguredPipingFailureMechanism_RemoveAllSurfaceLinesAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IPipingCalculation<PipingInput>[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                            .Cast<IPipingCalculation<PipingInput>>()
                                                                                            .Where(calc => calc.InputParameters.SurfaceLine != null)
                                                                                            .ToArray();
            IPipingCalculation<PipingInput>[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                  .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveAllSurfaceLines(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithOutput)
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
            TestDelegate call = () => PipingDataSynchronizationService.RemoveStochasticSoilModel(null, soilModel);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveStochasticSoilModel_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("soilModel", paramName);
        }

        [Test]
        public void RemoveStochasticSoilModel_FullyConfiguredPipingFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            PipingStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[1];
            IPipingCalculation<PipingInput>[] calculationsWithSoilModel = failureMechanism.Calculations
                                                                                          .Cast<IPipingCalculation<PipingInput>>()
                                                                                          .Where(c => ReferenceEquals(c.InputParameters.StochasticSoilModel, soilModel))
                                                                                          .ToArray();
            IPipingCalculation<PipingInput>[] calculationsWithOutput = calculationsWithSoilModel.Where(c => c.HasOutput)
                                                                                                .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSoilModel);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, soilModel);
            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithOutput)
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
            TestDelegate call = () => PipingDataSynchronizationService.RemoveAllStochasticSoilModels(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveAllStochasticSoilModel_FullyConfiguredPipingFailureMechanism_RemovesAllSoilModelAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IPipingCalculation<PipingInput>[] calculationsWithStochasticSoilModel = failureMechanism.Calculations
                                                                                                    .Cast<IPipingCalculation<PipingInput>>()
                                                                                                    .Where(calc => calc.InputParameters.StochasticSoilModel != null)
                                                                                                    .ToArray();
            IPipingCalculation<PipingInput>[] calculationsWithOutput = calculationsWithStochasticSoilModel.Where(c => c.HasOutput)
                                                                                                          .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithStochasticSoilModel);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveAllStochasticSoilModels(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithStochasticSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithStochasticSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (IPipingCalculation<PipingInput> calculation in calculationsWithOutput)
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
            TestDelegate test = () => PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                null, stochasticSoilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<PipingInput>();

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();

            var expectedAffectedObjects = new List<IObservable>();

            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            CollectionAssert.IsEmpty(affected.OfType<IPipingCalculation<PipingInput>>().Where(a => a.HasOutput));
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            TestDelegate test = () => PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                null, stochasticSoilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();
            PipingStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<PipingInput>();

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            IEnumerable<IPipingCalculation<PipingInput>> calculations = failureMechanism.Calculations
                                                                                        .Cast<IPipingCalculation<PipingInput>>();

            var expectedAffectedObjects = new List<IObservable>();

            PipingStochasticSoilProfile profileToDelete = null;

            foreach (IPipingCalculation<PipingInput> calculation in calculations)
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
            CollectionAssert.IsEmpty(affected.OfType<IPipingCalculation<PipingInput>>().Where(a => a.HasOutput));
        }
    }
}