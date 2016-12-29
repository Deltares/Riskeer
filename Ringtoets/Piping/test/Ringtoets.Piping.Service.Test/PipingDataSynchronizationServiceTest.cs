// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Integration.TestUtils;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service.Test
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
        public void ClearCalculationOutput_WithCalculation_ClearsOutputAndReturnAffectedCaculations()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            // Call
            IEnumerable<IObservable> changedObjects = PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = null,
                SemiProbabilisticOutput = null
            };

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
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetFullyConfiguredPipingFailureMechanism();
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
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetFullyConfiguredPipingFailureMechanism();
            PipingCalculation[] calculations = failureMechanism.Calculations.Cast<PipingCalculation>().ToArray();
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
            IEnumerable<IObservable> affectedItems = PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations
                                          .Cast<PipingCalculation>()
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
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetFullyConfiguredPipingFailureMechanism();

            var expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
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
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(array, failureMechanism.SurfaceLines);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.DeletedObjects);
        }

        [Test]
        public void RemoveSurfaceLine_PipingFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanism failureMechanism = null;
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveSurfaceLine_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLine surfaceLine = null;

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("surfaceLine", paramName);
        }

        [Test]
        public void RemoveSurfaceLine_FullyConfiguredPipingFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetFullyConfiguredPipingFailureMechanism();
            RingtoetsPipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines[0];
            PipingCalculation[] calculations = failureMechanism.Calculations.Cast<PipingCalculation>()
                                                               .Where(c => ReferenceEquals(c.InputParameters.SurfaceLine, surfaceLine))
                                                               .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, surfaceLine);
            foreach (PipingCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.SurfaceLines);
            foreach (PipingCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }

        [Test]
        public void RemoveStochasticSoilModel_PipingFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanism failureMechanism = null;
            StochasticSoilModel soilModel = new StochasticSoilModel(1, "A", "B");

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveStochasticSoilModel_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();
            StochasticSoilModel soilModel = null;

            // Call
            TestDelegate call = () => PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("soilModel", paramName);
        }

        [Test]
        public void RemoveStochasticSoilModel_FullyConfiguredPipingFailureMechanism_RemoveProfileAndClearDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetFullyConfiguredPipingFailureMechanism();
            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[1];
            PipingCalculation[] calculations = failureMechanism.Calculations.Cast<PipingCalculation>()
                                                               .Where(c => ReferenceEquals(c.InputParameters.StochasticSoilModel, soilModel))
                                                               .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculations);

            // Call
            IEnumerable<IObservable> observables = PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, soilModel);
            foreach (PipingCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            }

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(1 + calculations.Length, array.Length);

            CollectionAssert.Contains(array, failureMechanism.StochasticSoilModels);
            foreach (PipingCalculation calculation in calculations)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }
        }
    }
}