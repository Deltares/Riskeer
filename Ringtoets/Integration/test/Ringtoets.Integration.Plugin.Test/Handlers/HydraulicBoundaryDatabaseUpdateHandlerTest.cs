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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Plugin.Handlers;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Handlers;
using Ringtoets.Integration.TestUtil;

namespace Ringtoets.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseUpdateHandler(null, duneLocationsReplacementHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DuneLocationsReplacementHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("duneLocationsReplacementHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            // Call
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Assert
            Assert.IsInstanceOf<IHydraulicBoundaryDatabaseUpdateHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(null, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            TestDelegate call = () => handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseNotLinked_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseSameVersion_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "version"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsFalse(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        public void IsConfirmationRequired_HydraulicBoundaryDatabaseLinkedAndReadDatabaseDifferentVersion_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = "some/file/path",
                Version = "1"
            };

            // Call
            bool confirmationRequired = handler.IsConfirmationRequired(database, ReadHydraulicBoundaryDatabaseTestFactory.Create());

            // Assert
            Assert.IsTrue(confirmationRequired);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_ClickDialog_ReturnTrueIfOkAndFalseIfCancel(bool clickOk)
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

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

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("U heeft een ander hydraulische belastingendatabase bestand geselecteerd. Als gevolg hiervan moet de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            TestDelegate call = () => handler.Update(null, ReadHydraulicBoundaryDatabaseTestFactory.Create(), ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), null, ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicBoundaryDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike), duneLocationsReplacementHandler);

            // Call
            TestDelegate call = () => handler.Update(new HydraulicBoundaryDatabase(), ReadHydraulicBoundaryDatabaseTestFactory.Create(), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_FilePathAndVersionSame_NothingUpdatesAndReturnsEmptyCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);
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
            mocks.VerifyAll();
        }

        [Test]
        public void Update_VersionSameAndFilePathNotSame_UpdatesFilePathAndReturnsChangedObjects()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);
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
            CollectionAssert.IsEmpty(changedObjects);
            Assert.AreEqual(filePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual("version", hydraulicBoundaryDatabase.Version);
            AssertHydraulicBoundaryLocationsAndCalculations(locations, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_DatabaseLinkedAndVersionNotSame_RemovesOldLocationsAndCalculations()
        {
            // Setup
            const string filePath = "some/file/path";
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

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                           .WhenCalled(invocation =>
                                           {
                                               CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations, (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[0]);
                                           });
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryDatabase.Locations);

            var duneLocations = new[]
            {
                new TestDuneLocation(),
                new TestDuneLocation()
            };
            assessmentSection.DuneErosion.SetDuneLocations(duneLocations);

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            HydraulicBoundaryLocation[] oldLocations = hydraulicBoundaryDatabase.Locations.ToArray();

            // Precondition
            Assert.IsTrue(hydraulicBoundaryDatabase.IsLinked());
            CollectionAssert.AreEqual(oldLocations, hydraulicBoundaryDatabase.Locations);
            AssertHydraulicBoundaryLocationsAndCalculations(oldLocations, assessmentSection);

            // Call
            handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
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
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(Arg<IEnumerable<HydraulicBoundaryLocation>>.Is.NotNull))
                                           .WhenCalled(invocation =>
                                           {
                                               CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations, (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[0]);
                                           });
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

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
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenDatabase_WhenUpdatingDataWithNewLocations_ThenChangedObjectsReturned(bool isLinked)
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            if (isLinked)
            {
                hydraulicBoundaryDatabase.FilePath = filePath;
                hydraulicBoundaryDatabase.Version = "1";
                hydraulicBoundaryDatabase.Locations.AddRange(new[]
                {
                    new TestHydraulicBoundaryLocation("old location 1"),
                    new TestHydraulicBoundaryLocation("old location 2"),
                    new TestHydraulicBoundaryLocation("old location 3")
                });
            }

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // Precondition
            Assert.AreEqual(isLinked, hydraulicBoundaryDatabase.IsLinked());

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Then
            CollectionAssert.AreEqual(new IObservable[]
            {
                hydraulicBoundaryDatabase.Locations,
                assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                assessmentSection.WaterLevelCalculationsForSignalingNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                assessmentSection.WaveHeightCalculationsForSignalingNorm,
                assessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.DuneErosion.DuneLocations,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm,
                assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm,
                assessmentSection.DuneErosion.CalculationsForLowerLimitNorm,
                assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm
            }, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationsWithLocation_WhenUpdatingDatabaseWithNewLocations_ThenCalculationOutputClearedAndChangedObjectsReturned()
        {
            // Given
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.Stub<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();

            ICalculation[] calculationsWithOutput = assessmentSection.GetFailureMechanisms()
                                                                     .SelectMany(fm => fm.Calculations)
                                                                     .Where(c => c.HasOutput)
                                                                     .ToArray();

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // When
            IEnumerable<IObservable> changedObjects = handler.Update(assessmentSection.HydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Then
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));
            CollectionAssert.IsSubsetOf(calculationsWithOutput, changedObjects);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_NoUpdateCalled_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_AfterUpdateCalledAndNoLocationsUpdated_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            mocks.ReplayAll();

            const string filePath = "some/file/path";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
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

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Precondition
            CollectionAssert.IsEmpty(changedObjects);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostUpdateActions_AfterUpdateCalledAndLocationsUpdated_Perform()
        {
            // Setup
            var mocks = new MockRepository();
            var duneLocationsReplacementHandler = mocks.StrictMock<IDuneLocationsReplacementHandler>();
            duneLocationsReplacementHandler.Expect(h => h.Replace(null)).IgnoreArguments();
            duneLocationsReplacementHandler.Expect(h => h.DoPostReplacementUpdates());
            mocks.ReplayAll();

            const string filePath = "old/file/path";
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

            var handler = new HydraulicBoundaryDatabaseUpdateHandler(assessmentSection, duneLocationsReplacementHandler);

            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabaseTestFactory.Create(),
                                                                     ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(), filePath);

            // Precondition
            CollectionAssert.IsNotEmpty(changedObjects);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
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