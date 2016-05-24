// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
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

            var webBrowser = TypeUtils.GetField<WebBrowser>(htmlPageView, "webBrowser");
            Assert.IsTrue(webBrowser.ScriptErrorsSuppressed);
            Assert.IsFalse(webBrowser.IsWebBrowserContextMenuEnabled);
        }

        [Test]
        [RequiresSTA]
        public void Data_SetValidUrl_ShouldReturnSetValueAndNavigateToCorrespondingPage()
        {
            // Setup
            var url = new WebLink("", new Uri("http://www.google.nl"));

            // Call
            var htmlPageView = new HtmlPageView
            {
                Data = url
            };

            // Assert
            ShowHtmlPageView(htmlPageView, () =>
            {
                var webBrowser = TypeUtils.GetField<WebBrowser>(htmlPageView, "webBrowser");

                Assert.AreSame(url, htmlPageView.Data);
                Assert.IsNotNull(webBrowser.Url);
                Assert.AreEqual("https://www.google.nl/?gws_rd=ssl", webBrowser.Url.AbsoluteUri);
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
                var webBrowser = TypeUtils.GetField<WebBrowser>(htmlPageView, "webBrowser");

                Assert.IsNull(htmlPageView.Data);
                Assert.IsNotNull(webBrowser.Url);
                Assert.AreEqual("about:blank", webBrowser.Url.AbsoluteUri);
            });
        }

        [Test]
        [RequiresSTA]
        public void Data_SetInvalidUrl_ShouldReturnValueAndAndNavigateToBlankPage()
        {
            // Setup
            var url = new WebLink("", null);

            // Call
            var htmlPageView = new HtmlPageView
            {
                Data = url
            };

            // Assert
            ShowHtmlPageView(htmlPageView, () =>
            {
                var webBrowser = TypeUtils.GetField<WebBrowser>(htmlPageView, "webBrowser");

                Assert.AreSame(url, htmlPageView.Data);
                Assert.IsNotNull(webBrowser.Url);
                Assert.AreEqual("about:blank", webBrowser.Url.AbsoluteUri);
            });
        }

        private static void ShowHtmlPageView(HtmlPageView htmlPageView, Action assertAction)
        {
            var documentCompleted = false;
            var webBrowser = TypeUtils.GetField<WebBrowser>(htmlPageView, "webBrowser");

            webBrowser.DocumentCompleted += (s, e) => { documentCompleted = true; };

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