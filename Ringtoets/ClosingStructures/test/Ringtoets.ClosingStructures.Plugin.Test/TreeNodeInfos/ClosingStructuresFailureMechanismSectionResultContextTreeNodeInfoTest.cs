// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
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
            using (var plugin = new ClosingStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var mechanism = new ClosingStructuresFailureMechanism();
                var context = new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(mechanism.SectionResults, mechanism);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual("Resultaat", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.FailureMechanismSectionResultIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var section = new FailureMechanismSection("A",
                                                      new[]
                                                      {
                                                          new Point2D(0, 0)
                                                      });
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);
            var sectionResultContext = new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(new[]
            {
                sectionResult
            }, failureMechanism);

            var mockRepository = new MockRepository();
            var menuBuilder= mockRepository.StrictMock<IContextMenuBuilder>();
            menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.Build()).Return(null);

            using (var plugin = new ClosingStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                using (var treeViewControl = new TreeViewControl())
                {
                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(g => g.Get(sectionResultContext, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(sectionResultContext, null, treeViewControl);
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(ClosingStructuresPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>));
        }
    }
}