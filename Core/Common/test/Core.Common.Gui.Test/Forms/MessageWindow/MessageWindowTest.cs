using System;
using System.Windows.Forms;

using Core.Common.Gui.Forms.MessageWindow;
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowTest : NUnitFormTest
    {
        private MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = MessageWindowLogAppender.Instance;
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            MessageWindowLogAppender.Instance = originalValue;
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var logAppender = new MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, MessageWindowLogAppender.Instance);

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var messageWindow = new Gui.Forms.MessageWindow.MessageWindow(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(messageWindow);
                Assert.IsInstanceOf<IMessageWindow>(messageWindow);
                Assert.AreEqual("Berichten", messageWindow.Text);
                Assert.IsInstanceOf<MessageWindowData>(messageWindow.Data);
                Assert.AreSame(messageWindow, MessageWindowLogAppender.Instance.MessageWindow);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShowDetailsButton_NoMessageSelectedOnClick_DontShowMessageWindowDialog()
        {
            // Setup
            var logAppender = new MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, MessageWindowLogAppender.Instance);

            using (var form = new Form())
            using (var messageWindow = new Gui.Forms.MessageWindow.MessageWindow(null))
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

            var logAppender = new MessageWindowLogAppender();

            // Precondition
            Assert.AreSame(logAppender, MessageWindowLogAppender.Instance);

            using (var form = new Form())
            using (var messageWindow = new Gui.Forms.MessageWindow.MessageWindow(dialogParent))
            {
                form.Controls.Add(messageWindow);
                form.Show();

                string dialogTitle = null;
                string dialogText = null;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialogTester = new FormTester(name);
                    dialogTitle = ((Form)dialogTester.TheObject).Text;
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