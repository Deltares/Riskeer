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
    public class GrassCoverErosionInwardsDikeProfileReplaceDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ReplaceDataStrategyBase<DikeProfile, GrassCoverErosionInwardsFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IDikeProfileUpdateDataStrategy>(strategy);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

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
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

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
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(new DikeProfileCollection(),
                                                                                  Enumerable.Empty<DikeProfile>(),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfDataCollection()
        {
            // Setup
            var targetCollection = new DikeProfileCollection();

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());
            const string newSourcePath = "some/other/path";

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<DikeProfile>(),
                                                                                                   newSourcePath);
            // Assert
            Assert.AreEqual(newSourcePath, targetCollection.SourcePath);
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection
            }, affectedObjects);
            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CollectionEmptyAndImportedCollectionNotEmpty_AddsNewDikeProfiles()
        {
            // Setup
            var importedDikeProfiles = new[]
            {
                new TestDikeProfile()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(failureMechanism.DikeProfiles,
                                                                                                   importedDikeProfiles,
                                                                                                   sourceFilePath);

            // Assert
            DikeProfileCollection actualCollection = failureMechanism.DikeProfiles;
            CollectionAssert.AreEqual(importedDikeProfiles, actualCollection);
            CollectionAssert.AreEqual(new[]
            {
                actualCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CollectionAndImportedDataCollectionNotEmpty_ReplacesCurrentWithImportedData()
        {
            // Setup
            var targetDikeProfile = new TestDikeProfile("Name A", "Name A ID");
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                targetDikeProfile
            }, sourceFilePath);

            var readDikeProfile = new TestDikeProfile("Name B", "Name B ID");

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(
                failureMechanism.DikeProfiles,
                new[]
                {
                    readDikeProfile
                }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.DikeProfiles
            }, affectedObjects);

            var expectedDikeProfiles = new[]
            {
                readDikeProfile
            };
            CollectionAssert.AreEqual(expectedDikeProfiles, failureMechanism.DikeProfiles);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_ImportedDataContainsDuplicateIDs_ThrowsUpdateException()
        {
            // Setup
            var targetCollection = new DikeProfileCollection();

            const string duplicateId = "Duplicate ID it is";
            DikeProfile[] importedSurfaceLines =
            {
                new TestDikeProfile("Naam A", duplicateId),
                new TestDikeProfile("Naam B", duplicateId)
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<DikeProfileUpdateException>(call);
            string expectedMessage = "Het importeren van dijkprofielen is mislukt: " +
                                     $"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithOutputAndDikeProfile_CalculationUpdatedAndReturnsAffectedObjects()
        {
            // Setup
            var existingDikeProfile = new TestDikeProfile("test", "ID1");

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = existingDikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                existingDikeProfile
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(failureMechanism.DikeProfiles,
                                                                                                   Enumerable.Empty<DikeProfile>(),
                                                                                                   sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.DikeProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                failureMechanism.DikeProfiles
            }, affectedObjects);
        }
    }
}