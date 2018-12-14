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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Handlers;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseUpdateHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDatabaseUpdateHandler>(handler);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(null, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void IsConfirmationRequired_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNotLinked_ReturnsFalse()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseSameVersion_ReturnsFalse()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "version"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseDifferentVersion_ReturnsTrue()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "1"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsTrue(confirmationRequired);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_ClickDialog_ReturnTrueIfOkAndFalseIfCancel(bool clickOk)
        {
            // Setup
            string dialogTitle = null, dialogMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogTitle = tester.Title;
                dialogMessage = tester.Text;
                if (clickOk)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("U heeft een ander hydraulische belastingendatabase bestand geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." + Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(null, ReadHydraulicBoundaryDatabaseTestFactory.Create(), ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), null, ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
        }

        [Test]
        public void Update_FilePathAndVersionSame_NothingUpdatesAndReturnsEmptyCollection()
        {
            // Setup
            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Version = "version",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            HydraulicBoundaryLocation[] locations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Precondition
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual("version", hydraulicBoundaryDatabase.Version);
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);
        }

        [Test]
        public void Update_VersionSameAndFilePathNotSame_UpdatesFilePathAndReturnsChangedObjects()
        {
            // Setup
            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "old/file/path",
                Version = "version",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            HydraulicBoundaryLocation[] locations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Precondition
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                hydraulicBoundaryDatabase
            }, changedObjects);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual("version", hydraulicBoundaryDatabase.Version);
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);
        }

        [Test]
        public void Update_DatabaseAlreadyLinked_RemovesOldLocationsAndCalculations()
        {
            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Version = "1",
                Locations =
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection);

            HydraulicBoundaryLocation[] oldLocations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());
            CollectionAssert.AreEqual(oldLocations, hydraulicBoundaryDatabase.Locations);
            AssertHydraulicBoundaryLocationsAndCalculations(oldLocations, assessmentSection);

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Assert
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.IsNotSubsetOf(oldLocations, hydraulicBoundaryDatabase.Locations);
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaveHeightCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.IsNotSubsetOf(oldLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
        }

        [Test]
        public void Update_DatabaseNotLinked_SetsAllData()
        {
            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection);

            // Precondition
            Assert.IsFalse(hydraulicBoundaryDatabase.IsLinked());

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = ReadHydraulicBoundaryDatabaseTestFactory.Create();

            // Call
            handler.Update(hydraulicBoundaryDatabase, readHydraulicBoundaryDatabase,
                           ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Assert
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(readHydraulicBoundaryDatabase.Version, hydraulicBoundaryDatabase.Version);

            AssertHydraulicBoundaryLocations(readHydraulicBoundaryDatabase.Locations, hydraulicBoundaryDatabase.Locations);
            AssertHydraulicBoundaryLocationsAndCalculations(hydraulicBoundaryDatabase.Locations, assessmentSection);
        }

        private static void AssertHydraulicBoundaryLocationsAndCalculations(IEnumerable<HydraulicBoundaryLocation> locations, AssessmentSection assessmentSection)
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaveHeightCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
            CollectionAssert.AreEqual(locations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
        }

        private static void AssertHydraulicBoundaryLocations(IEnumerable<ReadHydraulicBoundaryLocation> readLocations, ObservableList<HydraulicBoundaryLocation> actualLocations)
        {
            Assert.AreEqual(readLocations.Count(), actualLocations.Count);

            for (var i = 0; i < actualLocations.Count; i++)
            {
                ReadHydraulicBoundaryLocation readLocation = readLocations.ElementAt(i);
                HydraulicBoundaryLocation actualLocation = actualLocations.ElementAt(i);

                Assert.AreEqual(readLocation.Id, actualLocation.Id);
                Assert.AreEqual(readLocation.Name, actualLocation.Name);
                Assert.AreEqual(readLocation.CoordinateX, actualLocation.Location.X);
                Assert.AreEqual(readLocation.CoordinateY, actualLocation.Location.Y);
            }
        }
    }
}