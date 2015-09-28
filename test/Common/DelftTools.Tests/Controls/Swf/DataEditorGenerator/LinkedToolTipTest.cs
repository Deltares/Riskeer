using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator
{
    [TestFixture]
    public class LinkedToolTipTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowWithToolTip()
        {
            var control = new Form {Text = "testje"};

            var firstLabel = new Label {Text = "hover here 1...", Location = new Point(100, 50)};

            firstLabel.MouseHover +=
                (s, e) => LinkedToolTip.SetToolTip(firstLabel, "This is a tooltip test", "www.google.com");

            firstLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            control.Controls.Add(firstLabel);

            var secondLabel = new Label {Text = "hover here 2...", Location = new Point(100, 100)};

            secondLabel.MouseHover += (s, e) => LinkedToolTip.SetToolTip(secondLabel,
                                                                         "This is the second tooltip test.\r\nIt tests how the tooltip displays multi-lined text,\r\nas you can observe...",
                                                                         "www.nu.nl");

            secondLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            control.Controls.Add(secondLabel);

            var thirdLabel = new Label {Text = "hover here 3...", Location = new Point(100, 150)};

            thirdLabel.MouseHover += (s, e) => LinkedToolTip.SetToolTip(thirdLabel, "This is the third tooltiptest.", null);

            thirdLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            control.Controls.Add(thirdLabel);

            WindowsFormsTestHelper.ShowModal(control);

            LinkedToolTip.Dispose();
        }
    }
}
