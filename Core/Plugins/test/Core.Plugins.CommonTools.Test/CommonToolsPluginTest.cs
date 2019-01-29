// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Test
{
    [TestFixture]
    public class CommonToolsPluginTest
    {
        [Test]
        public void GetCommonToolsPluginProperties_Always_ReturnViews()
        {
            // Setup
            using (var plugin = new CommonToolsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.NotNull(viewInfos);
                Assert.AreEqual(1, viewInfos.Length);

                ViewInfo richTextFileInfo = viewInfos.First(vi => vi.DataType == typeof(RichTextFile));

                Assert.AreEqual(richTextFileInfo.ViewType, typeof(RichTextView));
                Assert.IsNull(richTextFileInfo.Description);
                TestHelper.AssertImagesAreEqual(Resources.key, richTextFileInfo.Image);
            }
        }

        [Test]
        public void RichTextFileViewInfoName_WithoutData_EmptyString()
        {
            // Setup
            using (var plugin = new CommonToolsPlugin())
            {
                ViewInfo info = plugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));

                // Call
                string name = info.GetViewName(null, null);

                // Assert
                Assert.IsEmpty(name);
            }
        }

        [Test]
        public void RichTextFileViewInfoName_WithData_NameOfData()
        {
            // Setup
            const string expected = "SomeName";
            using (var plugin = new CommonToolsPlugin())
            {
                ViewInfo info = plugin.GetViewInfos().First(vi => vi.DataType == typeof(RichTextFile));
                var richTextFile = new RichTextFile
                {
                    Name = expected
                };

                // Call
                string name = info.GetViewName(null, richTextFile);

                // Assert
                Assert.AreEqual(expected, name);
            }
        }
    }
}