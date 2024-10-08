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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Data.TestUtil;

namespace Riskeer.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverDataSynchronizationServiceTest
    {
        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
                null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
                new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            StabilityStoneCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism(hydraulicBoundaryLocation1);
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    }
                },
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    },
                    Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
                }
            });

            StabilityStoneCoverWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                                          .ToArray();

            StabilityStoneCoverWaveConditionsCalculation[] expectedAffectedCalculations = calculations.Where(
                c => c.InputParameters.HydraulicBoundaryLocation == hydraulicBoundaryLocation1
                     && c.HasOutput).ToArray();

            var expectedAffectedItems = new List<IObservable>(expectedAffectedCalculations);
            expectedAffectedItems.AddRange(calculations.Select(c => c.InputParameters)
                                                       .Where(i => i.HydraulicBoundaryLocation == hydraulicBoundaryLocation1));

            // Call
            IEnumerable<IObservable> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(
                failureMechanism, new[]
                {
                    hydraulicBoundaryLocation1
                });

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(expectedAffectedCalculations.All(c => !c.HasOutput && c.InputParameters.HydraulicBoundaryLocation == null));
            Assert.IsTrue(calculations.All(c => c.InputParameters.HydraulicBoundaryLocation != hydraulicBoundaryLocation1));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_CalculationWithoutOutput_ReturnNoAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());

            // Call
            IEnumerable<IObservable> affectedItems =
                StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentDataAndReturnAffectedObjects()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .ToArray();

            // Call
            ClearResults results = StabilityStoneCoverDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        private static StabilityStoneCoverFailureMechanism CreateFullyConfiguredFailureMechanism(HydraulicBoundaryLocation hydraulicBoundaryLocation = null)
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

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, "some/path/to/sections");

            if (hydraulicBoundaryLocation == null)
            {
                hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            }

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var calculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var calculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var subCalculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var subCalculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
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