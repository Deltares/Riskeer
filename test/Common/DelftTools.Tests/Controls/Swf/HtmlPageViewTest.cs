using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using DelftTools.Utils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class HtmlPageViewTest
    {
#if ! MONO		
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var page = new HtmlPageView
                           {
                               Url = new Url("test", @"about:blank"),
                               IsWebBrowserContextMenuEnabled = false,
                               Margin = new Padding(0)
                           };

            WindowsFormsTestHelper.ShowModal(page);
        }
#endif
    }
}