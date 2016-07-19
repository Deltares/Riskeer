// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Commands;

namespace Ringtoets.Integration.Forms.Test.Commands
{
    [TestFixture]
    public class AssessmentSectionFromFileCommandHandlerTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        public void CreateAssessmentSectionFromFile_InvalidDirectory_ThrowsCriticalFileReadException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.StrictMock<IWin32Window>();
            mockRepository.ReplayAll();
            AssessmentSectionFromFileCommandHandler assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);

            string pathToNonExistingFolder = Path.Combine(testDataPath, "I do not exist");

            // Call
            TestDelegate call = () => assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathToNonExistingFolder);

            // Assert
            Assert.Throws<CriticalFileReadException>(call);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_validDirectoryWithEmptyShapeFile_ThrowsCriticalFileValidationException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.StrictMock<IWin32Window>();
            mockRepository.ReplayAll();
            AssessmentSectionFromFileCommandHandler assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");

            // Call
            TestDelegate call = () => assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            CriticalFileValidationException exception = Assert.Throws<CriticalFileValidationException>(call);
            Assert.AreEqual("Er kunnen geen trajecten gelezen worden uit het shape bestand.", exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_ValidDirectoryUserClicksCancel_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                new ButtonTester("Cancel", selectionDialog).Click();
            };

            // Call
            IAssessmentSection assessmentSection = assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            Assert.IsNull(assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_ValidDirectoryOkClicked_ReturnsFirstReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");

            int rowCount = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                rowCount = grid.GetRows().Count;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            var assessmentSection = (AssessmentSection) assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            Assert.AreEqual(3, rowCount);
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection1_2(true), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_ValidDirectoryLowLimitSelectedOkClicked_ReturnsFirstReadAssessmentSectionWithLowLimit()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var combobox = (ComboBox) new ComboBoxTester("SignalingLowerLimitComboBox", selectionDialog).TheObject;
                combobox.SelectedIndex = 1;
                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            var assessmentSection = (AssessmentSection) assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection1_2(false), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_SecondRowSelectedOkClicked_ReturnsSecondReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");

            int rowCount = 0;
            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                rowCount = grid.GetRows().Count;
                var dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView[0, 1].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            // Call
            AssessmentSection assessmentSection = (AssessmentSection) assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            Assert.AreEqual(3, rowCount);
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection2_1(), assessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_ThirdRowSelectedOkClicked_ReturnsThirdReadAssessmentSection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            var assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);
            string pathValidFolder = Path.Combine(testDataPath, "ValidShapeFile");

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (ReferenceLineMetaSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", selectionDialog).TheObject;
                var dataGridView = grid.Controls.OfType<DataGridView>().First();
                dataGridView[0, 2].Selected = true;

                new ButtonTester("Ok", selectionDialog).Click();
            };

            AssessmentSection assessmentSection = null;

            // Call
            Action call = () => assessmentSection = (AssessmentSection) assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            const string expectedMessage = @"Er zijn geen instellingen gevonden voor het geselecteerde traject. Standaardinstellingen zullen gebruikt worden.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsNotNull(assessmentSection);
            AssertAssessmentSection(TestAssessmentSection3_3(), assessmentSection);
            mockRepository.VerifyAll();
        }

        #region Test Assessment Sections

        private static AssessmentSection TestAssessmentSection1_2(bool useSignalingValue)
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "1-2",
                FailureMechanismContribution =
                {
                    Norm = (useSignalingValue) ? 3000 : 1000
                }
            };
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = 2;
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dune)
            {
                Id = "2-1",
                FailureMechanismContribution =
                {
                    Norm = 300
                }
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(155556.9191, 464341.1281),
                new Point2D(155521.4761, 464360.7401)
            });

            return assessmentSection;
        }

        private static AssessmentSection TestAssessmentSection3_3()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "3-3",
                FailureMechanismContribution =
                {
                    Norm = 300
                }
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
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
            Assert.AreEqual(expected.FailureMechanismContribution.Norm, actual.FailureMechanismContribution.Norm);
            Assert.AreEqual(expected.Composition, actual.Composition);

            Assert.AreEqual(expected.GrassCoverErosionInwards.GeneralInput.N, actual.GrassCoverErosionInwards.GeneralInput.N);
            Assert.AreEqual(expected.HeightStructures.GeneralInput.N, actual.HeightStructures.GeneralInput.N);

            AssertReferenceLine(expected.ReferenceLine, actual.ReferenceLine);
        }

        private static void AssertReferenceLine(ReferenceLine expected, ReferenceLine actual)
        {
            Assert.IsNotNull(expected);
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