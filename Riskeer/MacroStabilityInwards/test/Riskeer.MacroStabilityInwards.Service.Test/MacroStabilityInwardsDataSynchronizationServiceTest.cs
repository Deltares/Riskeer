﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutputWithoutManualAssessmentLevel_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputWithoutManualAssessmentLevel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputWithoutManualAssessmentLevel_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new MacroStabilityInwardsCalculationScenario(),
                new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        UseAssessmentLevelManualInput = true
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                }
            });

            MacroStabilityInwardsCalculationScenario[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                                                      .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                                      .Where(c => !c.InputParameters.UseAssessmentLevelManualInput && c.HasOutput)
                                                                                                      .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputWithoutManualAssessmentLevel(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .OfType<MacroStabilityInwardsCalculationScenario>()
                                          .Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                          .All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    UseAssessmentLevelManualInput = true
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            });

            MacroStabilityInwardsCalculation[] calculations = failureMechanism.Calculations
                                                                              .Cast<MacroStabilityInwardsCalculation>()
                                                                              .ToArray();

            var expectedAffectedItems = new List<IObservable>();
            expectedAffectedItems.AddRange(calculations.Where(c => !c.InputParameters.UseAssessmentLevelManualInput
                                                                   && c.HasOutput));
            expectedAffectedItems.AddRange(calculations.Select(c => c.InputParameters)
                                                       .Where(i => i.HydraulicBoundaryLocation != null));

            // Call
            IEnumerable<IObservable> affectedItems = MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(calculations.Where(c => !c.InputParameters.UseAssessmentLevelManualInput)
                                      .All(c => c.InputParameters.HydraulicBoundaryLocation == null
                                                && !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveReferenceLineDependentDataAndReturnAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.StochasticSoilModels)
                                                              .Concat(failureMechanism.SurfaceLines)
                                                              .ToArray();

            // Call
            ClearResults results = MacroStabilityInwardsDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

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
        public void RemoveSurfaceLine_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(null, surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveSurfaceLine_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void RemoveSurfaceLine_FullyConfiguredFailureMechanism_RemoveSurfaceLineAndClearDependentData()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            MacroStabilityInwardsSurfaceLine surfaceLine = failureMechanism.SurfaceLines[0];
            MacroStabilityInwardsCalculation[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                             .Cast<MacroStabilityInwardsCalculation>()
                                                                                             .Where(c => ReferenceEquals(c.InputParameters.SurfaceLine, surfaceLine))
                                                                                             .ToArray();
            MacroStabilityInwardsCalculation[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                   .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, surfaceLine);
            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithOutput)
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
        public void RemoveAllSurfaceLine_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveAllSurfaceLines(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllSurfaceLines_FullyConfiguredFailureMechanism_RemoveAllSurfaceLinesAndClearDependentData()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            MacroStabilityInwardsCalculation[] calculationsWithSurfaceLine = failureMechanism.Calculations
                                                                                             .Cast<MacroStabilityInwardsCalculation>()
                                                                                             .Where(calc => calc.InputParameters.SurfaceLine != null)
                                                                                             .ToArray();
            MacroStabilityInwardsCalculation[] calculationsWithOutput = calculationsWithSurfaceLine.Where(c => c.HasOutput)
                                                                                                   .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSurfaceLine);

            // Call
            IEnumerable<IObservable> observables = MacroStabilityInwardsDataSynchronizationService.RemoveAllSurfaceLines(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithSurfaceLine)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSurfaceLine.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithOutput)
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
        public void RemoveStochasticSoilModel_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel soilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();

            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(null, soilModel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilModel_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilModel", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilModel_FullyConfiguredFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            MacroStabilityInwardsStochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[1];
            MacroStabilityInwardsCalculation[] calculationsWithSoilModel = failureMechanism.Calculations
                                                                                           .Cast<MacroStabilityInwardsCalculation>()
                                                                                           .Where(c => ReferenceEquals(c.InputParameters.StochasticSoilModel, soilModel))
                                                                                           .ToArray();
            MacroStabilityInwardsCalculation[] calculationsWithOutput = calculationsWithSoilModel.Where(c => c.HasOutput)
                                                                                                 .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithSoilModel);

            // Call
            IEnumerable<IObservable> observables = MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, soilModel);
            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithOutput)
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
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveAllStochasticSoilModels(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllStochasticSoilModel_FullyConfiguredFailureMechanism_RemovesAllSoilModelAndClearDependentData()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            MacroStabilityInwardsCalculation[] calculationsWithStochasticSoilModel = failureMechanism.Calculations
                                                                                                     .Cast<MacroStabilityInwardsCalculation>()
                                                                                                     .Where(calc => calc.InputParameters.StochasticSoilModel != null)
                                                                                                     .ToArray();
            MacroStabilityInwardsCalculation[] calculationsWithOutput = calculationsWithStochasticSoilModel.Where(c => c.HasOutput)
                                                                                                           .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithStochasticSoilModel);

            // Call
            IEnumerable<IObservable> observables = MacroStabilityInwardsDataSynchronizationService.RemoveAllStochasticSoilModels(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithStochasticSoilModel)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] affectedObjectsArray = observables.ToArray();
            int expectedAffectedObjectCount = 1 + calculationsWithOutput.Length + calculationsWithStochasticSoilModel.Length;
            Assert.AreEqual(expectedAffectedObjectCount, affectedObjectsArray.Length);

            foreach (MacroStabilityInwardsCalculation calculation in calculationsWithOutput)
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
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                null, new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();
            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
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
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.IsEmpty(affected);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithOutputWithProfile_ReturnInputWithoutProfile()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();
            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<MacroStabilityInwardsInput>();

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    calculationScenario.ClearOutput();
                    expectedInputs.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedInputs, affected);
            CollectionAssert.IsEmpty(affected.Cast<MacroStabilityInwardsInput>().Where(a => a.StochasticSoilProfile != null));
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_CalculationWithOutputWithProfile_ReturnInputWithoutProfileAndCalculation()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();

            var expectedAffectedObjects = new List<IObservable>();

            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    if (calculationScenario.HasOutput)
                    {
                        expectedAffectedObjects.Add(calculationScenario);
                    }

                    expectedAffectedObjects.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affected);
            CollectionAssert.IsEmpty(affected.OfType<MacroStabilityInwardsInput>().Where(a => a.StochasticSoilProfile != null));
            CollectionAssert.IsEmpty(affected.OfType<MacroStabilityInwardsCalculation>().Where(a => a.HasOutput));
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                null, new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();
            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
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
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.IsEmpty(affected);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithOutputWithProfile_ReturnInput()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();
            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            var expectedInputs = new List<MacroStabilityInwardsInput>();

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    calculationScenario.ClearOutput();
                    expectedInputs.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedInputs, affected);
            CollectionAssert.IsEmpty(affected.Cast<MacroStabilityInwardsInput>().Where(a => a.StochasticSoilProfile == null));
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_CalculationWithOutputWithProfile_ReturnInputAndCalculation()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism.Calculations
                                                                                                 .Cast<MacroStabilityInwardsCalculationScenario>();

            var expectedAffectedObjects = new List<IObservable>();

            MacroStabilityInwardsStochasticSoilProfile profileToDelete = null;

            foreach (MacroStabilityInwardsCalculationScenario calculationScenario in calculations)
            {
                MacroStabilityInwardsInput input = calculationScenario.InputParameters;
                MacroStabilityInwardsStochasticSoilProfile currentProfile = input.StochasticSoilProfile;
                if (profileToDelete == null)
                {
                    profileToDelete = currentProfile;
                }

                if (profileToDelete != null && ReferenceEquals(profileToDelete, currentProfile))
                {
                    if (calculationScenario.HasOutput)
                    {
                        expectedAffectedObjects.Add(calculationScenario);
                    }

                    expectedAffectedObjects.Add(input);
                }
            }

            // Call
            IEnumerable<IObservable> affected = MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, profileToDelete);

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affected);
            CollectionAssert.IsEmpty(affected.OfType<MacroStabilityInwardsInput>().Where(a => a.StochasticSoilProfile == null));
            CollectionAssert.IsEmpty(affected.OfType<MacroStabilityInwardsCalculation>().Where(a => a.HasOutput));
        }
    }
}