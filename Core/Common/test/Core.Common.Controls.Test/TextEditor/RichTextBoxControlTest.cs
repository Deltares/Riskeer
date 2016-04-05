using System.Windows.Forms;
using Core.Common.Controls.TextEditor;
using NUnit.Framework;

namespace Core.Common.Controls.Test.TextEditor
{
    [TestFixture]
    public class RichTextBoxControlTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            var control = new RichTextBoxControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
        }

        [Test]
        public void Text_ValueSet_ReturnsValue()
        {
            // Setup
            var data = "<Some text>";
            var control = new RichTextBoxControl();

            // Call
            control.Text = data;

            // Assert
            Assert.AreEqual(data, control.Text);
        }
    }
}