// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Integration.Plugin.Properties;

namespace Ringtoets.Integration.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class ForeshoreProfilesContextImportInfoTest
    {
        [Test]
        public void CreateFileImporter_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            ReferenceLine referenceLine = mocks.Stub<ReferenceLine>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var list = new ObservableList<ForeshoreProfile>();

            var importTarget = new ForeshoreProfilesContext(list, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                IFileImporter importer = importInfo.CreateFileImporter(importTarget, "test");

                // Assert
                Assert.IsInstanceOf<ProfilesImporter<ObservableList<ForeshoreProfile>>>(importer);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                string name = importInfo.Name;

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
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                string category = importInfo.Category;

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
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                Image image = importInfo.Image;

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

            var list = new ObservableList<ForeshoreProfile>();

            var context = new ForeshoreProfilesContext(list, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                bool isEnabled = importInfo.IsEnabled(context);

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

            var list = new ObservableList<ForeshoreProfile>();

            var context = new ForeshoreProfilesContext(list, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                bool isEnabled = importInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnExpectedFileFilter()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                FileFilterGenerator fileFilter = importInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilter.Filter);
            }
        }

        private static ImportInfo GetImportInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetImportInfos().First(ii => ii.DataType == typeof(ForeshoreProfilesContext));
        }
    }
}