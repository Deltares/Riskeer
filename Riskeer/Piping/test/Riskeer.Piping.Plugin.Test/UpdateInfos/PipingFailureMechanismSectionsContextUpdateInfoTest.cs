// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
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
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

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
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

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
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                Image image = updateInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }
        }

        [Test]
        public void IsEnabled_FailureMechanismSectionsSourcePathSet_ReturnTrue()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();

            string sourcePath = TestHelper.GetScratchPadPath();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), sourcePath);
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }
        }

        [Test]
        public void IsEnabled_FailureMechanismSectionsSourcePathNull_ReturnFalse()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);

            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            using (var plugin = new PipingPlugin())
            {
                plugin.Gui = gui;

                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void CreateFileImporter_WithValidData_ReturnsFileImporter()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
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
        }

        [Test]
        public void CurrentPath_FailureMechanismSectionsSourcePathSet_ReturnsExpectedPath()
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
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
        }

        [Test]
        public void VerifyUpdates_NoProbabilisticCalculations_ReturnsTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
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
        }

        [Test]
        public void VerifyUpdates_ProbabilisticCalculationsWithoutOutput_ReturnsTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnsExpectedInquiryMessage(bool isActionConfirmed)
        {
            // Setup
            var mainWindow = Substitute.For<IMainWindow>();
            var gui = Substitute.For<IGui>();
            gui.MainWindow.Returns(mainWindow);
            var assessmentSection = Substitute.For<IAssessmentSection>();
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
        }

        private static UpdateInfo GetUpdateInfo(PipingPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ui => ui.DataType == typeof(PipingFailureMechanismSectionsContext));
        }
    }
}