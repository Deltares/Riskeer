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
using Core.Common.Base.Geometry;
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
        public void UpdateDikeProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

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
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfDataCollection()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            DikeProfileCollection targetCollection = failureMechanism.DikeProfiles;

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);
            const string newSourcePath = "some/other/path";

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
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
            DikeProfile[] importedDikeProfiles =
            {
                DikeProfileTestFactory.CreateDikeProfile()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(importedDikeProfiles,
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
            DikeProfile targetDikeProfile = DikeProfileTestFactory.CreateDikeProfile("Name A", "Name A ID");
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                targetDikeProfile
            }, sourceFilePath);

            DikeProfile readDikeProfile = DikeProfileTestFactory.CreateDikeProfile("Name B", "Name B ID");

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(new[]
            {
                readDikeProfile
            }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.DikeProfiles
            }, affectedObjects);

            DikeProfile[] expectedDikeProfiles =
            {
                readDikeProfile
            };
            CollectionAssert.AreEqual(expectedDikeProfiles, failureMechanism.DikeProfiles);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_ImportedDataContainsDuplicateIDs_ThrowsUpdateException()
        {
            // Setup
            const string duplicateId = "Duplicate ID it is";
            DikeProfile[] importedSurfaceLines =
            {
                DikeProfileTestFactory.CreateDikeProfile("Naam A", duplicateId),
                DikeProfileTestFactory.CreateDikeProfile("Naam B", duplicateId)
            };

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(new GrassCoverErosionInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateDikeProfilesWithImportedData(importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = $"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void UpdateDikeProfilesWithImportedData_CalculationWithOutputAndDikeProfile_CalculationUpdatedAndReturnsAffectedObjects()
        {
            // Setup
            DikeProfile existingDikeProfile = DikeProfileTestFactory.CreateDikeProfile("test", "ID1");

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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateDikeProfilesWithImportedData(Enumerable.Empty<DikeProfile>(),
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

            failureMechanism.AddSections(new[]
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

            var strategy = new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(failureMechanism);

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
    }
}