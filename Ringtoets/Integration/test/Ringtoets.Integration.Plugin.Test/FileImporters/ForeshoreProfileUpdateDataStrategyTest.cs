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
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path/to/foreshoreProfiles";

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IForeshoreProfileUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<ForeshoreProfile, IFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ForeshoreProfilePropertiesChanged_UpdateRelevantProperties()
        {
            // Setup
            var profileToBeUpdated = new TestForeshoreProfile("Name", "Profile ID");
            var profileToUpdateFrom = DeepCloneAndModify(profileToBeUpdated);

            var targetCollection = new ForeshoreProfileCollection();
            targetCollection.AddRange(new[]
            {
                profileToBeUpdated
            }, sourceFilePath);

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            // Call
            strategy.UpdateForeshoreProfilesWithImportedData(targetCollection,
                                                             new[]
                                                             {
                                                                 profileToUpdateFrom
                                                             }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(profileToBeUpdated, targetCollection[0]);
            AssertForeshoreProfile(profileToUpdateFrom, profileToBeUpdated);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());
            var foreshoreProfiles = new ForeshoreProfileCollection();

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionEmptyImportedCollectionContainDuplicateIDs_ThrowUpdateException()
        {
            // Setup 
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());
            var foreshoreProfiles = new ForeshoreProfileCollection();

            const string duplicateId = "duplicate ID";
            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Name A", duplicateId),
                new TestForeshoreProfile("Name B", duplicateId)
            };

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                       importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = $"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionEmptyAndImportedCollectionNotEmpty_NewProfilesAdded()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());
            var foreshoreProfiles = new ForeshoreProfileCollection();

            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Name A", "ID A"),
                new TestForeshoreProfile("Name B", "ID B")
            };

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                 importedForeshoreProfiles,
                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfiles
            }, affectedObjects);
            CollectionAssert.AreEqual(importedForeshoreProfiles, foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionNotEmptyAndImportedCollectionEmpty_RemovesProfiles()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("Name A", "ID A"),
                new TestForeshoreProfile("Name B", "ID B")
            }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                 Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfiles
            }, affectedObjects);
            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WithCurrentCollectionNotEmptyAndImportedCollectionHasProfilesWithSameId_ThrowsUpdateException()
        {
            // Setup 
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var originalForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("name 1", "different ID")
            };
            foreshoreProfiles.AddRange(originalForeshoreProfiles, sourceFilePath);

            const string duplicateId = "duplicate ID";
            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Name A", duplicateId),
                new TestForeshoreProfile("Name B", duplicateId)
            };

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                       importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = $"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionAndImportedCollectionHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup 
            const string currentForeshoreProfile = "Current ID";
            var targetForeshoreProfile = new TestForeshoreProfile(string.Empty, currentForeshoreProfile);

            const string readForeshoreProfileId = "Read ID";
            var readForeshoreProfile = new TestForeshoreProfile(string.Empty, readForeshoreProfileId);
            var readForeshoreProfiles = new[]
            {
                readForeshoreProfile
            };

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                targetForeshoreProfile
            }, sourceFilePath);

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                 readForeshoreProfiles,
                                                                 sourceFilePath);

            // Assert
            Assert.AreEqual(1, foreshoreProfiles.Count);
            Assert.AreSame(readForeshoreProfile, foreshoreProfiles[0]);

            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WithCurrentCollectionAndImportedCollectionHasFullOverlap_UpdatesTargetDataCollection()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var targetForeshoreProfile = new TestForeshoreProfile("Name", "ID");
            foreshoreProfiles.AddRange(new[]
            {
                targetForeshoreProfile
            }, sourceFilePath);

            var readForeshoreProfile = DeepCloneAndModify(targetForeshoreProfile);
            var importedForeshoreProfiles = new[]
            {
                readForeshoreProfile
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        importedForeshoreProfiles,
                                                                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(1, foreshoreProfiles.Count);
            Assert.AreSame(targetForeshoreProfile, foreshoreProfiles[0]);
            AssertForeshoreProfile(readForeshoreProfile, targetForeshoreProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetForeshoreProfile,
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WithCurrentCollectionAndImportedCollectionHasPartialOverlap_UpdatesTargetDataCollection()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            var commonName = "Name";
            var foreshoreProfileToBeUpdated = new TestForeshoreProfile(commonName, "Updated ID");
            var foreshoreProfileToBeRemoved = new TestForeshoreProfile(commonName, "Removed ID");

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfileToBeUpdated,
                foreshoreProfileToBeRemoved
            }, sourceFilePath);

            var foreshoreProfileToUpdateFrom = DeepCloneAndModify(foreshoreProfileToBeUpdated);
            var foreshoreProfileToBeAdded = new TestForeshoreProfile(commonName, "Added ID");
            var importedForeshoreProfiles = new[]
            {
                foreshoreProfileToUpdateFrom,
                foreshoreProfileToBeAdded
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        importedForeshoreProfiles,
                                                                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(2, foreshoreProfiles.Count);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfileToBeUpdated,
                foreshoreProfileToBeAdded
            }, foreshoreProfiles);

            ForeshoreProfile updatedForeshoreProfile = foreshoreProfiles[0];
            Assert.AreSame(foreshoreProfileToBeUpdated, updatedForeshoreProfile);
            AssertForeshoreProfile(foreshoreProfileToUpdateFrom, updatedForeshoreProfile);

            ForeshoreProfile addedForeshoreProfile = foreshoreProfiles[1];
            Assert.AreSame(foreshoreProfileToBeAdded, addedForeshoreProfile);
            AssertForeshoreProfile(foreshoreProfileToBeAdded, addedForeshoreProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfileToBeUpdated,
                foreshoreProfiles
            }, affectedObjects);
        }

        private static ForeshoreProfile DeepCloneAndModify(ForeshoreProfile foreshoreProfile)
        {
            var random = new Random(21);

            Point2D originalWorldCoordinate = foreshoreProfile.WorldReferencePoint;
            var modifiedWorldCoordinate = new Point2D(originalWorldCoordinate.X + random.NextDouble(),
                                                      originalWorldCoordinate.Y + random.NextDouble());

            List<Point2D> modifiedForeshoreGeometry = foreshoreProfile.Geometry.ToList();
            modifiedForeshoreGeometry.Add(new Point2D(1, 2));

            RoundedDouble originalBreakWaterHeight = foreshoreProfile.BreakWater?.Height ?? (RoundedDouble) 0.0;
            var modifiedBreakWater = new BreakWater(random.NextEnumValue<BreakWaterType>(),
                                                    originalBreakWaterHeight + random.NextDouble());

            string modifiedName = $"new_name_{foreshoreProfile.Name}";
            double modifiedOrientation = foreshoreProfile.Orientation + random.NextDouble();
            double modifiedX0 = foreshoreProfile.X0 + random.NextDouble();

            return new ForeshoreProfile(modifiedWorldCoordinate, modifiedForeshoreGeometry,
                                        modifiedBreakWater,
                                        new ForeshoreProfile.ConstructionProperties
                                        {
                                            Name = modifiedName,
                                            Id = foreshoreProfile.Id,
                                            Orientation = modifiedOrientation,
                                            X0 = modifiedX0
                                        });
        }

        private static void AssertForeshoreProfile(ForeshoreProfile expectedForeshoreProfile,
                                                   ForeshoreProfile actualForeshoreProfile)
        {
            Assert.AreEqual(expectedForeshoreProfile.WorldReferencePoint, actualForeshoreProfile.WorldReferencePoint);
            CollectionAssert.AreEqual(expectedForeshoreProfile.Geometry, actualForeshoreProfile.Geometry);
            Assert.AreEqual(expectedForeshoreProfile.BreakWater, actualForeshoreProfile.BreakWater);

            Assert.AreEqual(expectedForeshoreProfile.Id, actualForeshoreProfile.Id);
            Assert.AreEqual(expectedForeshoreProfile.Name, actualForeshoreProfile.Name);
            Assert.AreEqual(expectedForeshoreProfile.X0, actualForeshoreProfile.X0);
            Assert.AreEqual(expectedForeshoreProfile.Orientation, actualForeshoreProfile.Orientation);
        }
    }
}