﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
            
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism(hydraulicBoundaryLocation1);
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new GrassCoverErosionInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    }
                },
                new GrassCoverErosionInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    },
                    Output = new TestGrassCoverErosionInwardsOutput()
                }
            });

            IEnumerable<GrassCoverErosionInwardsCalculation> calculations = failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                                                                            .ToArray();

            IEnumerable<GrassCoverErosionInwardsCalculation> expectedAffectedCalculations = calculations.Where(
                c => c.InputParameters.HydraulicBoundaryLocation == hydraulicBoundaryLocation1
                     && c.HasOutput).ToArray();

            var expectedAffectedItems = new List<IObservable>(expectedAffectedCalculations);
            expectedAffectedItems.AddRange(calculations.Select(c => c.InputParameters)
                                                       .Where(i => i.HydraulicBoundaryLocation == hydraulicBoundaryLocation1));

            // Call
            IEnumerable<IObservable> affectedItems = GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(
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
        public void ClearIllustrationPoints_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.ClearIllustrationPoints(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void ClearIllustrationPoints_CalculationsWithAndWithoutIllustrationPoints_ReturnsAffectedObjects()
        {
            // Setup
            var random = new Random(21);

            var overtoppingOutputWithIllustrationPoints = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                                                             null,
                                                                                             null);
            var calculationWithOverToppingOutputWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = overtoppingOutputWithIllustrationPoints
            };

            var dikeHeightOutputWithIllustrationPoints = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                                                            new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                                                            null);
            var calculationWithDikeHeightWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = dikeHeightOutputWithIllustrationPoints
            };

            var overtoppingRateOutputWithIllustrationPoints = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                                                                 null,
                                                                                                 new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint()));
            var calculationWithOvertoppingRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = overtoppingRateOutputWithIllustrationPoints
            };

            var outputWithNoIllustrationPoints = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                                                    null,
                                                                                    null);
            var calculationWitNoIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = outputWithNoIllustrationPoints
            };

            GrassCoverErosionInwardsCalculation[] calculations =
            {
                calculationWitNoIllustrationPoints,
                calculationWithOverToppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints,
                new GrassCoverErosionInwardsCalculation()
            };

            // Call
            IEnumerable<IObservable> affectedObjects = GrassCoverErosionInwardsDataSynchronizationService.ClearIllustrationPoints(calculations);

            // Assert
            CollectionAssert.AreEquivalent(new[]
            {
                calculationWithOverToppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints
            }, affectedObjects);

            GrassCoverErosionInwardsCalculation[] calculationsWithOutput =
            {
                calculationWitNoIllustrationPoints,
                calculationWithOverToppingOutputWithIllustrationPoints,
                calculationWithOvertoppingRateWithIllustrationPoints,
                calculationWithDikeHeightWithIllustrationPoints
            };
            Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));

            Assert.IsTrue(calculationsWithOutput.All(calc =>
            {
                GrassCoverErosionInwardsOutput output = calc.Output;

                return !output.OvertoppingOutput.HasGeneralResult
                       && output.DikeHeightOutput?.GeneralResult == null
                       && output.OvertoppingRateOutput?.GeneralResult == null;
            }));
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
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
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);

            CollectionAssert.AreEquivalent(expectedRemovedObjectInstances, result.RemovedObjects);
        }

        [Test]
        public void RemoveAllDikeProfile_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.RemoveAllDikeProfiles(
                null, new DikeProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void RemoveAllDikeProfile_DikeProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.RemoveAllDikeProfiles(
                Enumerable.Empty<GrassCoverErosionInwardsCalculation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dikeProfiles", exception.ParamName);
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

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithDikeProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                GrassCoverErosionInwardsDataSynchronizationService.RemoveAllDikeProfiles(
                    failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(),
                    failureMechanism.DikeProfiles);

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
        public void RemoveDikeProfile_DikeProfileToRemove_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(
                null, Enumerable.Empty<GrassCoverErosionInwardsCalculation>(), new DikeProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dikeProfileToRemove", exception.ParamName);
        }

        [Test]
        public void RemoveDikeProfile_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(
                DikeProfileTestFactory.CreateDikeProfile(), null, new DikeProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void RemoveDikeProfile_DikeProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(
                DikeProfileTestFactory.CreateDikeProfile(), Enumerable.Empty<GrassCoverErosionInwardsCalculation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dikeProfiles", exception.ParamName);
        }

        [Test]
        public void RemoveDikeProfile_VariousCalculationConfigurations_ReturnsOnlyAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            DikeProfile profileToBeCleared = failureMechanism.DikeProfiles[0];
            DikeProfile profileToKeep = failureMechanism.DikeProfiles[1];

            GrassCoverErosionInwardsCalculation[] affectedCalculationsWithProfile =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => ReferenceEquals(profileToBeCleared, calc.InputParameters.DikeProfile))
                                .ToArray();
            GrassCoverErosionInwardsCalculation[] affectedCalculationsWithOutput =
                affectedCalculationsWithProfile.Where(calc => calc.HasOutput).ToArray();

            GrassCoverErosionInwardsCalculation[] unaffectedCalculationsWithProfile =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .Where(calc => ReferenceEquals(profileToKeep, calc.InputParameters.DikeProfile))
                                .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(affectedCalculationsWithOutput);

            // Call
            IEnumerable<IObservable> affectedObjects =
                GrassCoverErosionInwardsDataSynchronizationService.RemoveDikeProfile(profileToBeCleared,
                                                                                     failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(),
                                                                                     failureMechanism.DikeProfiles);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.DikeProfiles, profileToBeCleared);

            Assert.True(affectedCalculationsWithOutput.All(calc => !calc.HasOutput));
            Assert.True(unaffectedCalculationsWithProfile.All(calc => calc.InputParameters.DikeProfile != null));

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

        private static GrassCoverErosionInwardsFailureMechanism CreateFullyConfiguredFailureMechanism(HydraulicBoundaryLocation hydraulicBoundaryLocation = null)
        {
            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile("Profile 1", "ID 1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile("Profile 2", "ID 2");

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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, "some/path/to/dikeprofiles");
            failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, "some/path/to/sections");

            if (hydraulicBoundaryLocation == null)
            {
                hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            }

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
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
                    DikeProfile = dikeProfile1
                }
            };
            var calculationWithDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile2
                }
            };
            var calculationWithOutputHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile1
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var subCalculation = new GrassCoverErosionInwardsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestGrassCoverErosionInwardsOutput()
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
                    DikeProfile = dikeProfile1
                }
            };
            var subCalculationWithDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile2
                }
            };
            var subCalculationWithOutputHydraulicBoundaryLocationAndDikeProfile = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile1
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