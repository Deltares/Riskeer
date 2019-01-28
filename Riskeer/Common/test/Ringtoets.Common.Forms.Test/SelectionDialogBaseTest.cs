// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test
{
    [TestFixture]
    public class SelectionDialogBaseTest
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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var dialog = new TestSelectionDialogBase(testForm))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
            }
        }

        [Test]
        public void Constructor_DialogParentIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestSelectionDialogBase(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                dialog.Show();

                // Assert
                Assert.AreEqual(new Size(240, 90), dialog.AutoScrollMinSize);

                CollectionAssert.IsEmpty(dialog.SelectedItems);

                var dataGridViewControl = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                DataGridView dataGridView = dataGridViewControl.Controls.OfType<DataGridView>().First();
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[selectItemColumnIndex];
                const string expectedLocationCalculateHeaderText = "Gebruik";
                Assert.AreEqual(expectedLocationCalculateHeaderText, locationCalculateColumn.HeaderText);
                Assert.AreEqual("Selected", locationCalculateColumn.DataPropertyName);
                Assert.IsFalse(locationCalculateColumn.ReadOnly);

                var nameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[nameColumnIndex];
                Assert.IsEmpty(nameColumn.HeaderText);
                Assert.AreEqual("Name", nameColumn.DataPropertyName);
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, nameColumn.AutoSizeMode);
                Assert.IsTrue(nameColumn.ReadOnly);

                var buttonTester = new ButtonTester("DoForSelectedButton", dialog);
                var button = (Button) buttonTester.TheObject;
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void Constructor_Always_SetMinimumSize()
        {
            // Setup
            using (var dialog = new TestSelectionDialogBase(testForm))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(370, dialog.MinimumSize.Width);
                Assert.AreEqual(550, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void GivenDialogWithSelectedItems_WhenCloseWithoutConfirmation_ThenReturnsEmptyCollection()
        {
            // Given
            var items = new[]
            {
                new object(),
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                dialog.SetDataSource(items);

                dialog.Show();
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
            var selectedItem = new object();
            object[] items =
            {
                selectedItem,
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                dialog.SetDataSource(items);

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                cancelButton.Click();

                // Then
                CollectionAssert.IsEmpty(dialog.SelectedItems);
            }
        }

        [Test]
        public void GivenDialogWithSelectedItems_WhenDoForSelectedButton_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedItem = new object();
            object[] items =
            {
                selectedItem,
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                var selectionView = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                dialog.SetDataSource(items);

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var generateButton = new ButtonTester("DoForSelectedButton", dialog);
                generateButton.Click();

                // Then
                IEnumerable<object> result = dialog.SelectedItems;

                CollectionAssert.AreEqual(new[]
                {
                    selectedItem
                }, result);
            }
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllItemsSelected()
        {
            // Setup
            var items = new[]
            {
                new object(),
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                dialog.SetDataSource(items);
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
            var items = new[]
            {
                new object(),
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                dialog.SetDataSource(items);
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
        public void DoForSelectedButton_NoneSelected_DoForSelectedButtonDisabled()
        {
            // Setup
            var items = new[]
            {
                new object(),
                new object()
            };

            using (var dialog = new TestFullyConfiguredSelectionDialogBase(testForm))
            {
                dialog.SetDataSource(items);
                dialog.Show();
                var buttonTester = new ButtonTester("DoForSelectedButton", dialog);

                // Call
                var button = (Button) buttonTester.TheObject;

                // Assert
                Assert.IsFalse(button.Enabled);
                CollectionAssert.IsEmpty(dialog.SelectedItems);
            }
        }

        private class TestSelectionDialogBase : SelectionDialogBase<object>
        {
            public TestSelectionDialogBase(IWin32Window dialogParent) : base(dialogParent) {}
        }

        private class TestFullyConfiguredSelectionDialogBase : SelectionDialogBase<object>
        {
            public TestFullyConfiguredSelectionDialogBase(IWin32Window dialogParent) : base(dialogParent)
            {
                InitializeDataGridView("");
            }

            public void SetDataSource(IEnumerable<object> items)
            {
                base.SetDataSource(items.Select(o => new SelectableRow<object>(o, o.ToString())).ToArray());
            }
        }
    }
}