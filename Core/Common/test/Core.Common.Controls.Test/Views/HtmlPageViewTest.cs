using Core.Common.Controls.Views;
using NUnit.Framework;

namespace Core.Common.Controls.Test.Views
{
    [TestFixture]
    public class HtmlPageViewTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var htmlPageView = new HtmlPageView();

            // Assert
            Assert.IsInstanceOf<IView>(htmlPageView);
            Assert.IsTrue(htmlPageView.WebBrowser.ScriptErrorsSuppressed);
        }
    }
}
