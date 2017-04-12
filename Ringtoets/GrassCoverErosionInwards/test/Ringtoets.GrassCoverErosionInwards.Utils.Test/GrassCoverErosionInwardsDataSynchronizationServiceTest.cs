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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Service;

namespace Ringtoets.GrassCoverErosionInwards.Utils.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestSubCalculationAssessmentOutput(0),
                                                            new TestSubCalculationAssessmentOutput(0))
            };

            // Call
            IEnumerable<IObservable> changedObjects = GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

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
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = null
            };

            // Call
            IEnumerable<IObservable> changedObjects = GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

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
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            IEnumerable<GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .ToArray();
            IObservable[] expectedAffectedCalculations = grassCoverErosionInwardsCalculations
                .Where(c => c.HasOutput)
                .Cast<IObservable>()
                .ToArray();
            IObservable[] expectedAffectedCalculationInputs = grassCoverErosionInwardsCalculations
                .Select(c => c.InputParameters)
                .Where(i => i.HydraulicBoundaryLocation != null)
                .Cast<IObservable>()
                .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjectInstances = failureMechanism.Sections.OfType<object>()
                                                                      .Concat(failureMechanism.SectionResults)
                                                                      .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                                      .Concat(failureMechanism.DikeProfiles)
                                                                      .ToArray();

            // Call
            ClearResults result = GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.DikeProfiles);

            IObservable[] array = result.ChangedObjects.ToArray();
            Assert.AreEqual(3, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);

            CollectionAssert.AreEquivalent(expectedRemovedObjectInstances, result.RemovedObjects);
        }

        [Test]
        public void RemoveAllDikeProfiles_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.RemoveAllDikeProfiles(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllDikeProfiles_FullyConfiguredFailureMechanism_RemovesAllDikeProfilesAndDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            GrassCoverErosionInwardsCalculation[] calculationsWithDikeProfiles =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => calc.InputParameters.DikeProfile != null)
                                .ToArray();
            GrassCoverErosionInwardsCalculation[] calculationsWithDikeProfilesAndOutput =
                calculationsWithDikeProfiles.Where(calc => calc.HasOutput).ToArray();

            // Pre-condition
            CollectionAssert.IsNotEmpty(calculationsWithDikeProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                GrassCoverErosionInwardsDataSynchronizationService.RemoveAllDikeProfiles(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.DikeProfiles);
            foreach (GrassCoverErosionInwardsCalculation calculation in calculationsWithDikeProfilesAndOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithDikeProfiles.Select(calc => calc.InputParameters)
                                            .Cast<IObservable>()
                                            .Concat(calculationsWithDikeProfilesAndOutput)
                                            .Concat(new IObservable[]
                                            {
                                                failureMechanism.DikeProfiles
                                            });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveDikeProfile_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(null, new TestDikeProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveDikeProfile_DikeProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dikeProfile", exception.ParamName);
        }

        [Test]
        public void RemoveDikeProfile_FullyConfiguredFailureMechanism_ReturnsOnlyAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            DikeProfile profileToBeCleared = failureMechanism.DikeProfiles[0];

            GrassCoverErosionInwardsCalculation[] affectedCalculationsWithProfile =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => ReferenceEquals(profileToBeCleared, calc.InputParameters.DikeProfile))
                                .ToArray();
            GrassCoverErosionInwardsCalculation[] affectedCalculationsWithOutput =
                affectedCalculationsWithProfile.Where(calc => calc.HasOutput).ToArray();

            // Pre-condition
            CollectionAssert.IsNotEmpty(affectedCalculationsWithOutput);

            // Call
            IEnumerable<IObservable> affectedObjects =
                GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(failureMechanism, profileToBeCleared);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, profileToBeCleared);

            foreach (GrassCoverErosionInwardsCalculation calculation in affectedCalculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IEnumerable<IObservable> expectedAffectedObjects =
                affectedCalculationsWithProfile.Select(calc => calc.InputParameters)
                                               .Cast<IObservable>()
                                               .Concat(affectedCalculationsWithOutput)
                                               .Concat(new IObservable[]
                                               {
                                                   failureMechanism.DikeProfiles
                                               });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private static GrassCoverErosionInwardsFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var testDikeProfile1 = new TestDikeProfile("Profile 1", "ID 1");
            var testDikeProfile2 = new TestDikeProfile("Profile 2", "ID 2");

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                testDikeProfile1,
                testDikeProfile2
            }, "some/path/to/dikeprofiles");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestSubCalculationAssessmentOutput(0),
                                                            new TestSubCalculationAssessmentOutput(0))
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestSubCalculationAssessmentOutput(0),
                                                            new TestSubCalculationAssessmentOutput(0))
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = testDikeProfile1
                }
            };
            var calculationWithDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = testDikeProfile2
                }
            };
            var calculationWithOutputHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = testDikeProfile1
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var subCalculation = new GrassCoverErosionInwardsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestSubCalculationAssessmentOutput(0),
                                                            new TestSubCalculationAssessmentOutput(0))
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestSubCalculationAssessmentOutput(0),
                                                            new TestSubCalculationAssessmentOutput(0))
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = testDikeProfile1
                }
            };
            var subCalculationWithDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = testDikeProfile2
                }
            };
            var subCalculationWithOutputHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = testDikeProfile1
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithDikeProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocationAndDikeProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputHydraulicBoundaryLocationAndDikeProfile);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithDikeProfile,
                    subCalculationWithHydraulicBoundaryLocationAndDikeProfile,
                    subCalculationWithOutputHydraulicBoundaryLocationAndDikeProfile
                }
            });

            return failureMechanism;
        }
    }
}