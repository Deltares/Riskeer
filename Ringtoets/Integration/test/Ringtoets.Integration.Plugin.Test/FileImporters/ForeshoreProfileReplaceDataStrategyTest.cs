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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ForeshoreProfileReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path/to/foreshoreProfiles";

        private static IEnumerable<TestCaseData> SupportedFailureMechanisms
        {
            get
            {
                yield return new TestCaseData(new WaveImpactAsphaltCoverFailureMechanism()).SetName("WaveImpactAsphaltCover");
                yield return new TestCaseData(new GrassCoverErosionOutwardsFailureMechanism()).SetName("GrassCoverErosionOutwards");
                yield return new TestCaseData(new StabilityStoneCoverFailureMechanism()).SetName("StabilityStoneCover");
                yield return new TestCaseData(new HeightStructuresFailureMechanism()).SetName("HeightStructures");
                yield return new TestCaseData(new StabilityPointStructuresFailureMechanism()).SetName("StabilityPointStructures");
                yield return new TestCaseData(new ClosingStructuresFailureMechanism()).SetName("ClosingStructures");
            }
        }

        private static IEnumerable<TestCaseData> SupportedFailureMechanismsAndEmptyForeshoreProfileCollection
        {
            get
            {
                var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                waveImpactAsphaltCoverFailureMechanism.WaveConditionsCalculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation());
                yield return new TestCaseData(waveImpactAsphaltCoverFailureMechanism,
                                              waveImpactAsphaltCoverFailureMechanism.ForeshoreProfiles)
                    .SetName("WaveImpactAsphaltCover");

                var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                yield return new TestCaseData(grassCoverErosionOutwardsFailureMechanism,
                                              grassCoverErosionOutwardsFailureMechanism.ForeshoreProfiles)
                    .SetName("GrassCoverErosionOutwards");

                var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
                yield return new TestCaseData(stabilityStoneCoverFailureMechanism,
                                              stabilityStoneCoverFailureMechanism.ForeshoreProfiles)
                    .SetName("StabilityStoneCover");

                var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism();
                yield return new TestCaseData(heightStructuresFailureMechanism,
                                              heightStructuresFailureMechanism.ForeshoreProfiles)
                    .SetName("HeightStructures");

                var stabilityPointStructuresFailureMechanism = new StabilityPointStructuresFailureMechanism();
                yield return new TestCaseData(stabilityPointStructuresFailureMechanism,
                                              stabilityPointStructuresFailureMechanism.ForeshoreProfiles)
                    .SetName("StabilityPointStructures");

                var closingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();
                yield return new TestCaseData(closingStructuresFailureMechanism,
                                              closingStructuresFailureMechanism.ForeshoreProfiles)
                    .SetName("ClosingStructures");
            }
        }

        [Test]
        public void Constructor_UnsupportedFailureMechanism_ThrowsUnsupportedException()
        {
            // Call
            TestDelegate call = () => new ForeshoreProfileReplaceDataStrategy(new TestFailureMechanism());

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            Assert.AreEqual($"Can't apply this strategy for {typeof(TestFailureMechanism)}.", exception.Message);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call 
            TestDelegate call = () => new ForeshoreProfileReplaceDataStrategy(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void Constructor_SupportedFailureMechanism_CreatesNewInstance(IFailureMechanism failureMechanism)
        {
            // Call 
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ReplaceDataStrategyBase<ForeshoreProfile, IFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IForeshoreProfileUpdateDataStrategy>(strategy);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_TargetCollectionNull_ThrowsArgumentNullException(
            IFailureMechanism failureMechanism)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(null,
                                                                                       Enumerable.Empty<ForeshoreProfile>(),
                                                                                       "path");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("targetDataCollection", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException(
            IFailureMechanism failureMechanism)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(new ForeshoreProfileCollection(),
                                                                                       null,
                                                                                       "path");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException(
            IFailureMechanism failureMechanism)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(new ForeshoreProfileCollection(),
                                                                                       Enumerable.Empty<ForeshoreProfile>(),
                                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_ImportedDataContainsDuplicateIDs_ThrowsUpdateException(
            IFailureMechanism failureMechanism)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            var originalForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("original profile", "original ID")
            };
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(originalForeshoreProfiles, sourceFilePath);

            const string duplicateId = "Just a duplicate ID";
            var importedForeshoreProfiles = new[]
            {
                new TestForeshoreProfile("Profile 1", duplicateId),
                new TestForeshoreProfile("Profile 2", duplicateId)
            };

            // Call 
            TestDelegate call = () => strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                       importedForeshoreProfiles,
                                                                                       sourceFilePath);

            // Assert
            var exception = Assert.Throws<ForeshoreProfileUpdateException>(call);
            string expectedMessage = "Het importeren van voorlandprofielen is mislukt: " +
                                     $"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<UpdateDataException>(exception.InnerException);

            CollectionAssert.AreEqual(originalForeshoreProfiles, foreshoreProfiles);
        }

        [Test]
        [TestCaseSource(nameof(SupportedFailureMechanisms))]
        public void UpdateForeshoreProfilesWithImportedData_DifferentSourcePath_UpdatesSourcePathOfDataCollection(
            IFailureMechanism failureMechanism)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);
            var foreshoreProfiles = new ForeshoreProfileCollection();

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
        [TestCaseSource(nameof(SupportedFailureMechanismsAndEmptyForeshoreProfileCollection))]
        public void UpdateForeshoreProfilesWithImportedData_CollectionEmptyAndImportedCollectionNotEmpty_AddsNewForeshoreProfiles(
            IFailureMechanism failureMechanism, ForeshoreProfileCollection foreshoreProfiles)
        {
            // Setup
            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

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
        [TestCaseSource(nameof(SupportedFailureMechanismsAndEmptyForeshoreProfileCollection))]
        public void UpdateForeshoreProfilesWithImportedData_CollectionAndImportedCollectionNotEmpty_ReplaceCurrentWithImportedData(
            IFailureMechanism failureMechanism, ForeshoreProfileCollection foreshoreProfiles)
        {
            // Setup
            foreshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile("Profile 1", "ID 1")
            }, sourceFilePath);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

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
        [TestCaseSource(nameof(SupportedFailureMechanismsAndEmptyForeshoreProfileCollection))]
        public void UpdateForeshoreProfilesWithImportedData_CalculationsWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData(
            IFailureMechanism failureMechanism, ForeshoreProfileCollection foreshoreProfiles)
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

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

        #region Wave Impact Asphalt Cover

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_WaveImpactAsphaltCoverCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            WaveConditionsInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion

        #region Grass Cover Erosion Outwards

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_GrassCoverErosionOutwardsCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            WaveConditionsInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion

        #region Stability Stone Cover

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_StabilityStoneCoverCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            WaveConditionsInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion

        #region Height Structures

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_HeightStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new HeightStructuresFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            HeightStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion

        #region Stability Point Structures

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_StabilityPointStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            StabilityPointStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion

        #region Closing Structures

        [Test]
        public void UpdateForeshoreProfilesWithImportedData_ClosingStructuresCalculationWithForeshoreProfile_CalculationUpdatedAndReturnsAffectedData()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile("Profile 1", "ID 1");

            var failureMechanism = new ClosingStructuresFailureMechanism();
            ForeshoreProfileCollection foreshoreProfiles = failureMechanism.ForeshoreProfiles;
            foreshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, sourceFilePath);

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new ForeshoreProfileReplaceDataStrategy(failureMechanism);

            // Call 
            IEnumerable<IObservable> affectedObjects = strategy.UpdateForeshoreProfilesWithImportedData(foreshoreProfiles,
                                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                                        sourceFilePath);

            // Assert
            ClosingStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                foreshoreProfiles,
                inputParameters
            }, affectedObjects);
        }

        #endregion
    }
}