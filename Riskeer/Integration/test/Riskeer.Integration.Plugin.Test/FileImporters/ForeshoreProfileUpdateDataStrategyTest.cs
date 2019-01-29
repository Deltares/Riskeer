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
using Riskeer.Integration.Plugin.FileImporters;

namespace Riskeer.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path/to/foreshoreProfiles";

        private static IEnumerable<TestCaseData> DifferentForeshoreProfileWithSameId
        {
            get
            {
                var random = new Random(21);

                var defaultForeshoreProfile = new TestForeshoreProfile();

                string defaultId = defaultForeshoreProfile.Id;
                string defaultName = defaultForeshoreProfile.Name;
                RoundedPoint2DCollection defaultGeometry = defaultForeshoreProfile.Geometry;
                BreakWater defaultBreakWater = defaultForeshoreProfile.BreakWater;
                Point2D defaultReferencePoint = defaultForeshoreProfile.WorldReferencePoint;
                double defaultX0 = defaultForeshoreProfile.X0;
                RoundedDouble defaultOrientation = defaultForeshoreProfile.Orientation;

                yield return new TestCaseData(new TestForeshoreProfile("different name", defaultId))
                    .SetName("DifferentName");
                yield return new TestCaseData(new TestForeshoreProfile(new Point2D(defaultReferencePoint.X + random.NextDouble(),
                                                                                   defaultReferencePoint.Y + random.NextDouble())))
                    .SetName("DifferentWorldReferencePoint");
                yield return new TestCaseData(new TestForeshoreProfile(new BreakWater(random.NextEnumValue<BreakWaterType>(),
                                                                                      random.NextDouble())))
                    .SetName("DifferentBreakWater");
                yield return new TestCaseData(new TestForeshoreProfile(new[]
                    {
                        new Point2D(random.NextDouble(),
                                    random.NextDouble())
                    }))
                    .SetName("DifferentGeometry");
                yield return new TestCaseData(new ForeshoreProfile(defaultReferencePoint, defaultGeometry, defaultBreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Id = defaultId,
                                                                       Name = defaultName,
                                                                       Orientation = defaultOrientation,
                                                                       X0 = defaultX0 + random.NextDouble()
                                                                   }))
                    .SetName("DifferentX0");
                yield return new TestCaseData(new ForeshoreProfile(defaultReferencePoint, defaultGeometry, defaultBreakWater,
                                                                   new ForeshoreProfile.ConstructionProperties
                                                                   {
                                                                       Id = defaultId,
                                                                       Name = defaultName,
                                                                       Orientation = defaultOrientation + random.NextDouble(),
                                                                       X0 = defaultX0
                                                                   }))
                    .SetName("DifferentOrientation");
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfileUpdateDataStrategy(null, new ForeshoreProfileCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), new ForeshoreProfileCollection());

            // Assert
            Assert.IsInstanceOf<IForeshoreProfileUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<ForeshoreProfile, IFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), new ForeshoreProfileCollection());

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(null,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), new ForeshoreProfileCollection());

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(Enumerable.Empty<ForeshoreProfile>(),
                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(DifferentForeshoreProfileWithSameId))]
        public void UpdateForeshoreProfilesWithImportedData_ForeshoreProfilePropertiesChanged_UpdateRelevantProperties(
            ForeshoreProfile readForeshoreProfile)
        {
            // Setup
            var profileToBeUpdated = new TestForeshoreProfile();

            var targetCollection = new ForeshoreProfileCollection();
            targetCollection.AddRange(new[]
            {
                profileToBeUpdated
            }, sourceFilePath);

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), targetCollection);

            // Call
            strategy.UpdateForeshoreProfilesWithImportedData(new[]
            {
                readForeshoreProfile
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(profileToBeUpdated, targetCollection[0]);
            AssertForeshoreProfile(readForeshoreProfile, profileToBeUpdated);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionEmptyImportedCollectionContainDuplicateIDs_ThrowUpdateException()
        {
            // Setup 
            var foreshoreProfiles = new ForeshoreProfileCollection();

            const string duplicateId = "duplicate ID";
            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Name A", duplicateId),
                new TestForeshoreProfile("Name B", duplicateId)
            };

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WithCurrentCollectionNotEmptyAndImportedCollectionHasProfilesWithSameId_ThrowsUpdateException()
        {
            // Setup 

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

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            // Call
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(originalForeshoreProfiles, foreshoreProfiles);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CurrentCollectionAndImportedCollectionHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup 
            const string currentForeshoreProfile = "Current ID";
            var targetForeshoreProfile = new TestForeshoreProfile(string.Empty, currentForeshoreProfile);

            const string readForeshoreProfileId = "Read ID";
            var readForeshoreProfile = new TestForeshoreProfile(string.Empty, readForeshoreProfileId);
            TestForeshoreProfile[] readForeshoreProfiles =
            {
                readForeshoreProfile
            };

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                targetForeshoreProfile
            }, sourceFilePath);

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(readForeshoreProfiles,
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

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var targetForeshoreProfile = new TestForeshoreProfile("Name", "ID");
            foreshoreProfiles.AddRange(new[]
            {
                targetForeshoreProfile
            }, sourceFilePath);

            ForeshoreProfile readForeshoreProfile = DeepCloneAndModify(targetForeshoreProfile);
            ForeshoreProfile[] importedForeshoreProfiles =
            {
                readForeshoreProfile
            };

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(importedForeshoreProfiles,
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
            var commonName = "Name";
            var foreshoreProfileToBeUpdated = new TestForeshoreProfile(commonName, "Updated ID");
            var foreshoreProfileToBeRemoved = new TestForeshoreProfile(commonName, "Removed ID");

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfileToBeUpdated,
                foreshoreProfileToBeRemoved
            }, sourceFilePath);

            ForeshoreProfile foreshoreProfileToUpdateFrom = DeepCloneAndModify(foreshoreProfileToBeUpdated);
            var foreshoreProfileToBeAdded = new TestForeshoreProfile(commonName, "Added ID");
            ForeshoreProfile[] importedForeshoreProfiles =
            {
                foreshoreProfileToUpdateFrom,
                foreshoreProfileToBeAdded
            };

            var strategy = new ForeshoreProfileUpdateDataStrategy(new TestFailureMechanism(), foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(importedForeshoreProfiles,
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

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CalculationWithOutputAssignedToRemovedProfile_UpdatesCalculation()
        {
            // Setup
            var profileToBeRemoved = new TestForeshoreProfile("Name", "Removed ID");
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(profileToBeRemoved);
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                profileToBeRemoved
            }, sourceFilePath);

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism, foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(Enumerable.Empty<ForeshoreProfile>(),
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);

            CollectionAssert.IsEmpty(foreshoreProfiles);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_MultipleCalculationsWithOutputAndProfile_UpdatesCalculationWithUpdatedProfile()
        {
            // Setup
            var affectedProfile = new TestForeshoreProfile("Name", "Updated ID");
            TestCalculationWithForeshoreProfile affectedCalculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(affectedProfile);
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(affectedProfile);

            const string unaffectedProfileId = "Unaffected ID";
            const string unaffectedProfileName = "Name";
            var unaffectedProfile = new TestForeshoreProfile(unaffectedProfileName, unaffectedProfileId);
            TestCalculationWithForeshoreProfile unaffectedCalculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(unaffectedProfile);

            var foreshoreProfiles = new ForeshoreProfileCollection();
            TestForeshoreProfile[] originalForeshoreProfiles =
            {
                affectedProfile,
                unaffectedProfile
            };
            foreshoreProfiles.AddRange(originalForeshoreProfiles, sourceFilePath);

            var failureMechanism = new TestFailureMechanism(new[]
            {
                affectedCalculation,
                unaffectedCalculation
            });

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism, foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(new[]
                                                                 {
                                                                     new TestForeshoreProfile(unaffectedProfileName,
                                                                                              unaffectedProfileId),
                                                                     profileToUpdateFrom
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(unaffectedCalculation.HasOutput);
            Assert.AreSame(unaffectedProfile,
                           unaffectedCalculation.InputParameters.ForeshoreProfile);

            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.AreSame(affectedProfile, affectedCalculation.InputParameters.ForeshoreProfile);

            CollectionAssert.AreEquivalent(originalForeshoreProfiles, foreshoreProfiles);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile,
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_MultipleCalculationsWithSameReference_OnlyReturnsDistinctCalculations()
        {
            // Setup
            var affectedProfile = new TestForeshoreProfile("Name", "Updated ID");
            TestCalculationWithForeshoreProfile affectedCalculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(affectedProfile);
            ForeshoreProfile profileToUpdateFrom = DeepCloneAndModify(affectedProfile);

            TestCalculationWithForeshoreProfile calculationSameReference = affectedCalculation;

            var foreshoreProfiles = new ForeshoreProfileCollection();
            TestForeshoreProfile[] originalForeshoreProfiles =
            {
                affectedProfile
            };
            foreshoreProfiles.AddRange(originalForeshoreProfiles, sourceFilePath);

            var failureMechanism = new TestFailureMechanism(new[]
            {
                affectedCalculation,
                calculationSameReference
            });

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism, foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(new[]
                                                                 {
                                                                     profileToUpdateFrom
                                                                 },
                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile,
                foreshoreProfiles
            }, affectedObjects);
        }

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_CalculationWithOutputAndForeshoreProfileUpdatedWithProfileWithoutGeometry_UpdatesCalculation()
        {
            // Setup
            const string id = "profile ID";
            IEnumerable<Point2D> geometry = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var affectedProfile = new TestForeshoreProfile(id, geometry);
            TestCalculationWithForeshoreProfile affectedCalculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(affectedProfile);
            affectedCalculation.InputParameters.UseForeshore = true;

            var profileToUpdateFrom = new TestForeshoreProfile(id, Enumerable.Empty<Point2D>());

            var foreshoreProfiles = new ForeshoreProfileCollection();
            TestForeshoreProfile[] originalForeshoreProfiles =
            {
                affectedProfile
            };
            foreshoreProfiles.AddRange(originalForeshoreProfiles, sourceFilePath);

            var failureMechanism = new TestFailureMechanism(new[]
            {
                affectedCalculation
            });

            var strategy = new ForeshoreProfileUpdateDataStrategy(failureMechanism, foreshoreProfiles);

            // Call
            IEnumerable<IObservable> affectedObjects =
                strategy.UpdateForeshoreProfilesWithImportedData(new[]
                                                                 {
                                                                     profileToUpdateFrom
                                                                 },
                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsFalse(affectedCalculation.InputParameters.UseForeshore);
            AssertForeshoreProfile(affectedProfile, profileToUpdateFrom);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                affectedProfile,
                foreshoreProfiles
            }, affectedObjects);
        }

        /// <summary>
        /// Makes a deep clone of the foreshore profile and modifies all the properties,
        /// except the <see cref="ForeshoreProfile.Id"/>.
        /// </summary>
        /// <param name="foreshoreProfile">The foreshore profile to deep clone.</param>
        /// <returns>A deep clone of the <paramref name="foreshoreProfile"/>.</returns>
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