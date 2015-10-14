using DelftTools.Controls.Swf;
using DelftTools.Utils.Reflection;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class FindAndReplaceControlTest
    {
        [TestCase(true)] // search for third "Test" occurrence (index 23)
        [TestCase(false)] // search for second "Test" occurrence (index 11)
        public void FindNextText(bool check2)
        {
            var text = "Test text\n Test text2\n Test text2";
            var findAndReplaceControl = new FindAndReplaceControl
            {
                GetTextToSearch = () => text,
                GetCurrentPosition = () => (!check2) ? 4 : 11,
                SelectText = (start, length) =>
                {
                    Assert.AreEqual(check2 ? 23 : 11, start);
                    Assert.AreEqual(4, length);
                },
                HighLightText = t => Assert.AreEqual("Test", t)
            };

            findAndReplaceControl.FindTextBox.Text = "Test";

            TypeUtils.CallPrivateMethod(findAndReplaceControl, "FindNext");
        }

        [Test]
        public void ReplaceText()
        {
            var text = "Test text\n Test text2\n Test text2";
            var findAndReplaceControl = new FindAndReplaceControl
            {
                GetTextToSearch = () => text,
                GetCurrentPosition = () => 11,
                ReplaceText = (start, length, newText) =>
                {
                    Assert.AreEqual(11, start);
                    Assert.AreEqual(4, length);
                    Assert.AreEqual("New test", newText);
                },
                SelectText = (start, length) =>
                {
                    Assert.AreEqual(23, start);
                    Assert.AreEqual(4, length);
                },
                HighLightText = t => Assert.AreEqual("Test", t)
            };

            findAndReplaceControl.FindTextBox.Text = "Test";
            findAndReplaceControl.ReplaceTextBox.Text = "New test";

            // replace second "Test" occurrence (index 11) with "New test"
            TypeUtils.CallPrivateMethod(findAndReplaceControl, "Replace");
        }
    }
}