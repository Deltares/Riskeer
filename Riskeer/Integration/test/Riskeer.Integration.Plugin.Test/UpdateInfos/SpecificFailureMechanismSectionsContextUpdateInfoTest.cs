// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class SpecificFailureMechanismSectionsContextUpdateInfoTest
    {
        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new SpecificFailureMechanism();
            var importTarget = new SpecificFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "test");

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string name = updateInfo.Name;

                // Assert
                Assert.AreEqual("Vakindeling", name);
            }
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
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
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                Image image = updateInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }
        }

        [Test]
        public void IsEnabled_SourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new SpecificFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(),
                                         "path");

            var context = new SpecificFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new RiskeerPlugin())
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
        public void IsEnabled_SourcePathNotSet_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new SpecificFailureMechanism();
            var context = new SpecificFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new RiskeerPlugin())
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
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void CurrentPath_FailureMechanismSectionsHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string expectedFilePath = "path";
            var failureMechanism = new SpecificFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(),
                                         expectedFilePath);

            var context = new SpecificFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string currentFilePath = updateInfo.CurrentPath(context);

                // Assert
                Assert.AreEqual(expectedFilePath, currentFilePath);
                mocks.VerifyAll();
            }
        }

        private static UpdateInfo GetUpdateInfo(RiskeerPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ui => ui.DataType == typeof(SpecificFailureMechanismSectionsContext));
        }
    }
}