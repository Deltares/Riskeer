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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Base.TestUtil.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using Core.Gui.Forms.ViewHost;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Dialogs;
using Riskeer.Integration.Plugin.Handlers;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionFromFileHandlerTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        public void Constructor_ParentDialogNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionFromFileHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dialogParent", exception.ParamName);
        }

        [Test]
        public void GetAssessmentSectionFromFile_InvalidDirectory_ThrowsCriticalFileReadException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathToNonExistingFolder = Path.Combine(testDataPath, "I do not exist");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathToNonExistingFolder);

            // Call
            void Call() => assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            var expectedMessage = $"De map met specificaties voor trajecten '{pathToNonExistingFolder}' is niet gevonden.";
            var exception = Assert.Throws<CriticalFileReadException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_validDirectoryWithEmptyShapeFile_ShowsWarningDialogAndThrowsCriticalFileReadException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            string messageText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageText = messageBox.Text;
                messageBox.ClickOk();
            };

            // Call
            void Call() => assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Er kunnen geen trajecten gelezen worden uit het shapebestand.";
            var exception = Assert.Throws<CriticalFileValidationException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedMessage, messageText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_ValidDirectoryUserClicksCancel_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                new ButtonTester("Cancel", selectionDialog).Click();
            };

            // Call
            AssessmentSection assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            Assert.IsNull(assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_ValidDirectoryOkClicked_ReturnsFirstReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            var rowCount = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                rowCount = grid.Rows.Count;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            AssessmentSection assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            Assert.AreEqual(3, rowCount);
            AssertAssessmentSection(TestAssessmentSection1_2(true), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_ValidDirectoryLowLimitSelectedOkClicked_ReturnsFirstReadAssessmentSectionWithLowLimit()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var lowLimitValueRadioButton = (RadioButton) new RadioButtonTester("LowLimitValueRadioButton", selectionDialog).TheObject;
                lowLimitValueRadioButton.Checked = true;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            AssessmentSection assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            AssertAssessmentSection(TestAssessmentSection1_2(false), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_SecondRowSelectedOkClicked_ReturnsSecondReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

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
            AssessmentSection assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            Assert.AreEqual(3, rowCount);
            AssertAssessmentSection(TestAssessmentSection2_1(), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_ThirdRowSelectedOkClicked_ReturnsThirdReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView[0, 2].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            AssessmentSection assessmentSection = null;
            
            // Call
            void Call() => assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Er zijn geen instellingen gevonden voor het geselecteerde traject. Standaardinstellingen zullen gebruikt worden.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection3_3(), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAssessmentSectionFromFile_ShapeWithoutPointsOkClicked_LogsAndReturnsAssessmentSectionWithoutReferenceLine()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "ShapeWithoutPoints");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView.Rows[1].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            AssessmentSection assessmentSection = null;
            
            // Call
            void Call() => assessmentSection = assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Het importeren van de referentielijn is mislukt.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
            Assert.IsNotNull(assessmentSection);

            AssessmentSection expectedAssessmentSection = TestAssessmentSection1_2(true);
            expectedAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

            AssertAssessmentSection(expectedAssessmentSection, assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GetAssessmentSectionFromFile_ShapeWithInvalidNorm_ThrowsCriticalFileValidationException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "InvalidNorm");
            SetShapeFileDirectory(assessmentSectionFromFileHandler, pathValidFolder);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;

                DataGridView dataGridView = ControlTestHelper.GetDataGridView(selectionDialog, "dataGridView");
                dataGridView.Rows[0].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            void Call() => assessmentSectionFromFileHandler.GetAssessmentSectionFromFile();

            // Assert
            const string expectedMessage = "Het traject kan niet aangemaakt worden met een ondergrens van 1/9.999.999 en een signaleringswaarde van 1/8.888.888. " +
                                           "De waarde van de ondergrens en signaleringswaarde moet in het bereik [0,000001, 0,1] liggen en " +
                                           "de ondergrens moet gelijk zijn aan of groter zijn dan de signaleringswaarde.";
            var exception = Assert.Throws<CriticalFileValidationException>(Call);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(exception.InnerException);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DoPostHandleActions_ProjectNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var viewController = mockRepository.Stub<IDocumentViewController>();
            mockRepository.ReplayAll();
            
            // Call
            void Call() => AssessmentSectionFromFileHandler.DoPostHandleActions(null, viewController);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("project", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DoPostHandleActions_ViewControllerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var project = mockRepository.Stub<IProject>();
            mockRepository.ReplayAll();
            
            // Call
            void Call() => AssessmentSectionFromFileHandler.DoPostHandleActions(project, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewController", exception.ParamName);
            mockRepository.VerifyAll();
        }
        
        [Test]
        public void DoPostHandleActions_WithAssessmentSection_OpensViewForAssessmentSection()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            
            var mockRepository = new MockRepository();
            var viewController = mockRepository.StrictMock<IDocumentViewController>();
            viewController.Expect(dvc => dvc.OpenViewForData(assessmentSection)).Return(true);
            mockRepository.ReplayAll();

            var project = new RiskeerProject
            {
                AssessmentSections =
                {
                    assessmentSection
                }
            };

            // Call
            AssessmentSectionFromFileHandler.DoPostHandleActions(project, viewController);
            
            // Assert
            mockRepository.VerifyAll();
        }

        private static void SetShapeFileDirectory(AssessmentSectionFromFileHandler handler, string nonExistingFolder)
        {
            const string privateShapeFileDirectoryName = "shapeFileDirectory";
            Type commandHandlerType = handler.GetType();
            FieldInfo fieldInfo = commandHandlerType.GetField(privateShapeFileDirectoryName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Assert.Fail("Unable to find private field '{0}'", privateShapeFileDirectoryName);
            }
            else
            {
                fieldInfo.SetValue(handler, nonExistingFolder);
            }
        }

        #region Test Assessment Sections

        private static AssessmentSection TestAssessmentSection1_2(bool useSignalingValue)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike,
                                                          1.0 / 1000,
                                                          1.0 / 3000)
            {
                Id = "1-2",
                Name = "Traject 1-2",
                FailureMechanismContribution =
                {
                    NormativeNorm = useSignalingValue ? NormType.Signaling : NormType.LowerLimit
                }
            };
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = (RoundedDouble) 2.0;
            assessmentSection.GrassCoverErosionOutwards.GeneralInput.N = (RoundedDouble) 2.0;
            assessmentSection.HeightStructures.GeneralInput.N = (RoundedDouble) 2.0;
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(160679.9250, 475072.583),
                new Point2D(160892.0751, 474315.4917)
            });

            return assessmentSection;
        }

        private static AssessmentSection TestAssessmentSection2_1()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dune,
                                                          1.0 / 100,
                                                          1.0 / 300)
            {
                Id = "2-1",
                Name = "Traject 2-1",
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                },
                GrassCoverErosionInwards =
                {
                    GeneralInput =
                    {
                        N = (RoundedDouble) 3.0
                    }
                },
                GrassCoverErosionOutwards =
                {
                    GeneralInput =
                    {
                        N = (RoundedDouble) 3.0
                    }
                },
                HeightStructures =
                {
                    GeneralInput =
                    {
                        N = (RoundedDouble) 3.0
                    }
                }
            };

            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(155556.9191, 464341.1281),
                new Point2D(155521.4761, 464360.7401)
            });

            return assessmentSection;
        }

        private static AssessmentSection TestAssessmentSection3_3()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike,
                                                          1.0 / 100,
                                                          1.0 / 300)
            {
                Id = "3-3",
                Name = "Traject 3-3",
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                }
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
            Assert.AreEqual(expected.FailureMechanismContribution.LowerLimitNorm, actual.FailureMechanismContribution.LowerLimitNorm);
            Assert.AreEqual(expected.FailureMechanismContribution.SignalingNorm, actual.FailureMechanismContribution.SignalingNorm);
            Assert.AreEqual(expected.FailureMechanismContribution.NormativeNorm, actual.FailureMechanismContribution.NormativeNorm);
            Assert.AreEqual(expected.Composition, actual.Composition);

            Assert.AreEqual(expected.GrassCoverErosionInwards.GeneralInput.N, actual.GrassCoverErosionInwards.GeneralInput.N);
            Assert.AreEqual(expected.GrassCoverErosionOutwards.GeneralInput.N, actual.GrassCoverErosionOutwards.GeneralInput.N);
            Assert.AreEqual(expected.HeightStructures.GeneralInput.N, actual.HeightStructures.GeneralInput.N);

            AssertReferenceLine(expected.ReferenceLine, actual.ReferenceLine);
        }

        private static void AssertReferenceLine(ReferenceLine expected, ReferenceLine actual)
        {
            Point2D[] expectedPoints = expected.Points.ToArray();
            Point2D[] actualPoints = actual.Points.ToArray();
            CollectionAssert.AreEqual(expectedPoints, actualPoints,
                                      new Point2DComparerWithTolerance(1e-6),
                                      "Unexpected geometry found in ReferenceLine");
        }

        #endregion
    }
}