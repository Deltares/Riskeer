﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.IO;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class ClosingStructuresContextUpdateInfoTest : NUnitFormTest
    {
        private UpdateInfo updateInfo;
        private ClosingStructuresPlugin plugin;

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = updateInfo.Name;

            // Assert
            Assert.AreEqual("Kunstwerken", name);
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.StructuresIcon, image);
        }

        [Test]
        public void IsEnabled_SourcePathNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var structures = new StructureCollection<ClosingStructure>();

            var context = new ClosingStructuresContext(structures, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = updateInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_SourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(Enumerable.Empty<ClosingStructure>(), "some path");

            var context = new ClosingStructuresContext(failureMechanism.ClosingStructures,
                                                       failureMechanism,
                                                       assessmentSection);

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
            Assert.AreEqual(@"Shapebestand (*.shp)|*.shp", fileFilter);
        }

        [Test]
        public void CurrentPath_StructureCollectionHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string expectedFilePath = "some/path";
            var structures = new StructureCollection<ClosingStructure>();
            structures.AddRange(Enumerable.Empty<ClosingStructure>(), expectedFilePath);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresContext(structures, failureMechanism, assessmentSection);

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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var structures = new StructureCollection<ClosingStructure>();

            var importTarget = new ClosingStructuresContext(structures, failureMechanism, assessmentSection);

            // Call
            IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "This is valid");

            // Assert
            Assert.IsInstanceOf<ClosingStructuresImporter>(importer);
            mocks.VerifyAll();
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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var structures = new StructureCollection<ClosingStructure>();
            var context = new ClosingStructuresContext(structures, failureMechanism, assessmentSection);

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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            });

            var structures = new StructureCollection<ClosingStructure>();
            var context = new ClosingStructuresContext(structures, failureMechanism, assessmentSection);

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
            string expectedInquiryMessage = "Wanneer de kunstwerken wijzigen als gevolg van het bijwerken " +
                                            "zullen de resultaten van berekeningen die deze kunstwerken gebruiken worden verwijderd." +
                                            $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
            Assert.AreEqual(isActionConfirmed, updatesVerified);
            mocks.VerifyAll();
        }

        public override void Setup()
        {
            plugin = new ClosingStructuresPlugin();
            updateInfo = plugin.GetUpdateInfos().First(i => i.DataType == typeof(ClosingStructuresContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
        }
    }
}