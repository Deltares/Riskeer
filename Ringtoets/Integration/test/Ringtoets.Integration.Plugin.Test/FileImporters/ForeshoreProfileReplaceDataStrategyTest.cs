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
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
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
    public class ForeshoreProfileReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path/to/foreshoreProfiles";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call 
            TestDelegate call = () => new ForeshoreProfileReplaceDataStrategy(null, new ForeshoreProfileCollection());

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfileCollectionNull_ThrowsArgumentNullException()
        {
            // Call 
            TestDelegate call = () => new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_SupportedFailureMechanism_CreatesNewInstance()
        {
            // Call 
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), new ForeshoreProfileCollection());

            // Assert
            Assert.IsInstanceOf<ReplaceDataStrategyBase<ForeshoreProfile, IFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IForeshoreProfileUpdateDataStrategy>(strategy);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_TargetDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var foreshoreProfileCollection = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfileCollection);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(null,
                                                                                       Enumerable.Empty<ForeshoreProfile>(),
                                                                                       "path");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var foreshoreProfileCollection = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfileCollection);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfileCollection,
                                                                                       null,
                                                                                       "path");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var foreshoreProfileCollection = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfileCollection);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfileCollection,
                                                                                       Enumerable.Empty<ForeshoreProfile>(),
                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ImportedDataContainsDuplicateIDs_ThrowsUpdateException()
        {
            // Setup
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            const string duplicateId = "Just a duplicate ID";
            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Profile 1", duplicateId),
                new TestForeshoreProfile("Profile 2", duplicateId)
            };

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles, importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<ForeshoreProfileUpdateException>(call);
            string expectedMessage = "Het importeren van voorlandprofielen is mislukt: " +
                                     $"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfDataCollection()
        {
            // Setup
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            const string newForeshoreProfilesPath = "new/path";

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        newForeshoreProfilesPath);

            // Assert
            Assert.AreEqual(newForeshoreProfilesPath, foreshoreProfiles.SourcePath);
            CollectionAssert.AreEqual(new IObservable[]
            {
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CollectionEmptyAndImportedCollectionNotEmpty_AddsNewForeshoreProfiles()
        {
            // Setup
            var foreshoreProfiles = new ForeshoreProfileCollection();
            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile()
            };

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        importedForeshoreProfiles,
                                                                                                        sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedForeshoreProfiles, foreshoreProfiles);
            CollectionAssert.AreEqual(new IObservable[]
            {
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CollectionAndImportedCollectionNotEmpty_ReplaceCurrentWithImportedData()
        {
            // Setup
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("Profile 1", "ID 1")
            }, sourceFilePath);

            var strategy = new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Profile 2", "ID 2")
            };

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        importedForeshoreProfiles,
                                                                                                        sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedForeshoreProfiles, foreshoreProfiles);
            CollectionAssert.AreEqual(new IObservable[]
            {
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CalculationsWithForeshoreProfilesAndOutput_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var calculationWithForeshoreProfileAndOutput = new TestCalculationWithForeshoreProfile(true)
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            var calculationWithForeshoreProfile = new TestCalculationWithForeshoreProfile(false)
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            var calculationWithoutForeshoreProfile = new TestCalculationWithForeshoreProfile(false);

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculationWithForeshoreProfileAndOutput,
                calculationWithForeshoreProfile,
                calculationWithoutForeshoreProfile
            });

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculationsWithForeshoreProfile = new[]
            {
                calculationWithForeshoreProfileAndOutput,
                calculationWithForeshoreProfile
            };

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism, foreshoreProfiles);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(foreshoreProfiles);
            Assert.IsFalse(calculationWithForeshoreProfileAndOutput.HasOutput);
            Assert.IsTrue(calculationsWithForeshoreProfile.All(calc => calc.InputParameters.ForeshoreProfile == null));

            IEnumerable<IObservable> expectedAffectedObjects =
                calculationsWithForeshoreProfile.Select(calc => calc.InputParameters)
                                                .Concat(new IObservable[]
                                                {
                                                    foreshoreProfiles,
                                                    calculationWithForeshoreProfileAndOutput
                                                });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        private class TestCalculationWithForeshoreProfile : Observable, ICalculation<TestCalculationInputWithForeshoreProfile>
        {
            public TestCalculationWithForeshoreProfile(bool hasOutput)
            {
                InputParameters = new TestCalculationInputWithForeshoreProfile();
                HasOutput = hasOutput;
            }

            public string Name { get; set; }
            public bool HasOutput { get; private set; }
            public Comment Comments { get; }

            public TestCalculationInputWithForeshoreProfile InputParameters { get; }

            public void ClearOutput()
            {
                HasOutput = false;
            }
        }

        private class TestCalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile
        {
            public ForeshoreProfile ForeshoreProfile { get; set; }

            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }
        }
    }
}