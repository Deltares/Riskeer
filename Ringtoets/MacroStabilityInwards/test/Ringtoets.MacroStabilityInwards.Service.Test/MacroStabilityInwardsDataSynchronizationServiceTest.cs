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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Service;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutputAndReturnAffectedCaculations()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            // Call
            IEnumerable<IObservable> changedObjects = MacroStabilityInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsNull(calculation.Output);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation();

            // Call
            IEnumerable<IObservable> changedObjects = MacroStabilityInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

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
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            MacroStabilityInwardsCalculation[] calculations = failureMechanism.Calculations.Cast<MacroStabilityInwardsCalculation>().ToArray();
            IObservable[] expectedAffectedCalculations = calculations
                .Where(c => c.HasOutput)
                .Cast<IObservable>()
                .ToArray();
            IObservable[] expectedAffectedCalculationInputs = calculations
                .Select(c => c.InputParameters)
                .Where(i => i.HydraulicBoundaryLocation != null)
                .Cast<IObservable>()
                .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = MacroStabilityInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .Cast<MacroStabilityInwardsCalculation>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(null, surfaceLine);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveSurfaceLine_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("surfaceLine", paramName);
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
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveAllSurfaceLines(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(null, soilModel);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveStochasticSoilModel_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("soilModel", paramName);
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
            TestDelegate call = () => MacroStabilityInwardsDataSynchronizationService.RemoveAllStochasticSoilModels(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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
            TestDelegate test = () => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                null,
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(
                new MacroStabilityInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void RemoveStochasticSoilProfileFromInput_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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
            TestDelegate test = () => MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                null,
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_WithoutSoilProfile_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(
                new MacroStabilityInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void ClearStochasticSoilProfileDependentData_NoCalculationsWithProfile_ReturnNoAffectedObjects()
        {
            // Setup
            MacroStabilityInwardsFailureMechanism failureMechanism = MacroStabilityInwardsTestDataGenerator.GetMacroStabilityInwardsFailureMechanismWithAllCalculationConfigurations();
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = failureMechanism
                .Calculations
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