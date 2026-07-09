// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Gui;
using Core.Gui.ContextMenu;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Forms.Properties;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string nodeText = info.Text(null);

                // Assert
                Assert.AreEqual("Invoer", nodeText);
            }
        }

        [Test]
        public void Image_Always_ReturnOutputIcon()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image icon = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.GenericInputOutputIcon, icon);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddOpenItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);
                menuBuilder.Build().Returns((System.Windows.Forms.ContextMenuStrip) null);

                var gui = Substitute.For<IGui>();
                gui.Get(null, treeViewControl).Returns(menuBuilder);

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }

                Received.InOrder(() =>
                {
                    menuBuilder.AddOpenItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddPropertiesItem();
                    menuBuilder.Build();
                });
            }
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsInputContext));
        }
    }
}