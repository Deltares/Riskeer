﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class ReferenceLineContextImportInfoTest
    {
        private ImportInfo importInfo;
        private RingtoetsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(ReferenceLineContext));
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
            Assert.AreEqual("Referentielijn", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ReferenceLineIcon, image);
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
        public void FileFilter_Always_ReturnExpectedFileFilter()
        {
            // Call
            string fileFilter = importInfo.FileFilter;

            // Assert
            Assert.AreEqual("ESRI Shapefile (*.shp)|*.shp", fileFilter);
        }

        [Test]
        public void CreateFileImporter_ValidInput_SuccesfullImport()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                  Path.Combine("ReferenceLine", "traject_10-2.shp"));

            var importTarget = new ReferenceLineContext(assessmentSection);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(importTarget);

            // Assert
            Assert.IsTrue(importer.Import(null, path));
            mocks.VerifyAll();
        }
    }
}