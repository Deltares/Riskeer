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
using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Integration.Plugin.Properties;

namespace Riskeer.Integration.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class ForeshoreProfileContextUpdateInfoTest : NUnitFormTest
    {
        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

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
        public void IsEnabled_SourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(Enumerable.Empty<ForeshoreProfile>(),
                                       "path");

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
        public void IsEnabled_SourcePathNotSet_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
        public void CurrentPath_ForeshoreProfileCollectionHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            const string expectedFilePath = "some/path";
            var foreshoreProfiles = new ForeshoreProfileCollection();
            foreshoreProfiles.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, expectedFilePath);

            var context = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

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

        [Test]
        public void VerifyUpdates_CalculationWithoutOutputs_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var calculationWithoutOutput = mocks.Stub<ICalculation>();
            calculationWithoutOutput.Stub(calc => calc.HasOutput).Return(false);

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculationWithoutOutput
            });

            mocks.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var context = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool updatesVerified = updateInfo.VerifyUpdates(context);

                // Assert
                Assert.IsTrue(updatesVerified);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnInquiryMessage(bool isActionConfirmed)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            var calculationWithoutOutput = mocks.Stub<ICalculation>();
            calculationWithoutOutput.Stub(calc => calc.HasOutput).Return(true);

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new[]
            {
                calculationWithoutOutput
            });
            mocks.ReplayAll();

            var foreshoreProfiles = new ForeshoreProfileCollection();
            var context = new ForeshoreProfilesContext(foreshoreProfiles, failureMechanism, assessmentSection);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;

                if (isActionConfirmed)
                {
                    helper.ClickOk();
                }
                else
                {
                    helper.ClickCancel();
                }
            };

            using (var plugin = new RingtoetsPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool updatesVerified = updateInfo.VerifyUpdates(context);

                // Assert
                Assert.AreEqual(isActionConfirmed, updatesVerified);
                string expectedInquiryMessage = "Als voorlandprofielen wijzigen door het bijwerken, " +
                                                "dan worden de resultaten van berekeningen die deze voorlandprofielen gebruiken verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
            }

            mocks.VerifyAll();
        }

        private static UpdateInfo GetUpdateInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ui => ui.DataType == typeof(ForeshoreProfilesContext));
        }
    }
}