using System.Windows.Forms;
using Core.Common.Controls.Swf;
using NUnit.Framework;
using Rhino.Mocks;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.Base.Tests.Controls.Swf
{
    [TestFixture]
    public class MessageBoxTest
    {
        [Test]
        public void UseCustomMessageBoxIfDefined()
        {
            var mockRepository = new MockRepository();
            var customMessageBox = mockRepository.StrictMock<IMessageBox>();
            customMessageBox.Expect(m => m.Show("text", "caption", MessageBoxButtons.OK)).Return(DialogResult.OK);
            MessageBox.CustomMessageBox = customMessageBox;

            mockRepository.ReplayAll();

            MessageBox.Show("text", "caption", MessageBoxButtons.OK);

            mockRepository.VerifyAll();

            //don't forget to reset!
            MessageBox.CustomMessageBox = null;
        }
    }
}