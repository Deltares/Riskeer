// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using FormsResources = Riskeer.MacroStabilityInwards.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelCollectionContextImportInfoTest : NUnitFormTestWithHiddenDesktop
    {
        private ImportInfo importInfo;
        private MacroStabilityInwardsPlugin plugin;

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Stochastische ondergrondmodellen", name);
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
            TestHelper.AssertImagesAreEqual(FormsResources.SoilProfileIcon, image);
        }

        [Test]
        public void IsEnabled_ReferenceLineWithoutGeometry_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineWithGeometry_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("D-Soil Model bestand (*.soil)|*.soil", fileFilterGenerator.Filter);
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario());

            var stochasticSoilModelCollection = new MacroStabilityInwardsStochasticSoilModelCollection();
            var context = new MacroStabilityInwardsStochasticSoilModelCollectionContext(stochasticSoilModelCollection, failureMechanism, assessmentSection);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            var stochasticSoilModelCollection = new MacroStabilityInwardsStochasticSoilModelCollection();
            var context = new MacroStabilityInwardsStochasticSoilModelCollectionContext(stochasticSoilModelCollection, failureMechanism, assessmentSection);

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
            string expectedInquiryMessage = "Als u stochastische ondergrondmodellen importeert, " +
                                            "dan worden alle rekenresultaten van dit toetsspoor verwijderd." +
                                            $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
            Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
            Assert.AreEqual(isActionConfirmed, updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importTarget = new MacroStabilityInwardsStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(importTarget, "");

            // Assert
            Assert.IsInstanceOf<StochasticSoilModelImporter<MacroStabilityInwardsStochasticSoilModel>>(importer);
            mocks.VerifyAll();
        }

        public override void Setup()
        {
            plugin = new MacroStabilityInwardsPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
        }
    }
}