// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using GuiFormsMessageWindow = Core.Common.Gui.Forms.MessageWindow;

namespace Core.Common.Gui.Test.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowTest : NUnitFormTest
    {
        private GuiFormsMessageWindow.MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = GuiFormsMessageWindow.MessageWindowLogAppender.Instance;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            GuiFormsMessageWindow.MessageWindowLogAppender.Instance = originalValue;
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var logAppender = new GuiFormsMessageWindow.MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, GuiFormsMessageWindow.MessageWindowLogAppender.Instance);

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var messageWindow = new GuiFormsMessageWindow.MessageWindow(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(messageWindow);
                Assert.IsInstanceOf<GuiFormsMessageWindow.IMessageWindow>(messageWindow);
                Assert.IsInstanceOf<DataTable>(messageWindow.Data);
                Assert.AreSame(messageWindow, GuiFormsMessageWindow.MessageWindowLogAppender.Instance.MessageWindow);
            }

            mocks.VerifyAll();
        }
        
        [Test]
        public void OnLoad_ExpectedMessageWindowText()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var logAppender = new GuiFormsMessageWindow.MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, GuiFormsMessageWindow.MessageWindowLogAppender.Instance);

            using (var form = new Form())
            using (var messageWindow = new GuiFormsMessageWindow.MessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);

                // Call
                form.Show();

                // Assert
                Assert.AreEqual("Berichten", messageWindow.Text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddMessage_LevelIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var logAppender = new GuiFormsMessageWindow.MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, GuiFormsMessageWindow.MessageWindowLogAppender.Instance);

            // Setup
            using (var messageWindow = new GuiFormsMessageWindow.MessageWindow(null))
            {
                // Call
                TestDelegate call = () => messageWindow.AddMessage(null, new DateTime(),
                                                                   "Should throw exception");

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("level", paramName);
            }
        }

        [Test]
        public void ShowDetailsButton_NoMessageSelectedOnClick_DontShowMessageWindowDialog()
        {
            // Setup
            var logAppender = new GuiFormsMessageWindow.MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, GuiFormsMessageWindow.MessageWindowLogAppender.Instance);

            using (var form = new Form())
            using (var messageWindow = new GuiFormsMessageWindow.MessageWindow(null))
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

            var detailedMessage = "TestDetailedMessage";

            var logAppender = new GuiFormsMessageWindow.MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, GuiFormsMessageWindow.MessageWindowLogAppender.Instance);

            using (var form = new Form())
            using (var messageWindow = new GuiFormsMessageWindow.MessageWindow(dialogParent))
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
                var dataGridView = (DataGridView) new ControlTester("messagesDataGridView").TheObject;
                dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                var buttonTester = new ToolStripButtonTester("buttonShowDetails");

                // Call
                buttonTester.Click();

                // Assert
                Assert.AreEqual("Berichtdetails", dialogTitle);
                Assert.AreEqual(detailedMessage, dialogText);
            }
            mocks.VerifyAll();
        }
    }
}