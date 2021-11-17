// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data;
using System.Windows.Forms;
using Core.Gui.Clipboard;
using Core.Gui.Forms.Log;
using Core.Gui.TestUtil.Clipboard;
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Forms.Log
{
    [TestFixture]
    public class MessageWindowTest : NUnitFormTest
    {
        private MessageWindowLogAppender originalValue;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var logAppender = new MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, MessageWindowLogAppender.Instance);

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (MessageWindow messageWindow = ShowMessageWindow(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(messageWindow);
                Assert.IsInstanceOf<IMessageWindow>(messageWindow);
                Assert.IsInstanceOf<DataTable>(messageWindow.Data);
                Assert.AreSame(messageWindow, MessageWindowLogAppender.Instance.MessageWindow);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddMessage_LevelIsNull_ThrowsArgumentNullException()
        {
            // Setup
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                // Call
                void Call() => messageWindow.AddMessage(null, new DateTime(), "Should throw exception");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
                Assert.AreEqual("level", paramName);
            }
        }

        [Test]
        public void GivenMessageWindow_WhenMultipleMessagesAdded_ThenFirstColumnOnFirstRowIsSelected()
        {
            // Given
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;
                Level topLeftCellValue = Level.Info;

                // Precondition
                Assert.IsNull(dataGridView.CurrentCell);

                // When
                messageWindow.AddMessage(Level.Warn, new DateTime(), "DetailedWarnMessage");
                messageWindow.AddMessage(Level.Error, new DateTime(), "DetailedErrorMessage");
                messageWindow.AddMessage(topLeftCellValue, new DateTime(), "DetailedInfoMessage");
                messageWindow.Refresh();

                // Then
                Assert.IsNotNull(dataGridView.CurrentCell);

                DataGridViewCell topLeftCell = dataGridView.Rows[0].Cells[0];
                Assert.AreSame(topLeftCell, dataGridView.CurrentCell);
                Assert.AreEqual(topLeftCellValue.ToString(), dataGridView.CurrentCell.Value);
            }
        }

        [Test]
        public void ShowDetailsButton_NoMessageSelectedOnClick_DoNotShowMessageWindowDialog()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var button = new ToolStripButtonTester("buttonShowDetails");

                // Call
                button.Click();

                // Assert
                // No dialog window shown
            }
        }

        [Test]
        public void ShowDetailsButton_MessageSelectedOnClick_ShowMessageWindowDialogWithDetails()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            const string detailedMessage = "TestDetailedMessage";

            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                string dialogTitle = null;
                string dialogText = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new FormTester(name);
                    dialogTitle = ((Form) dialogTester.TheObject).Text;
                    var testBoxTester = new TextBoxTester("textBox");
                    dialogText = testBoxTester.Text;
                    dialogTester.Close();
                };
                messageWindow.AddMessage(Level.Warn, new DateTime(), detailedMessage);
                messageWindow.Refresh();
                var buttonTester = new ToolStripButtonTester("buttonShowDetails");

                // Call
                buttonTester.Click();

                // Assert
                Assert.AreEqual("Berichtdetails", dialogTitle);
                Assert.AreEqual(detailedMessage, dialogText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDetailsButton_NoMessageSelectedOnDoubleClick_DoNotShowMessageWindowDialog()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var gridView = new ControlTester("messagesDataGridView");

                // Call
                gridView.DoubleClick();

                // Assert
                // No dialog window shown
            }
        }

        [Test]
        public void ShowDetailsButton_DoubleClickOnRowHeader_DoesShowMessageWindowDialog()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();
            const string detailedMessage = "TestDetailedMessage";

            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                string dialogTitle = null;
                string dialogText = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new FormTester(name);
                    dialogTitle = ((Form) dialogTester.TheObject).Text;
                    var testBoxTester = new TextBoxTester("textBox");
                    dialogText = testBoxTester.Text;
                    dialogTester.Close();
                };

                var gridView = new ControlTester("messagesDataGridView");
                messageWindow.AddMessage(Level.Warn, new DateTime(), detailedMessage);
                messageWindow.Refresh();
                int rowHeaderColumnIndex = ((DataGridView) gridView.TheObject).Rows[0].HeaderCell.ColumnIndex;

                // Call
                gridView.FireEvent("CellMouseDoubleClick", new DataGridViewCellMouseEventArgs(
                                       rowHeaderColumnIndex, 0, 0, 0,
                                       new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0)));

                // Assert
                Assert.AreEqual("Berichtdetails", dialogTitle);
                Assert.AreEqual(detailedMessage, dialogText);
            }
        }

        [Test]
        public void ShowDetailsButton_DoubleClickOnColumnHeader_DoNotShowMessageWindowDialog()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var gridView = new ControlTester("messagesDataGridView");
                messageWindow.AddMessage(Level.Warn, new DateTime(), "TestDetailedMessage");
                messageWindow.Refresh();
                int columnHeaderRowIndex = ((DataGridView) gridView.TheObject).Columns[0].HeaderCell.RowIndex;

                // Call
                gridView.FireEvent("CellMouseDoubleClick", new DataGridViewCellMouseEventArgs(
                                       0, columnHeaderRowIndex, 0, 0,
                                       new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0)));

                // Assert
                // No dialog window shown
            }
        }

        [Test]
        [TestCase(MouseButtons.Middle)]
        [TestCase(MouseButtons.None)]
        [TestCase(MouseButtons.Right)]
        [TestCase(MouseButtons.XButton1)]
        [TestCase(MouseButtons.XButton2)]
        public void ShowDetailsButton_MessageSelectedOnDoubleClickOtherThanLeft_DoNotShowMessageWindowDialog(MouseButtons mouseButton)
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var gridView = new ControlTester("messagesDataGridView");
                messageWindow.AddMessage(Level.Warn, new DateTime(), "TestDetailedMessage");
                messageWindow.Refresh();

                // Call
                gridView.FireEvent("CellMouseDoubleClick", new DataGridViewCellMouseEventArgs(
                                       0, 0, 0, 0,
                                       new MouseEventArgs(mouseButton, 2, 0, 0, 0)));

                // Assert
                // No dialog window shown
            }
        }

        [Test]
        public void ShowDetailsButton_MessageSelectedOnDoubleClick_ShowMessageWindowDialogWithDetails()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();
            const string detailedMessage = "TestDetailedMessage";

            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                string dialogTitle = null;
                string dialogText = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new FormTester(name);
                    dialogTitle = ((Form) dialogTester.TheObject).Text;
                    var testBoxTester = new TextBoxTester("textBox");
                    dialogText = testBoxTester.Text;
                    dialogTester.Close();
                };

                var gridView = new ControlTester("messagesDataGridView");
                messageWindow.AddMessage(Level.Warn, new DateTime(), detailedMessage);
                messageWindow.Refresh();

                // Call
                gridView.FireEvent("CellMouseDoubleClick", new DataGridViewCellMouseEventArgs(
                                       0, 0, 0, 0,
                                       new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0)));

                // Assert
                Assert.AreEqual("Berichtdetails", dialogTitle);
                Assert.AreEqual(detailedMessage, dialogText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDetailsButton_NoMessageSelectedOnEnterKeyDown_DoNotShowMessageWindowDialog()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                var gridView = new ControlTester("messagesDataGridView");

                // Call
                gridView.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));

                // Assert
                // No dialog window shown
            }
        }

        [Test]
        public void ShowDetailsButton_MessageSelectedOnEnterKeyDown_ShowMessageWindowDialogWithDetails()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();
            const string detailedMessage = "TestDetailedMessage";

            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                string dialogTitle = null;
                string dialogText = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new FormTester(name);
                    dialogTitle = ((Form) dialogTester.TheObject).Text;
                    var testBoxTester = new TextBoxTester("textBox");
                    dialogText = testBoxTester.Text;
                    dialogTester.Close();
                };

                var gridView = new ControlTester("messagesDataGridView");
                messageWindow.AddMessage(Level.Warn, new DateTime(), detailedMessage);
                messageWindow.Refresh();

                // Call
                gridView.FireEvent("KeyDown", new KeyEventArgs(Keys.Enter));

                // Assert
                Assert.AreEqual("Berichtdetails", dialogTitle);
                Assert.AreEqual(detailedMessage, dialogText);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ButtonClearAll_Click_RemovesAllMessages()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                messageWindow.AddMessage(Level.Warn, new DateTime(), "message");
                messageWindow.Refresh();
                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;

                var button = new ToolStripItemTester("buttonClearAll");

                // Call
                button.Click();

                // Assert
                Assert.AreEqual(0, dataGridView.Rows.Count);
            }
        }

        [Test]
        public void ButtonCopy_Click_CopiesContentToClipboard()
        {
            // Setup
            using (var form = new Form())
            using (new ClipboardConfig())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                messageWindow.AddMessage(Level.Warn, new DateTime(), "message");
                messageWindow.Refresh();

                var button = new ToolStripItemTester("buttonCopy");

                // Call
                button.Click();

                // Assert
                IDataObject actualDataObject = ClipboardProvider.Clipboard.GetDataObject();
                Assert.IsTrue(actualDataObject != null && actualDataObject.GetDataPresent(DataFormats.Text));
                var actualContent = (string) actualDataObject.GetData(DataFormats.Text);
                Assert.AreEqual("WARN\t\tmessage\t00:00:00", actualContent);
            }
        }

        [Test]
        public void ButtonShowInfo_ButtonUnchecked_FiltersInfoMessages()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                AddMessages(messageWindow);
                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;

                var button = new ToolStripButtonTester("buttonShowInfo");

                // Call
                button.Click();

                // Assert
                Assert.IsFalse(((ToolStripButton) button.TheObject).Checked);
                Assert.AreEqual(2, dataGridView.Rows.Count);
                string filteredLevel = Level.Info.ToString();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Assert.AreNotEqual(filteredLevel, row.Cells[0].Value.ToString());
                }
            }
        }

        [Test]
        public void ButtonShowWarn_ButtonUnchecked_FiltersWarningMessages()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                AddMessages(messageWindow);
                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;
                var button = new ToolStripButtonTester("buttonShowWarning");

                // Call
                button.Click();

                // Assert
                Assert.IsFalse(((ToolStripButton) button.TheObject).Checked);
                Assert.AreEqual(2, dataGridView.Rows.Count);
                string filteredLevel = Level.Warn.ToString();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Assert.AreNotEqual(filteredLevel, row.Cells[0].Value.ToString());
                }
            }
        }

        [Test]
        public void ButtonShowError_ButtonUnchecked_FiltersErrorMessages()
        {
            // Setup
            using (var form = new Form())
            using (MessageWindow messageWindow = ShowMessageWindow(null))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                AddMessages(messageWindow);
                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;
                var button = new ToolStripButtonTester("buttonShowError");

                // Call
                button.Click();

                // Assert
                Assert.IsFalse(((ToolStripButton) button.TheObject).Checked);
                Assert.AreEqual(2, dataGridView.Rows.Count);
                string filteredLevel = Level.Error.ToString();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    Assert.AreNotEqual(filteredLevel, row.Cells[0].Value.ToString());
                }
            }
        }

        public override void Setup()
        {
            originalValue = MessageWindowLogAppender.Instance;
        }

        public override void TearDown()
        {
            base.TearDown();
            MessageWindowLogAppender.Instance = originalValue;
        }

        private static void AddMessages(MessageWindow messageWindow)
        {
            messageWindow.AddMessage(Level.Info, new DateTime(), "Info message");
            messageWindow.AddMessage(Level.Warn, new DateTime(), "Warn message");
            messageWindow.AddMessage(Level.Error, new DateTime(), "Error message");
            messageWindow.Refresh();
        }

        private MessageWindow ShowMessageWindow(IWin32Window dialogParent)
        {
            var logAppender = new MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, MessageWindowLogAppender.Instance);

            return new MessageWindow(dialogParent);
        }
    }
}