// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class BackgroundMapDataContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            var context = new BackgroundMapDataContext(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var text = info.Text(context);

                // Assert
                Assert.AreEqual("Achtergrondlaag", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            var context = new BackgroundMapDataContext(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var image = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.Map, image);
            }
        }

        [Test]
        public void ForeColor_ConnectedMapData_ReturnControlText()
        {
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            var context = new BackgroundMapDataContext(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var image = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), image);
            }
        }

        [Test]
        public void ForeColor_UnconnectedMapData_ReturnControlText()
        {
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            var context = new BackgroundMapDataContext(mapData);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var image = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), image);
            }
        }

        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(BackgroundMapDataContext));
        }
    }
}