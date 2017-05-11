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

using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Integration.Plugin.Properties;

namespace Ringtoets.Integration.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class ForeshoreProfileContextUpdateInfoTest
    {
        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            var foreshoreProfiles = new ForeshoreProfileCollection();

            var importTarget = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "test");

                // Assert
                Assert.IsInstanceOf<ProfilesImporter<ForeshoreProfileCollection>>(importer);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string name = updateInfo.Name;

                // Assert
                Assert.AreEqual("Voorlandprofiellocaties", name);
            }
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string category = updateInfo.Category;

                // Assert
                Assert.AreEqual("Algemeen", category);
            }
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                Image image = updateInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.Foreshore, image);
            }
        }

        [Test]
        public void IsEnabled_ReferenceLineSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();

            var context = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineNotSet_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();

            var context = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void CurrentPath_DikeProfileCollectionHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            const string expectedFilePath = "some/path";
            var surfaceLines = new ForeshoreProfileCollection();
            surfaceLines.AddRange(new[]
            {
                new TestForeshoreProfile(), 
            }, expectedFilePath);

            var context = new ForeshoreProfilesContext(surfaceLines, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string currentFilePath = updateInfo.CurrentPath(context);

                // Assert
                Assert.AreEqual(expectedFilePath, currentFilePath);
                mocks.VerifyAll();
            }
        }

        private static UpdateInfo GetUpdateInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ii => ii.DataType == typeof(ForeshoreProfilesContext));
        }
    }
}
