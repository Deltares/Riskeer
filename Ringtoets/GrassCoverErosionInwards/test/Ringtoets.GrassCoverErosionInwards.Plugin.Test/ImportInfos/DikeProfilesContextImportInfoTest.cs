﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class DikeProfilesContextImportInfoTest : NUnitFormTest
    {
        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                string name = importInfo.Name;

                // Assert
                Assert.AreEqual("Dijkprofiellocaties", name);
            }
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
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
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                Image image = importInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.DikeProfile, image);
            }
        }

        [Test]
        public void IsEnabled_ReferenceLineSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            using (var plugin = new GrassCoverErosionInwardsPlugin())
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
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            using (var plugin = new GrassCoverErosionInwardsPlugin())
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
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
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
            mocks.ReplayAll();

            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                plugin.Gui = gui;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

                var dikeProfiles = new DikeProfileCollection();
                var context = new DikeProfilesContext(dikeProfiles, failureMechanism, assessmentSection);

                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                bool updatesVerified = importInfo.VerifyUpdates(context);

                // Assert
                Assert.IsTrue(updatesVerified);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnsExpectedInquiryMessage(bool isActionConfirmed)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                plugin.Gui = gui;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
                {
                    Output = new TestGrassCoverErosionInwardsOutput()
                });

                var dikeProfiles = new DikeProfileCollection();
                var context = new DikeProfilesContext(dikeProfiles, failureMechanism, assessmentSection);

                ImportInfo importInfo = GetImportInfo(plugin);

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

                // Call
                bool updatesVerified = importInfo.VerifyUpdates(context);

                // Assert
                string expectedInquiryMessage = "Als u dijkprofielen importeert, " +
                                                "dan worden alle rekenresultaten van dit toetsspoor " +
                                                $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
                Assert.AreEqual(isActionConfirmed, updatesVerified);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CreateFileImporter_Always_ReturnsFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            assessmentSection.ReferenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var context = new DikeProfilesContext(failureMechanism.DikeProfiles, failureMechanism, assessmentSection);

            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                ImportInfo importInfo = GetImportInfo(plugin);

                // Call
                IFileImporter importer = importInfo.CreateFileImporter(context, string.Empty);

                // Assert
                Assert.IsInstanceOf<DikeProfilesImporter>(importer);
                mocks.VerifyAll();
            }
        }

        private static ImportInfo GetImportInfo(PluginBase plugin)
        {
            return plugin.GetImportInfos().First(ii => ii.DataType == typeof(DikeProfilesContext));
        }
    }
}