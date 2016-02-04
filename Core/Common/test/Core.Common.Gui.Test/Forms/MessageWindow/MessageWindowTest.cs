using System.Windows.Forms;

using Core.Common.Gui.Forms.MessageWindow;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowTest
    {
        private MessageWindowLogAppender originalValue;

        [SetUp]
        public void SetUp()
        {
            originalValue = MessageWindowLogAppender.Instance;
        }

        [TearDown]
        public void TearDown()
        {
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
            using (var messageswindow = new Gui.Forms.MessageWindow.MessageWindow(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(messageswindow);
                Assert.IsInstanceOf<IMessageWindow>(messageswindow);
                Assert.AreEqual("Berichten", messageswindow.Text);
                Assert.IsInstanceOf<MessageWindowData>(messageswindow.Data);
                Assert.AreSame(messageswindow, MessageWindowLogAppender.Instance.MessageWindow);
                
            }
            mocks.VerifyAll();
        }
    }
}