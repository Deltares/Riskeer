// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverErosionInwardsDikeProfileUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/Path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call 
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<UpdateDataStrategyBase<DikeProfile, GrassCoverErosionInwardsFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IDikeProfileUpdateDataStrategy>(strategy);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(null,
                                                                                  string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_DikeProfilePropertiesChanged_UpdateRelevantProperties()
        {
            // Setup
            DikeProfile dikeProfileToUpdate = DikeProfileTestFactory.CreateDikeProfile("name", "ID A");
            DikeProfile dikeProfileToUpdateFrom = DeepCloneAndModify(dikeProfileToUpdate);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection targetCollection = failureMechanism.DikeProfiles;
            targetCollection.AddRange(new[]
            {
                dikeProfileToUpdate
            }, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                dikeProfileToUpdateFrom
            }, sourceFilePath);

            // Assert
            Assert.AreSame(dikeProfileToUpdate, targetCollection[0]);
            AssertDikeProfile(dikeProfileToUpdateFrom, dikeProfileToUpdate);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CurrentCollectionEmptyAndReadProfilesHaveDuplicateIds_ThrowsUpdateException()
        {
            // Setup
            const string duplicateId = "A duplicated ID";
            DikeProfile dikeProfileOne = DikeProfileTestFactory.CreateDikeProfile("name one", duplicateId);
            DikeProfile dikeProfileTwo = DikeProfileTestFactory.CreateDikeProfile("Another dike profile", duplicateId);

            var targetCollection = new DikeProfileCollection();
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                dikeProfileOne,
                dikeProfileTwo
            }, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);

            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfileAndImportedMultipleDikeProfilesWithSameId_ThrowsUpdateException()
        {
            // Setup
            const string duplicateId = "A duplicated ID";
            DikeProfile expectedDikeProfile = DikeProfileTestFactory.CreateDikeProfile("expectedName", duplicateId);

            var targetCollection = new DikeProfileCollection();
            DikeProfile[] expectedTargetCollection =
            {
                expectedDikeProfile
            };
            targetCollection.AddRange(expectedTargetCollection, sourceFilePath);

            DikeProfile[] importedTargetCollection =
            {
                DeepCloneAndModify(expectedDikeProfile),
                DeepCloneAndModify(expectedDikeProfile)
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(importedTargetCollection,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedTargetCollection, targetCollection);
            AssertDikeProfile(expectedDikeProfile, targetCollection[0]);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfilesAndImportedDataFullyOverlaps_UpdatesTargetCollection()
        {
            // Setup
            const string id = "Just an ID";
            DikeProfile targetDikeProfile = DikeProfileTestFactory.CreateDikeProfile("name", id);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection targetCollection = failureMechanism.DikeProfiles;
            targetCollection.AddRange(new[]
            {
                targetDikeProfile
            }, sourceFilePath);

            DikeProfile readDikeProfile = DeepCloneAndModify(targetDikeProfile);
            DikeProfile[] readDikeProfiles =
            {
                readDikeProfile
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(readDikeProfiles,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(targetDikeProfile, targetCollection[0]);
            AssertDikeProfile(readDikeProfile, targetDikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetCollection,
                targetDikeProfile
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfilesAndImportedDataHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string currentDikeProfileId = "Current ID";
            DikeProfile targetDikeProfile = DikeProfileTestFactory.CreateDikeProfile(string.Empty, currentDikeProfileId);

            const string readDikeProfileId = "Read ID";
            DikeProfile readDikeProfile = DikeProfileTestFactory.CreateDikeProfile(string.Empty, readDikeProfileId);
            DikeProfile[] readDikeProfiles =
            {
                readDikeProfile
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                targetDikeProfile
            }, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(readDikeProfiles,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, dikeProfiles.Count);
            Assert.AreSame(readDikeProfile, dikeProfiles[0]);

            CollectionAssert.AreEquivalent(new[]
            {
                dikeProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfilesAndImportedDataHasPartialOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string addedDikeProfileId = "ID A";
            const string removedDikeProfileId = "ID B";
            const string updatedDikeProfileId = "ID C";
            const string commonName = "Just a name for dike profile";

            DikeProfile dikeProfileToBeRemoved = DikeProfileTestFactory.CreateDikeProfile(commonName, removedDikeProfileId);
            DikeProfile dikeProfileToBeUpdated = DikeProfileTestFactory.CreateDikeProfile(commonName, updatedDikeProfileId);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                dikeProfileToBeRemoved,
                dikeProfileToBeUpdated
            }, sourceFilePath);

            DikeProfile dikeProfileToUpdateFrom = DeepCloneAndModify(dikeProfileToBeUpdated);
            DikeProfile dikeProfileToBeAdded = DikeProfileTestFactory.CreateDikeProfile(commonName, addedDikeProfileId);
            DikeProfile[] readDikeProfiles =
            {
                dikeProfileToBeAdded,
                dikeProfileToUpdateFrom
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(readDikeProfiles,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(2, dikeProfiles.Count);
            DikeProfile[] expectedDikeProfiles =
            {
                dikeProfileToBeAdded,
                dikeProfileToBeUpdated
            };
            CollectionAssert.AreEqual(expectedDikeProfiles, dikeProfiles);

            DikeProfile addedDikeProfile = dikeProfiles[0];
            Assert.AreSame(dikeProfileToBeAdded, addedDikeProfile);
            AssertDikeProfile(dikeProfileToBeAdded, addedDikeProfile);

            DikeProfile updatedDikeProfile = dikeProfiles[1];
            Assert.AreSame(dikeProfileToBeUpdated, updatedDikeProfile);
            AssertDikeProfile(dikeProfileToUpdateFrom, updatedDikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                dikeProfileToBeUpdated,
                dikeProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithOutputAssignedToRemovedProfile_UpdatesCalculation()
        {
            // Setup
            DikeProfile profileToBeRemoved = DikeProfileTestFactory.CreateDikeProfile("Removed profile", "removed profile ID");
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = profileToBeRemoved
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            DikeProfileCollection dikeProfileCollection = failureMechanism.DikeProfiles;
            dikeProfileCollection.AddRange(new[]
            {
                profileToBeRemoved
            }, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
                                                                                                   sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.DikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                dikeProfileCollection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_MultipleCalculationWithAssignedProfile_OnlyUpdatesCalculationWithUpdatedProfile()
        {
            // Setup
            DikeProfile affectedProfile = DikeProfileTestFactory.CreateDikeProfile("Profile to be updated", "ID of updated profile");
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = affectedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            const string unaffectedProfileName = "Unaffected Profile";
            const string unaffectedProfileId = "unaffected profile Id";
            DikeProfile unaffectedProfile = DikeProfileTestFactory.CreateDikeProfile(unaffectedProfileName, unaffectedProfileId);
            var unaffectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = unaffectedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                affectedProfile,
                unaffectedProfile
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);

            DikeProfile importedAffectedProfile = DeepCloneAndModify(affectedProfile);
            DikeProfile importedUnaffectedProfile = DikeProfileTestFactory.CreateDikeProfile(unaffectedProfileName, unaffectedProfileId);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                importedAffectedProfile,
                importedUnaffectedProfile
            }, sourceFilePath);
            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            DikeProfile inputParametersUnaffectedDikeProfile = unaffectedCalculation.InputParameters.DikeProfile;
            Assert.AreSame(unaffectedProfile, inputParametersUnaffectedDikeProfile);
            AssertDikeProfile(unaffectedProfile, inputParametersUnaffectedDikeProfile);

            Assert.IsFalse(affectedCalculation.HasOutput);
            DikeProfile inputParametersAffectedDikeProfile = affectedCalculation.InputParameters.DikeProfile;
            Assert.AreSame(affectedProfile, inputParametersAffectedDikeProfile);
            AssertDikeProfile(importedAffectedProfile, inputParametersAffectedDikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile,
                dikeProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_MultipleCalculationWithAssignedProfileOneRemovedProfile_OnlyUpdatesCalculationWithRemovedProfile()
        {
            // Setup
            DikeProfile removedProfile = DikeProfileTestFactory.CreateDikeProfile("Profile to be removed", "ID of removed profile");
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = removedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            const string unaffectedProfileName = "Unaffected Profile";
            const string unaffectedProfileId = "unaffected profile Id";
            DikeProfile unaffectedProfile = DikeProfileTestFactory.CreateDikeProfile(unaffectedProfileName, unaffectedProfileId);
            var unaffectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = unaffectedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                removedProfile,
                unaffectedProfile
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unaffectedCalculation);

            DikeProfile importedUnaffectedProfile = DikeProfileTestFactory.CreateDikeProfile(unaffectedProfileName, unaffectedProfileId);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                importedUnaffectedProfile
            }, sourceFilePath);
            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            DikeProfile inputParametersUnaffectedDikeProfile = unaffectedCalculation.InputParameters.DikeProfile;
            Assert.AreSame(unaffectedProfile, inputParametersUnaffectedDikeProfile);
            AssertDikeProfile(unaffectedProfile, inputParametersUnaffectedDikeProfile);

            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsNull(affectedCalculation.InputParameters.DikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                dikeProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithSameReference_OnlyReturnsDistinctCalculation()
        {
            // Setup
            DikeProfile affectedProfile = DikeProfileTestFactory.CreateDikeProfile("Profile to be updated", "ID of updated profile");
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = affectedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                affectedProfile
            }, sourceFilePath);

            DikeProfile profileToUpdateFrom = DeepCloneAndModify(affectedProfile);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                profileToUpdateFrom
            }, sourceFilePath);
            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                dikeProfiles,
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithDikeProfileUpdatedToOtherSegment_UpdatesSectionResults()
        {
            // Setup
            const string dikeProfileId = "ID of updated profile";

            var originalMatchingPoint = new Point2D(0, 0);
            var updatedMatchingPoint = new Point2D(20, 20);
            DikeProfile affectedProfile = DikeProfileTestFactory.CreateDikeProfile(originalMatchingPoint, dikeProfileId);
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = affectedProfile
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            var intersectionPoint = new Point2D(10, 10);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("OldSection", new[]
                {
                    originalMatchingPoint,
                    intersectionPoint
                }),
                new FailureMechanismSection("NewSection", new[]
                {
                    intersectionPoint,
                    updatedMatchingPoint
                })
            });

            GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                failureMechanism.SectionResults,
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>());

            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                affectedProfile
            }, sourceFilePath);

            // Precondition
            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults
                                                                                                     .ToArray();
            Assert.AreEqual(2, sectionResults.Length);
            Assert.AreSame(affectedCalculation, sectionResults[0].Calculation);
            Assert.IsNull(sectionResults[1].Calculation);

            DikeProfile profileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile(updatedMatchingPoint, dikeProfileId);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                profileToUpdateFrom
            }, sourceFilePath);
            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                dikeProfiles,
                affectedCalculation.InputParameters,
                affectedProfile,
                sectionResults[0],
                sectionResults[1]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(affectedCalculation, sectionResults[1].Calculation);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithToBeRemovedDikeProfile_UpdatesSectionResults()
        {
            // Setup
            const string dikeProfileId = "ID of removed profile";

            var matchingPoint = new Point2D(0, 0);
            DikeProfile removedProfile = DikeProfileTestFactory.CreateDikeProfile(matchingPoint, dikeProfileId);
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = removedProfile
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section", new[]
                {
                    matchingPoint,
                    new Point2D(10, 10)
                })
            });

            GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                failureMechanism.SectionResults,
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>());

            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                removedProfile
            }, sourceFilePath);

            // Precondition
            GrassCoverErosionInwardsFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults
                                                                                                     .ToArray();
            Assert.AreEqual(1, sectionResults.Length);
            Assert.AreSame(affectedCalculation, sectionResults[0].Calculation);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
                                                                                                   sourceFilePath);
            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                dikeProfiles,
                affectedCalculation.InputParameters,
                sectionResults[0]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(1, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithOutputAndForeshoreProfileUpdatedWithProfileWithoutGeometry_UpdatesCalculation()
        {
            // Setup
            const string id = "profile ID";
            IEnumerable<Point2D> geometry = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            DikeProfile affectedProfile = DikeProfileTestFactory.CreateDikeProfile(geometry, id);
            var affectedCalculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = affectedProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            affectedCalculation.InputParameters.UseForeshore = true;

            DikeProfile profileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile(Enumerable.Empty<Point2D>(), id);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        affectedCalculation
                    }
                }
            };

            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            DikeProfile[] originalDikeProfiles =
            {
                affectedProfile
            };
            dikeProfiles.AddRange(originalDikeProfiles, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateDikeProfilesWithImportedData(new[]
                                                            {
                                                                profileToUpdateFrom
                                                            },
                                                            sourceFilePath);

            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsFalse(affectedCalculation.InputParameters.UseForeshore);
            AssertDikeProfile(affectedProfile, profileToUpdateFrom);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile,
                dikeProfiles
            }, affectedObjects);
        }

        /// <summary>
        /// Makes a deep clone of the <paramref name="dikeProfile"/> and modifies 
        /// all its parameters, except the ID.
        /// </summary>
        /// <param name="dikeProfile">The <see cref="DikeProfile"/> to clone and 
        /// to modify.</param>
        /// <returns>A deep clone of <paramref name="dikeProfile"/> with modified
        /// parameters.</returns>
        private static DikeProfile DeepCloneAndModify(DikeProfile dikeProfile)
        {
            var random = new Random(21);

            Point2D originalWorldCoordinate = dikeProfile.WorldReferencePoint;
            var modifiedWorldCoordinate = new Point2D(originalWorldCoordinate.X + random.NextDouble(),
                                                      originalWorldCoordinate.Y + random.NextDouble());

            List<RoughnessPoint> modifiedDikeGeometry = dikeProfile.DikeGeometry.ToList();
            modifiedDikeGeometry.Add(new RoughnessPoint(new Point2D(1, 2), 3));

            List<Point2D> modifiedForeshoreGeometry = dikeProfile.ForeshoreGeometry.ToList();
            modifiedForeshoreGeometry.Add(new Point2D(1, 2));

            RoundedDouble originalBreakWaterHeight = dikeProfile.BreakWater?.Height ?? (RoundedDouble) 0.0;
            var modifiedBreakWater = new BreakWater(random.NextEnumValue<BreakWaterType>(),
                                                    originalBreakWaterHeight + random.NextDouble());

            string modifiedName = $"new_name_{dikeProfile.Name}";
            double modifiedDikeHeight = dikeProfile.DikeHeight + random.NextDouble();
            double modifiedOrientation = dikeProfile.Orientation + random.NextDouble();
            double modifiedX0 = dikeProfile.X0 + random.NextDouble();

            return new DikeProfile(modifiedWorldCoordinate, modifiedDikeGeometry,
                                   modifiedForeshoreGeometry, modifiedBreakWater,
                                   new DikeProfile.ConstructionProperties
                                   {
                                       Name = modifiedName,
                                       Id = dikeProfile.Id,
                                       DikeHeight = modifiedDikeHeight,
                                       Orientation = modifiedOrientation,
                                       X0 = modifiedX0
                                   });
        }

        private static void AssertDikeProfile(DikeProfile expectedDikeProfile, DikeProfile actualDikeProfile)
        {
            Assert.AreEqual(expectedDikeProfile.WorldReferencePoint, actualDikeProfile.WorldReferencePoint);
            CollectionAssert.AreEqual(expectedDikeProfile.ForeshoreGeometry, actualDikeProfile.ForeshoreGeometry);
            CollectionAssert.AreEqual(expectedDikeProfile.DikeGeometry, actualDikeProfile.DikeGeometry);
            Assert.AreEqual(expectedDikeProfile.BreakWater, actualDikeProfile.BreakWater);

            Assert.AreEqual(expectedDikeProfile.Id, actualDikeProfile.Id);
            Assert.AreEqual(expectedDikeProfile.Name, actualDikeProfile.Name);
            Assert.AreEqual(expectedDikeProfile.X0, actualDikeProfile.X0);
            Assert.AreEqual(expectedDikeProfile.Orientation, actualDikeProfile.Orientation);
            Assert.AreEqual(expectedDikeProfile.DikeHeight, actualDikeProfile.DikeHeight);
        }
    }
}