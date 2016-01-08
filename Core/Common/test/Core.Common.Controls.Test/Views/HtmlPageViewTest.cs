using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Utils;
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
            Assert.IsFalse(htmlPageView.WebBrowser.IsWebBrowserContextMenuEnabled);
        }

        [Test]
        [RequiresSTA]
        public void Data_SetValidUrl_ShouldReturnSetValueAndNavigateToCorrespondingPage()
        {
            // Setup
            var url = new Url("", "http://www.google.nl");

            // Call
            var htmlPageView = new HtmlPageView
            {
                Data = url
            };

            // Assert
            ShowHtmlPageView(htmlPageView, () =>
            {
                Assert.AreSame(url, htmlPageView.Data);
                Assert.IsNotNull(htmlPageView.WebBrowser.Url);
                Assert.AreEqual("https://www.google.nl/?gws_rd=ssl", htmlPageView.WebBrowser.Url.AbsoluteUri);
            });
        }

        [Test]
        [RequiresSTA]
        public void Data_SetNullValue_ShouldReturnNullValueAndAndNavigateToBlankPage()
        {
            // Call
            var htmlPageView = new HtmlPageView
            {
                Data = null
            };

            // Assert
            ShowHtmlPageView(htmlPageView, () =>
            {
                Assert.IsNull(htmlPageView.Data);
                Assert.IsNotNull(htmlPageView.WebBrowser.Url);
                Assert.AreEqual("about:blank", htmlPageView.WebBrowser.Url.AbsoluteUri);
            });
        }

        [Test]
        [RequiresSTA]
        public void Data_SetInvalidUrl_ShouldReturnValueAndAndNavigateToBlankPage()
        {
            // Setup
            var url = new Url("", null);

            // Call
            var htmlPageView = new HtmlPageView
            {
                Data = url
            };

            // Assert
            ShowHtmlPageView(htmlPageView, () =>
            {
                Assert.AreSame(url, htmlPageView.Data);
                Assert.IsNotNull(htmlPageView.WebBrowser.Url);
                Assert.AreEqual("about:blank", htmlPageView.WebBrowser.Url.AbsoluteUri);
            });
        }

        private static void ShowHtmlPageView(HtmlPageView htmlPageView, Action assertAction)
        {
            var documentCompleted = false;

            htmlPageView.WebBrowser.DocumentCompleted += (s, e) => { documentCompleted = true; };

            WindowsFormsTestHelper.ShowModal(htmlPageView,
                                             v =>
                                             {
                                                 // Ensure the web browser is completely loaded
                                                 while (!documentCompleted)
                                                 {
                                                     Application.DoEvents();
                                                 }

                                                 // Perform the assert action
                                                 assertAction();
                                             });
        }
    }
}