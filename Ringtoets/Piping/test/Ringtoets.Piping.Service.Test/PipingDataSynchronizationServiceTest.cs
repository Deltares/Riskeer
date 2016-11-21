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
using Ringtoets.HydraRing.Data;
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
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            // Call
            PipingDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
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
            PipingFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<PipingCalculation> affectedItems = PipingDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Any(c => c.HasOutput));
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            PipingCalculation[] expectedAffectedCalculations = failureMechanism.Calculations.Cast<PipingCalculation>()
                                                                               .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput)
                                                                               .ToArray();

            // Call
            IEnumerable<PipingCalculation> affectedItems = PipingDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Cast<PipingCalculation>()
                                           .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
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
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            IObservable[] observables = PipingDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism).ToArray();

            // Assert
            Assert.AreEqual(4, observables.Length);

            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.Contains(observables, failureMechanism);

            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.Contains(observables, failureMechanism.CalculationsGroup);

            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.Contains(observables, failureMechanism.StochasticSoilModels);

            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.Contains(observables, failureMechanism.SurfaceLines);
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
        public void RemoveSurfaceLine_PipingFailureMechanismProfileNull_ThrowsArgumentNullException()
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
            IObservable[] observables = PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1 + calculations.Length, observables.Length);

            CollectionAssert.DoesNotContain(failureMechanism.SurfaceLines, surfaceLine);
            CollectionAssert.Contains(observables, failureMechanism.SurfaceLines);

            foreach (PipingCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.SurfaceLine);
                CollectionAssert.Contains(observables, calculation.InputParameters);
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
        public void RemoveStochasticSoilModel_PipingFailureMechanismProfileNull_ThrowsArgumentNullException()
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
            IObservable[] observables = PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, soilModel).ToArray();

            // Assert
            Assert.AreEqual(1 + calculations.Length, observables.Length);

            CollectionAssert.DoesNotContain(failureMechanism.StochasticSoilModels, soilModel);
            CollectionAssert.Contains(observables, failureMechanism.StochasticSoilModels);

            foreach (PipingCalculation calculation in calculations)
            {
                Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
                CollectionAssert.Contains(observables, calculation.InputParameters);
            }
        }

        private static PipingFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new PipingFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new PipingCalculation(new GeneralPipingInput());
            var calculationWithOutput = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new PipingCalculation(new GeneralPipingInput());
            var subCalculationWithOutput = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation
                }
            });

            return failureMechanism;
        }
    }
}