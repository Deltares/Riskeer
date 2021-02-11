// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class PipingFailureMechanismSectionsContextUpdateInfoTest : NUnitFormTest
    {
        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

                // Call
                string name = importInfo.Name;

                // Assert
                Assert.AreEqual("Vakindeling", name);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

                // Call
                string category = importInfo.Category;

                // Assert
                Assert.AreEqual("Algemeen", category);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

                // Call
                Image image = importInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_FailureMechanismSectionsSourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            string sourcePath = TestHelper.GetScratchPadPath();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), sourcePath);
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = importInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_FailureMechanismSectionsSourcePathNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

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
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo importInfo = GetUpdateInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileImporter_WithValidData_ReturnsFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                IFileImporter importer = updateInfo.CreateFileImporter(context, string.Empty);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CurrentPath_FailureMechanismSectionsSourcePathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            string sourcePath = TestHelper.GetScratchPadPath();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), sourcePath);
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string currentFilePath = updateInfo.CurrentPath(context);

                // Assert
                Assert.AreEqual(sourcePath, currentFilePath);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void VerifyUpdates_NoProbabilisticCalculations_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            });

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
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
        public void VerifyUpdates_ProbabilisticCalculationsWithoutOutput_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
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
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnsExpectedInquiryMessage(bool isActionConfirmed)
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculationWithOutput = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

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

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool updatesVerified = updateInfo.VerifyUpdates(context);

                // Assert
                string expectedInquiryMessage = "Als u de vakindeling wijzigt, dan worden de resultaten van alle probabilistische piping berekeningen verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedInquiryMessage, textBoxMessage);
                Assert.AreEqual(isActionConfirmed, updatesVerified);
            }

            mocks.VerifyAll();
        }

        private static UpdateInfo GetUpdateInfo(PipingPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ui => ui.DataType == typeof(PipingFailureMechanismSectionsContext));
        }
    }
}