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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test
{
    [TestFixture]
    public class PipingSurfaceLineSelectionDialogTest
    {
        private const int selectItemColumnIndex = 0;
        private const int nameColumnIndex = 1;

        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_DialogParentNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingSurfaceLineSelectionDialog(null, Enumerable.Empty<PipingSurfaceLine>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dialogParent", exception.ParamName);
        }

        [Test]
        public void Constructor_SurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingSurfaceLineSelectionDialog(testForm, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLines", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, Enumerable.Empty<PipingSurfaceLine>()))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                CollectionAssert.IsEmpty(dialog.SelectedItems);
                Assert.AreEqual("Selecteer profielschematisaties", dialog.Text);
                Assert.IsTrue(dialog.GenerateSemiProbabilistic);
                Assert.IsFalse(dialog.GenerateProbabilistic);
            }
        }

        [Test]
        public void Constructor_ControlsCorrectlyInitialized()
        {
            // Call
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, Enumerable.Empty<PipingSurfaceLine>()))
            {
                // Assert
                var selectAllButton = new ButtonTester("SelectAllButton", dialog);
                var deselectAllButton = new ButtonTester("DeselectAllButton", dialog);
                var generateButton = new ButtonTester("DoForSelectedButton", dialog);
                var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                Assert.AreEqual("Selecteer alles", selectAllButton.Text);
                Assert.AreEqual("Deselecteer alles", deselectAllButton.Text);
                Assert.AreEqual("Genereren", generateButton.Text);
                Assert.AreEqual("Annuleren", cancelButton.Text);

                var semiProbabilisticCheckBox = new CheckBoxTester("SemiProbabilisticCheckBox", dialog);
                var probabilisticCheckBox = new CheckBoxTester("ProbabilisticCheckBox", dialog);
                Assert.AreEqual("Semi-probabilistische toets", semiProbabilisticCheckBox.Text);
                Assert.IsTrue(semiProbabilisticCheckBox.Checked);
                Assert.AreEqual("Probabilistische toets", probabilisticCheckBox.Text);
                Assert.IsFalse(probabilisticCheckBox.Checked);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, Enumerable.Empty<PipingSurfaceLine>()))
            {
                dialog.Show();

                // Assert
                CollectionAssert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridView dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[selectItemColumnIndex];
                Assert.AreEqual("Gebruik", locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[nameColumnIndex];
                Assert.AreEqual("Profielschematisatie", nameColumn.HeaderText);
                Assert.AreEqual("Name", nameColumn.DataPropertyName);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, nameColumn.AutoSizeMode);
                Assert.IsTrue(nameColumn.ReadOnly);
            }
        }

        [Test]
        public void Constructor_SurfaceLinesOneEntry_OneRowInGrid()
        {
            // Setup
            const string testName = "testName";
            var pipingSurfaceLine = new PipingSurfaceLine(testName);

            // Call
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, new[]
            {
                pipingSurfaceLine
            }))
            {
                // Assert
                dialog.Show();

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl").TheObject;
                Assert.AreEqual(1, dataGridViewControl.Rows.Count);
                Assert.IsFalse((bool) dataGridViewControl.Rows[0].Cells[selectItemColumnIndex].Value);
                Assert.AreEqual(testName, (string) dataGridViewControl.Rows[0].Cells[nameColumnIndex].Value);
            }
        }

        [Test]
        public void GivenDialogWithSelectedItems_WhenCloseWithoutConfirmation_ThenReturnsEmptyCollection()
        {
            // Given
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                dialog.Close();

                // Then
                CollectionAssert.IsEmpty(dialog.SelectedItems);
            }
        }

        [Test]
        public void GivenDialogWithSelectedItems_WhenCancelButtonClicked_ThenReturnsEmptyCollection()
        {
            // Given
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                cancelButton.Click();

                // Then
                CollectionAssert.IsEmpty(dialog.SelectedItems);
            }
        }

        [Test]
        public void GivenDialogWithSelectedItems_WhenDoForSelectedButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedSurfaceLine = new PipingSurfaceLine("surface line 1");
            PipingSurfaceLine[] surfaceLines =
            {
                selectedSurfaceLine,
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var generateButton = new ButtonTester("DoForSelectedButton", dialog);
                generateButton.Click();

                // Then
                CollectionAssert.AreEqual(new[]
                {
                    selectedSurfaceLine
                }, dialog.SelectedItems);
            }
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllItemsSelected()
        {
            // Setup
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var dataGridView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                var button = new ButtonTester("SelectAllButton", dialog);

                // Precondition
                Assert.IsFalse((bool) rows[0].Cells[selectItemColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[selectItemColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsTrue((bool) rows[0].Cells[selectItemColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[selectItemColumnIndex].Value);
            }
        }

        [Test]
        public void DeselectAllButton_AllItemsSelectedDeselectAllButtonClicked_AllItemsNotSelected()
        {
            // Setup
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var dataGridView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                var button = new ButtonTester("DeselectAllButton", dialog);

                foreach (DataGridViewRow row in rows)
                {
                    row.Cells[selectItemColumnIndex].Value = true;
                }

                // Precondition
                Assert.IsTrue((bool) rows[0].Cells[selectItemColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[selectItemColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsFalse((bool) rows[0].Cells[selectItemColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[selectItemColumnIndex].Value);
            }
        }

        [Test]
        public void GivenDialog_WhenNoSurfaceLinesSelected_ThenDoForSelectedButtonDisabled()
        {
            // Given
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var buttonTester = new ButtonTester("DoForSelectedButton", dialog);

                // When
                var button = (Button) buttonTester.TheObject;

                // Then
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void GivenDialog_WhenSurfaceLinesSelectedAndNoCheckboxesChecked_ThenDoForSelectedButtonDisabled()
        {
            // Given
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                var semiProbabilisticCheckbox = new CheckBoxTester("SemiProbabilisticCheckBox", dialog);
                semiProbabilisticCheckbox.UnCheck();

                var buttonTester = new ButtonTester("DoForSelectedButton", dialog);

                // When
                var button = (Button) buttonTester.TheObject;

                // Then
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCheckBoxes))]
        public void GivenDialog_WhenSurfaceLinesSelectedAndCheckboxChecked_ThenDoForSelectedButtonEnabled(IEnumerable<Func<PipingSurfaceLineSelectionDialog, CheckBoxTester>> getCheckBoxFuncs)
        {
            // Given
            var surfaceLines = new[]
            {
                new PipingSurfaceLine("surface line 1"),
                new PipingSurfaceLine("surface line 2")
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, surfaceLines))
            {
                dialog.Show();

                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                selectionView.Rows[0].Cells[0].Value = true;

                foreach (Func<PipingSurfaceLineSelectionDialog, CheckBoxTester> getCheckBoxFunc in getCheckBoxFuncs)
                {
                    CheckBoxTester checkBox = getCheckBoxFunc(dialog);
                    checkBox.Check();
                }

                var buttonTester = new ButtonTester("DoForSelectedButton", dialog);

                // When
                var button = (Button) buttonTester.TheObject;

                // Then
                Assert.IsTrue(button.Enabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenDialog_WhenChangingSemiProbabilisticCheckBoxValue_ThenGenerateSemiProbabilisticExpectedValue(bool checkBoxChecked)
        {
            // Given
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, Enumerable.Empty<PipingSurfaceLine>()))
            {
                dialog.Show();
                
                // When
                var semiProbabilisticCheckBox = new CheckBoxTester("SemiProbabilisticCheckBox", dialog);
                if (checkBoxChecked)
                {
                    semiProbabilisticCheckBox.Check();
                }
                else
                {
                    semiProbabilisticCheckBox.UnCheck();
                }
                
                // Then
                Assert.AreEqual(checkBoxChecked, dialog.GenerateSemiProbabilistic);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenDialog_WhenChangingProbabilisticCheckBoxValue_ThenGenerateProbabilisticExpectedValue(bool checkBoxChecked)
        {
            // Given
            using (var dialog = new PipingSurfaceLineSelectionDialog(testForm, Enumerable.Empty<PipingSurfaceLine>()))
            {
                dialog.Show();
                
                // When
                var probabilisticCheckBox = new CheckBoxTester("ProbabilisticCheckBox", dialog);
                if (checkBoxChecked)
                {
                    probabilisticCheckBox.Check();
                }
                else
                {
                    probabilisticCheckBox.UnCheck();
                }
                
                // Then
                Assert.AreEqual(checkBoxChecked, dialog.GenerateProbabilistic);
            }
        }

        private static IEnumerable<TestCaseData> GetCheckBoxes
        {
            get
            {
                yield return new TestCaseData(new List<Func<PipingSurfaceLineSelectionDialog, CheckBoxTester>>
                {
                    dialog => new CheckBoxTester("SemiProbabilisticCheckBox", dialog)
                });
                yield return new TestCaseData(new List<Func<PipingSurfaceLineSelectionDialog, CheckBoxTester>>
                {
                    dialog => new CheckBoxTester("ProbabilisticCheckBox", dialog)
                });
                yield return new TestCaseData(new List<Func<PipingSurfaceLineSelectionDialog, CheckBoxTester>>
                {
                    dialog => new CheckBoxTester("SemiProbabilisticCheckBox", dialog),
                    dialog => new CheckBoxTester("ProbabilisticCheckBox", dialog)
                });
            }
        }
    }
}