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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineContextUpdateInfoTest
    {
        private UpdateInfo updateInfo;
        private PipingPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            updateInfo = plugin.GetUpdateInfos().First(i => i.DataType == typeof(RingtoetsPipingSurfaceLinesContext));
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
            string name = updateInfo.Name;

            // Assert
            Assert.AreEqual("Profielschematisaties", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = updateInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = updateInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingSurfaceLineIcon, image);
        }

        [Test]
        public void IsEnabled_ReferenceLineNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new RingtoetsPipingSurfaceLineCollection();

            var context = new RingtoetsPipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new RingtoetsPipingSurfaceLineCollection();

            var context = new RingtoetsPipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnExpectedFileFilter()
        {
            // Call
            string fileFilter = updateInfo.FileFilter.Filter;

            // Assert
            Assert.AreEqual("Profielschematisaties Kommagescheiden bestand (*.csv)|*.csv", fileFilter);
        }

        [Test]
        public void CreateFileImporter_ValidInput_SuccessfulImport()
        {
            // Setup
            var filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                      Path.Combine("SurfaceLines", "TwoValidSurfaceLines.csv"));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(3.3, -1),
                new Point2D(3.3, 1),
                new Point2D(94270, 427775.65),
                new Point2D(94270, 427812.08)
            });

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new RingtoetsPipingSurfaceLineCollection();

            var updateTarget = new RingtoetsPipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);
            
            // Call
            IFileImporter importer = updateInfo.CreateFileImporter(updateTarget, filePath);

            // Assert
            Assert.IsTrue(importer.Import());
            mocks.VerifyAll();
        }
    }
}
