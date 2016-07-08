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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Properties;
using Core.Plugins.CommonTools.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test
{
    [TestFixture]
    public class CommonToolsGuiPluginTest
    {
        [Test]
        public void GetCommonToolsGuiPluginProperties_Always_ReturnProperties()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            // Call
            var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

            // Assert
            Assert.AreEqual(2, propertyInfos.Count);

            var projectPropertyInfo = propertyInfos.First(pi => pi.DataType == typeof(Project));
            var urlPropertyInfo = propertyInfos.First(pi => pi.DataType == typeof(WebLink));

            Assert.AreEqual(typeof(ProjectProperties), projectPropertyInfo.PropertyObjectType);
            Assert.AreEqual(typeof(WebLinkProperties), urlPropertyInfo.PropertyObjectType);
        }

        [Test]
        public void GetCommonToolsGuiPluginProperties_Always_ReturnViews()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            // Call
            var viewInfos = guiPlugin.GetViewInfos().ToArray();

            // Assert
            Assert.NotNull(viewInfos);
            Assert.AreEqual(2, viewInfos.Length);

            var richTextFileInfo = viewInfos.First(vi => vi.DataType == typeof(RichTextFile));
            var webLinkInfo = viewInfos.First(vi => vi.DataType == typeof(WebLink));

            Assert.AreEqual(richTextFileInfo.ViewType, typeof(RichTextView));
            Assert.IsNull(richTextFileInfo.Description);
            TestHelper.AssertImagesAreEqual(Common.Gui.Properties.Resources.key, richTextFileInfo.Image);

            Assert.AreEqual(webLinkInfo.ViewType, typeof(HtmlPageView));
            Assert.AreEqual(webLinkInfo.Description, Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser);
            TestHelper.AssertImagesAreEqual(Resources.HomeIcon, webLinkInfo.Image);
        }

        [Test]
        public void RichTextFileViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
            
            // Call
            var name = info.GetViewName(null, null);

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void RichTextFileViewInfoName_WithData_NameOfData()
        {
            // Setup
            var expected = "SomeName";
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
            var richTextFile = new RichTextFile
            {
                Name = expected
            };
            
            // Call
            var name = info.GetViewName(null, richTextFile);

            // Assert
            Assert.AreEqual(expected, name);
        }

        [Test]
        public void HtmlPageViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(WebLink));

            // Call
            var name = info.GetViewName(null, null);

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void HtmlPageViewInfoName_WithData_NameOfData()
        {
            // Setup
            var expected = "SomeName";
            var guiPlugin = new CommonToolsGuiPlugin();

            var info = guiPlugin.GetViewInfos().First(vi => vi.DataType == typeof(WebLink));
            var webLink = new WebLink(expected, null);

            // Call
            var name = info.GetViewName(null, webLink);

            // Assert
            Assert.AreEqual(expected, name);
        }
    }
}