using System.Windows.Forms;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms
{
    [TestFixture]
    public class RichTextViewTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var view = new RichTextView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }
        }
    }
}