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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
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
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionNotEmptyImportedCollectionContainDuplicateIDs_ThrowUpdateException()
        {
            // Setup 
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism());

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("name 1", "different ID")
            }, sourceFilePath);

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
    }
}