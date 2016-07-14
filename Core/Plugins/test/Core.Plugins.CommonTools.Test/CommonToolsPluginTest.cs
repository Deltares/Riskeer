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
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using Core.Plugins.CommonTools.Property;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test
{
    [TestFixture]
    public class CommonToolsPluginTest
    {
        [Test]
        public void GetCommonToolsPluginProperties_Always_ReturnProperties()
        {
            // Setup
            var plugin = new CommonToolsPlugin();

            // Call
            var propertyInfos = plugin.GetPropertyInfos().ToList();

            // Assert
            Assert.AreEqual(1, propertyInfos.Count);

            var projectPropertyInfo = propertyInfos.First(pi => pi.DataType == typeof(Project));

            Assert.AreEqual(typeof(ProjectProperties), projectPropertyInfo.PropertyObjectType);
        }

        [Test]
        public void GetCommonToolsPluginProperties_Always_ReturnViews()
        {
            // Setup
            var plugin = new CommonToolsPlugin();

            // Call
            var viewInfos = plugin.GetViewInfos().ToArray();

            // Assert
            Assert.NotNull(viewInfos);
            Assert.AreEqual(1, viewInfos.Length);

            var richTextFileInfo = viewInfos.First(vi => vi.DataType == typeof(RichTextFile));

            Assert.AreEqual(richTextFileInfo.ViewType, typeof(RichTextView));
            Assert.IsNull(richTextFileInfo.Description);
            TestHelper.AssertImagesAreEqual(Resources.key, richTextFileInfo.Image);
        }

        [Test]
        public void RichTextFileViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            var plugin = new CommonToolsPlugin();

            var info = plugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));

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
            var plugin = new CommonToolsPlugin();

            var info = plugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
            var richTextFile = new RichTextFile
            {
                Name = expected
            };

            // Call
            var name = info.GetViewName(null, richTextFile);

            // Assert
            Assert.AreEqual(expected, name);
        }
    }
}