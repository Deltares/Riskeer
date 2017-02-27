﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Importers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.Gis.IO.Test.Importers
{
    [TestFixture]
    public class FeatureBasedMapDataImporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");

            // Call
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, "");

            // Assert
            Assert.IsInstanceOf<FileImporterBase<MapDataCollection>>(importer);
        }

        [Test]
        public void Import_ShapefileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "I_dont_exist.shp");
            var mapDataCollection = new MapDataCollection("test");
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': het bestand of andere benodigde bestanden zijn niet gevonden.", path) + Environment.NewLine +
                                  "Er is geen kaartlaag geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_ShapeFileEmptyFile_CancelImportWithErrorMessage()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "EmptyFile.shp");
            var mapDataCollection = new MapDataCollection("test");
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = string.Format(@"Fout bij het lezen van bestand '{0}': kon geen geometrieën vinden in dit bestand.", path) + Environment.NewLine +
                                  "Er is geen kaartlaag geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_ShapeFileCorrupt_CancelImportWithErrorMessage()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "CorruptFile.shp");
            var mapDataCollection = new MapDataCollection("test");
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);

            // Call
            bool importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            const string message = @"Fout bij het lezen van bestand '{0}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
            var expectedMessage = string.Format(message, path) + Environment.NewLine +
                                  "Er is geen kaartlaag geïmporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_ShapeFileInUse_CancelImportWithErrorMessage()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Single_Point_with_ID.shp");
            var mapDataCollection = new MapDataCollection("test");
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);

            using (new StreamReader(new FileStream(path, FileMode.Open)))
            {
                // Call
                bool importSuccessful = true;
                Action call = () => importSuccessful = importer.Import();

                // Assert
                const string message = @"Fout bij het lezen van bestand '{0}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var expectedMessage = string.Format(message, path) + Environment.NewLine +
                                      "Er is geen kaartlaag geïmporteerd.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
                Assert.IsFalse(importSuccessful);
            }
        }

        [Test]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Single_PolyLine_with_ID.shp")]
        public void Import_ValidShapeFile_ImportDataOnMapDataCollection(string fileName)
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, fileName);
            var mapDataCollection = new MapDataCollection("test");
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);

            // Precondition
            CollectionAssert.IsEmpty(mapDataCollection.Collection);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            Assert.AreEqual(1, mapDataCollection.Collection.Count());
        }

        [Test]
        public void DoPostImportUpdates_ImportSuccessful_NotifiesMapDataCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Single_Point_with_ID.shp");
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Attach(observer);
            var importer = new FeatureBasedMapDataImporter(mapDataCollection, path);
            
            // Precondition
            Assert.IsTrue(importer.Import());

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll();
        }
    }
}