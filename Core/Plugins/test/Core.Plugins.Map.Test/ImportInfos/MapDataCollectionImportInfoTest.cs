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

using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Importers;
using Core.Plugins.Map.Properties;
using NUnit.Framework;

namespace Core.Plugins.Map.Test.ImportInfos
{
    [TestFixture]
    public class MapDataCollectionImportInfoTest
    {
        private ImportInfo importInfo;
        private MapPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new MapPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(MapDataCollection));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Kaartlaag", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Kaartlaag", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.MapPlusIcon, image);
        }

        [Test]
        public void IsEnabled_Always_ReturnTrue()
        {
            // Call
            bool isEnabled = importInfo.IsEnabled(null);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilter = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilter.Filter);
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var importTarget = new MapDataCollection("test");

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(importTarget, "");

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataImporter>(importer);
        }
    }
}