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
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Importers;

namespace Ringtoets.Integration.IO.Test.Importers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseImporterTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                 nameof(HydraulicLocationConfigurationDatabaseImporter));

        [Test]
        public void Constructor_UpdateHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), null,
                                                                                         new HydraulicBoundaryDatabase(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("updateHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler, null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            // Call
            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler,
                                                                              new HydraulicBoundaryDatabase(), "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<HydraulicLocationConfigurationSettings>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDifferentFromHydraulicBoundaryDatabasesFilePath_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string hydraulicBoundaryDatabasePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO,
                                                                                           nameof(HydraulicBoundaryDatabaseImporter)), "complete.sqlite");
            string path = Path.Combine(testDataPath, "hlcd.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabasePath
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler, hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptySchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "empty.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = path
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler, hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bevragen van de database is mislukt.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidSchema_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IHydraulicLocationConfigurationDatabaseUpdateHandler>();
            mocks.ReplayAll();

            string path = Path.Combine(testDataPath, "invalid.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = path,
                TrackId = 13
            };

            var importer = new HydraulicLocationConfigurationDatabaseImporter(new HydraulicLocationConfigurationSettings(), handler, hydraulicBoundaryDatabase, path);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{path}': kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.";
            AssertImportFailed(call, expectedMessage, ref importSuccessful);
            mocks.VerifyAll();
        }

        private static void AssertImportFailed(Action call, string errorMessage, ref bool importSuccessful)
        {
            string expectedMessage = $"{errorMessage}" +
                                     $"{Environment.NewLine}Er is geen HLCD geïmporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importSuccessful);
        }
    }
}