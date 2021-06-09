﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Primitives;
using PipingFormsResources = Riskeer.Piping.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class PipingSurfaceLinesContextUpdateInfoTest : NUnitFormTest
    {
        private UpdateInfo updateInfo;
        private PipingPlugin plugin;

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
        public void IsEnabled_SurfaceLineCollectionSourcePathNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new PipingSurfaceLineCollection();

            var context = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_SurfaceLineCollectionSourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new PipingSurfaceLineCollection();
            surfaceLines.AddRange(Enumerable.Empty<PipingSurfaceLine>(), "some/path");

            var context = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            string fileFilter = updateInfo.FileFilterGenerator.Filter;

            // Assert
            Assert.AreEqual("Profielschematisaties Kommagescheiden bestand (*.csv)|*.csv", fileFilter);
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

            plugin.Gui = gui;

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario());

            var surfaceLines = new PipingSurfaceLineCollection();
            var context = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            bool updatesVerified = updateInfo.VerifyUpdates(context);

            // Assert
            Assert.IsTrue(updatesVerified);
            mocks.VerifyAll();
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

            plugin.Gui = gui;

            var failureMechanism = new PipingFailureMechanism();
            var calculationWithOutput = new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            var surfaceLines = new PipingSurfaceLineCollection();
            var context = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

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
            bool updatesVerified = updateInfo.VerifyUpdates(context);

            // Assert
            string expectedInquiryMessage = "Als profielschematisaties wijzigen door het bijwerken, " +
                                            "dan worden de resultaten van berekeningen die deze profielschematisaties gebruiken " +
                                            $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
            Assert.AreEqual(isActionConfirmed, updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        public void CurrentPath_SurfaceLineCollectionHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string expectedFilePath = "some/path";
            var surfaceLines = new PipingSurfaceLineCollection();
            surfaceLines.AddRange(new[]
            {
                new PipingSurfaceLine(string.Empty)
            }, expectedFilePath);

            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            string currentPath = updateInfo.CurrentPath(context);

            // Assert
            Assert.AreEqual(expectedFilePath, currentPath);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileImporter_ValidInput_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var surfaceLines = new PipingSurfaceLineCollection();

            var importTarget = new PipingSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "");

            // Assert
            Assert.IsInstanceOf<SurfaceLinesCsvImporter<PipingSurfaceLine>>(importer);
            mocks.VerifyAll();
        }

        public override void Setup()
        {
            plugin = new PipingPlugin();
            updateInfo = plugin.GetUpdateInfos().First(i => i.DataType == typeof(PipingSurfaceLinesContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
        }
    }
}