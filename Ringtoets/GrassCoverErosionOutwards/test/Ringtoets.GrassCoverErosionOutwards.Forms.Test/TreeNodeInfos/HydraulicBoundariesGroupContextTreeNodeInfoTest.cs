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
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundariesGroupContextTreeNodeInfoTest
    {
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundariesGroupContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HydraulicBoundariesGroupContext), info.TagType);

            Assert.IsNull(info.EnsureVisibleOnCreate);
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

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism);

            // Call
            string nodeText = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische randvoorwaarden", nodeText);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var context = new HydraulicBoundariesGroupContext(failureMechanism);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnChildDataNodes()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var sectionSpecificWaterLevelHydraulicBoundaryLocationsContext = (SectionSpecificWaterLevelHydraulicBoundaryLocationsContext) children[0];
            Assert.AreSame(failureMechanism.GrassCoverErosionOutwardsHydraulicBoundaryLocations, sectionSpecificWaterLevelHydraulicBoundaryLocationsContext.WrappedData);
        }
    }
}