﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporters;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.FileImporters
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
        public void UpdateDikeProfilesWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(null,
                                                                                  Enumerable.Empty<DikeProfile>(),
                                                                                  string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(new DikeProfileCollection(),
                                                                                  null,
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
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(new DikeProfileCollection(),
                                                                                  Enumerable.Empty<DikeProfile>(),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CurrentCollectionAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var targetCollection = new DikeProfileCollection();
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<DikeProfile>(),
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_DikeProfilePropertiesChanged_UpdateRelevantProperties()
        {
            // Setup
            var dikeProfileToUpdate = new TestDikeProfile("name", "ID A");
            DikeProfile dikeProfileToUpdateFrom = DeepCloneAndModify(dikeProfileToUpdate);

            var targetCollection = new DikeProfileCollection();
            targetCollection.AddRange(new[]
            {
                dikeProfileToUpdate
            }, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                        new[]
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
            var dikeProfileOne = new TestDikeProfile("name one", duplicateId);
            var dikeProfileTwo = new TestDikeProfile("Another dike profile", duplicateId);

            var targetCollection = new DikeProfileCollection();
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                  new[]
                                                                                  {
                                                                                      dikeProfileOne,
                                                                                      dikeProfileTwo
                                                                                  }, sourceFilePath);

            // Assert
            var exception = Assert.Throws<DikeProfileUpdateException>(call);

            string expectedMessage = "Het bijwerken van de dijkprofielen is mislukt: " +
                                     $"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CurrentCollectionEmptyAndImportedCollectionNotEmpty_NewProfilesAdded()
        {
            // Setup 
            var dikeProfileOne = new TestDikeProfile(string.Empty, "ID One");
            var dikeProfileTwo = new TestDikeProfile(string.Empty, "ID Two");
            var importedDataCollection = new[]
            {
                dikeProfileOne,
                dikeProfileTwo
            };

            var targetCollection = new DikeProfileCollection();
            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                                   importedDataCollection,
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedDataCollection, targetCollection);
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfileAndImportedMultipleDikeProfilesWithSameId_ThrowsUpdateException()
        {
            // Setup
            const string duplicateId = "A duplicated ID";
            var expectedDikeProfile = new TestDikeProfile("expectedName", duplicateId);

            var targetCollection = new DikeProfileCollection();
            var expectedTargetCollection = new[]
            {
                expectedDikeProfile
            };
            targetCollection.AddRange(expectedTargetCollection, sourceFilePath);

            var importedTargetCollection = new[]
            {
                DeepCloneAndModify(expectedDikeProfile),
                DeepCloneAndModify(expectedDikeProfile)
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                  importedTargetCollection,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<DikeProfileUpdateException>(call);
            const string expectedMessage = "Het bijwerken van de dijkprofielen is mislukt: " +
                                           "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);

            CollectionAssert.AreEqual(expectedTargetCollection, targetCollection);
            AssertDikeProfile(expectedDikeProfile, targetCollection[0]);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfilesAndImportedDataEmpty_RemovesDikeProfiles()
        {
            // Setup   
            var dikeProfile = new TestDikeProfile();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfileCollection = failureMechanism.DikeProfiles;
            dikeProfileCollection.AddRange(new[]
            {
                dikeProfile
            }, sourceFilePath);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfileCollection,
                                                                                                   Enumerable.Empty<DikeProfile>(),
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(dikeProfileCollection);
            CollectionAssert.AreEqual(new[]
            {
                dikeProfileCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_WithCurrentDikeProfilesAndImportedDataFullyOverlaps_UpdatesTargetCollection()
        {
            // Setup
            const string id = "Just an ID";
            var targetDikeProfile = new TestDikeProfile("name", id);
            var targetCollection = new DikeProfileCollection();
            targetCollection.AddRange(new[]
            {
                targetDikeProfile
            }, sourceFilePath);

            DikeProfile readDikeProfile = DeepCloneAndModify(targetDikeProfile);
            var readDikeProfiles = new[]
            {
                readDikeProfile
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                                   readDikeProfiles,
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
            var targetDikeProfile = new TestDikeProfile(string.Empty, currentDikeProfileId);

            const string readDikeProfileId = "Read ID";
            var readDikeProfile = new TestDikeProfile(string.Empty, readDikeProfileId);
            var readDikeProfiles = new[]
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfiles,
                                                                                                   readDikeProfiles,
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

            var dikeProfileToBeRemoved = new TestDikeProfile(commonName, removedDikeProfileId);
            var dikeProfileToBeUpdated = new TestDikeProfile(commonName, updatedDikeProfileId);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection dikeProfiles = failureMechanism.DikeProfiles;
            dikeProfiles.AddRange(new[]
            {
                dikeProfileToBeRemoved,
                dikeProfileToBeUpdated
            }, sourceFilePath);

            DikeProfile dikeProfileToUpdateFrom = DeepCloneAndModify(dikeProfileToBeUpdated);
            var dikeProfileToBeAdded = new TestDikeProfile(commonName, addedDikeProfileId);
            var readDikeProfiles = new[]
            {
                dikeProfileToBeAdded,
                dikeProfileToUpdateFrom
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfiles,
                                                                                                   readDikeProfiles,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(2, dikeProfiles.Count);
            var expectedDikeProfiles = new[]
            {
                dikeProfileToBeUpdated,
                dikeProfileToBeAdded
            };
            CollectionAssert.AreEqual(expectedDikeProfiles, dikeProfiles);

            DikeProfile updatedDikeProfile = dikeProfiles[0];
            Assert.AreSame(dikeProfileToBeUpdated, updatedDikeProfile);
            AssertDikeProfile(dikeProfileToUpdateFrom, updatedDikeProfile);

            DikeProfile addedDikeProfile = dikeProfiles[1];
            Assert.AreSame(dikeProfileToBeAdded, addedDikeProfile);
            AssertDikeProfile(dikeProfileToBeAdded, addedDikeProfile);

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
            var profileToBeRemoved = new TestDikeProfile("Removed profile", "removed profile ID");
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfileCollection,
                                                                                                   Enumerable.Empty<DikeProfile>(),
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
            var affectedProfile = new TestDikeProfile("Profile to be updated", "ID of updated profile");
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
            var unaffectedProfile = new TestDikeProfile(unaffectedProfileName, unaffectedProfileId);
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
            var importedUnaffectedProfile = new TestDikeProfile(unaffectedProfileName, unaffectedProfileId);

            var strategy = new GrassCoverErosionInwardsDikeProfileUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfiles,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedAffectedProfile,
                                                                                                       importedUnaffectedProfile
                                                                                                   }, sourceFilePath);
            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            DikeProfile inputParametersUnaffectedDikeProfile = unaffectedCalculation.InputParameters.DikeProfile;
            Assert.AreSame(unaffectedProfile, inputParametersUnaffectedDikeProfile);
            AssertDikeProfile(unaffectedProfile, inputParametersUnaffectedDikeProfile);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
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
        public void UpdateDikeProfilesWithImportedData_CalculationWithSameReference_OnlyReturnsDistinctCalculation()
        {
            // Setup
            var affectedProfile = new TestDikeProfile("Profile to be updated", "ID of updated profile");
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(dikeProfiles,
                                                                                                   new[]
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