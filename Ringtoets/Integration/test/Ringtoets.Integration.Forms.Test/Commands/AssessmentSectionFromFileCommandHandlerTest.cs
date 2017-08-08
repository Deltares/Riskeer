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

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Commands;

namespace Ringtoets.Integration.Forms.Test.Commands
{
    [TestFixture]
    public class AssessmentSectionFromFileCommandHandlerTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        public void Constructor_ParentDialogNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IProjectOwner>();
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionFromFileCommandHandler(null, parentDialog, viewController);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dialogParent", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ProjectOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IWin32Window>();
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionFromFileCommandHandler(parentDialog, null, viewController);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("projectOwner", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ViewControllerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IWin32Window>();
            var projectOwner= mockRepository.StrictMock<IProjectOwner>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("viewController", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IWin32Window>();
            var projectOwner= mockRepository.StrictMock<IProjectOwner>();
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            // Call
            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionFromFileCommandHandler>(assessmentSectionFromFileCommandHandler);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_InvalidDirectory_LogsWarningProjectOwnerNotUpdated()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IWin32Window>();
            var projectOwner= mockRepository.StrictMock<IProjectOwner>();
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);

            string pathToNonExistingFolder = Path.Combine(testDataPath, "I do not exist");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathToNonExistingFolder);

            // Call
            Action action = () => assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            string expectedMessage = string.Format("De map met specificaties voor trajecten '{0}' is niet gevonden.", pathToNonExistingFolder);
            TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_validDirectoryWithEmptyShapeFile_LogsWarningProjectOwnerNotUpdated()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.StrictMock<IWin32Window>();
            var projectOwner= mockRepository.StrictMock<IProjectOwner>();
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);

            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathValidFolder);

            string messageText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageText = messageBox.Text;
                messageBox.ClickOk();
            };

            // Call
            Action action = () => assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Er kunnen geen trajecten gelezen worden uit het shapebestand.";
            TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);
            Assert.AreEqual(expectedMessage, messageText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ValidDirectoryUserClicksCancel_ProjectOwnerNotUpdated()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                new ButtonTester("Cancel", selectionDialog).Click();
            };

            // Call
            assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ValidDirectoryOkClicked_SetsFirstReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathValidFolder);

            var rowCount = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                rowCount = grid.Rows.Count;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            Assert.AreEqual(3, rowCount);
            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection1_2(true), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ValidDirectoryOkClickedForDuplicateAssessmentSection_SetsFirstReadAssessmentSectionWithUniqueName()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFile =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFile, pathValidFolder);

            project.AssessmentSections.Add(TestAssessmentSection1_2(true));
            string expectedAssessmentSectionName = NamingHelper.GetUniqueName(project.AssessmentSections, "Traject 1-2", a => a.Name);

            var signallingValueRadioButtonSelected = false;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var signallingValueRadioButton = (RadioButton) new RadioButtonTester("SignallingValueRadioButton", selectionDialog).TheObject;
                signallingValueRadioButtonSelected = signallingValueRadioButton.Checked;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            assessmentSectionFromFile.AddAssessmentSectionFromFile();

            // Assert
            Assert.IsTrue(signallingValueRadioButtonSelected);
            Assert.AreEqual(2, project.AssessmentSections.Count);
            AssessmentSection assessmentSection = project.AssessmentSections[1];
            Assert.IsNotNull(assessmentSection);

            AssessmentSection expectedAssessmentSection = TestAssessmentSection1_2(true);
            expectedAssessmentSection.Name = expectedAssessmentSectionName;
            AssertAssessmentSection(expectedAssessmentSection, assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ValidDirectoryLowLimitSelectedOkClicked_SetsFirstReadAssessmentSectionWithLowLimit()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFile, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var lowLimitValueRadioButton = (RadioButton) new RadioButtonTester("LowLimitValueRadioButton", selectionDialog).TheObject;
                lowLimitValueRadioButton.Checked = true;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            assessmentSectionFromFile.AddAssessmentSectionFromFile();

            // Assert
            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection1_2(false), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_SecondRowSelectedOkClicked_SetsSecondReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFile, pathValidFolder);

            var rowCount = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                rowCount = grid.Rows.Count;
                DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView[0, 1].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            assessmentSectionFromFile.AddAssessmentSectionFromFile();

            // Assert
            Assert.AreEqual(3, rowCount);
            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection2_1(), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ThirdRowSelectedOkClicked_SetsThirdReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView[0, 2].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            Action call = () => assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Er zijn geen instellingen gevonden voor het geselecteerde traject. Standaardinstellingen zullen gebruikt worden.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection3_3(), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void AddAssessmentSectionFromFile_ShapeWithoutPointsOkClicked_LogsAndSetsAssessmentSectionWithoutReferenceLine()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog= mockRepository.Stub<IWin32Window>();
            var project = new RingtoetsProject();
            var projectOwner= mockRepository.Stub<IProjectOwner>();
            projectOwner.Stub(po => po.Project).Return(project);
            var viewController= mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(null)).IgnoreArguments().Return(true);
            mockRepository.ReplayAll();

            var assessmentSectionFromFileCommandHandler =
                new AssessmentSectionFromFileCommandHandler(parentDialog, projectOwner, viewController);
            string pathValidFolder = Path.Combine(testDataPath, "ShapeWithoutPoints");
            SetShapeFileDirectory(assessmentSectionFromFileCommandHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView.Rows[1].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            Action call = () => assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Het importeren van de referentielijn is mislukt.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            AssessmentSection assessmentSection = project.AssessmentSections.FirstOrDefault();
            Assert.IsNotNull(assessmentSection);

            AssessmentSection expectedAssessmentSection = TestAssessmentSection1_2(true);
            expectedAssessmentSection.ReferenceLine = null;

            AssertAssessmentSection(expectedAssessmentSection, assessmentSection);
            mockRepository.VerifyAll();
        }

        private static void SetShapeFileDirectory(AssessmentSectionFromFileCommandHandler commandHandler, string nonExistingFolder)
        {
            const string privateShapeFileDirectoryName = "shapeFileDirectory";
            Type commandHandlerType = commandHandler.GetType();
            FieldInfo fieldInfo = commandHandlerType.GetField(privateShapeFileDirectoryName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Assert.Fail("Unable to find private field '{0}'", privateShapeFileDirectoryName);
            }
            else
            {
                fieldInfo.SetValue(commandHandler, nonExistingFolder);
            }
        }

        #region Test Assessment Sections

        private static AssessmentSection TestAssessmentSection1_2(bool useSignalingValue)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "1-2",
                Name = "Traject 1-2",
                FailureMechanismContribution =
                {
                    Norm = useSignalingValue ? 1.0 / 3000 : 1.0 / 1000
                }
            };
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = 2;
            assessmentSection.GrassCoverErosionOutwards.GeneralInput.N = 2;
            assessmentSection.HeightStructures.GeneralInput.N = 2;
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(160679.9250, 475072.583),
                new Point2D(160892.0751, 474315.4917)
            });

            return assessmentSection;
        }

        private static AssessmentSection TestAssessmentSection2_1()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(155556.9191, 464341.1281),
                new Point2D(155521.4761, 464360.7401)
            });

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dune)
            {
                Id = "2-1",
                Name = "Traject 2-1",
                FailureMechanismContribution =
                {
                    Norm = 1.0 / 300
                },
                ReferenceLine = referenceLine,
                GrassCoverErosionInwards =
                {
                    GeneralInput =
                    {
                        N = 3
                    }
                },
                GrassCoverErosionOutwards =
                {
                    GeneralInput =
                    {
                        N = 3
                    }
                },
                HeightStructures =
                {
                    GeneralInput =
                    {
                        N = 3
                    }
                }
            };

            return assessmentSection;
        }

        private static AssessmentSection TestAssessmentSection3_3()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "3-3",
                Name = "Traject 3-3",
                FailureMechanismContribution =
                {
                    Norm = 1.0 / 300
                },
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(147367.32190, 476902.91571),
                new Point2D(147410.0515, 476938.9447)
            });

            return assessmentSection;
        }

        #endregion

        #region Asserts

        private static void AssertAssessmentSection(AssessmentSection expected, AssessmentSection actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.FailureMechanismContribution.Norm, actual.FailureMechanismContribution.Norm);
            Assert.AreEqual(expected.Composition, actual.Composition);

            Assert.AreEqual(expected.GrassCoverErosionInwards.GeneralInput.N, actual.GrassCoverErosionInwards.GeneralInput.N);
            Assert.AreEqual(expected.GrassCoverErosionOutwards.GeneralInput.N, actual.GrassCoverErosionOutwards.GeneralInput.N);
            Assert.AreEqual(expected.HeightStructures.GeneralInput.N, actual.HeightStructures.GeneralInput.N);

            AssertReferenceLine(expected.ReferenceLine, actual.ReferenceLine);
        }

        private static void AssertReferenceLine(ReferenceLine expected, ReferenceLine actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }
            Assert.IsNotNull(actual);
            Point2D[] expectedPoints = expected.Points.ToArray();
            Point2D[] actualPoints = actual.Points.ToArray();
            CollectionAssert.AreEqual(expectedPoints, actualPoints,
                                      new Point2DComparerWithTolerance(1e-6),
                                      "Unexpected geometry found in ReferenceLine");
        }

        #endregion
    }
}