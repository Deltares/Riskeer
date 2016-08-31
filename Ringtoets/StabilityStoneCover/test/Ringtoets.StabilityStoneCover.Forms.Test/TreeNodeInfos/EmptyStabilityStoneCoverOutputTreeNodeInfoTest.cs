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
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class EmptyStabilityStoneCoverOutputTreeNodeInfoTest
    {
        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(EmptyStabilityStoneCoverOutput));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(EmptyStabilityStoneCoverOutput), info.TagType);

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
            Assert.IsNull(info.ChildNodeObjects);
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            mocks.ReplayAll();

            var output = new EmptyStabilityStoneCoverOutput();

            // Call
            string nodeText = info.Text(output);

            // Assert
            Assert.AreEqual("Resultaat", nodeText);
        }

        [Test]
        public void ForeColor_Always_ReturnGrayText()
        {
            // Setup
            mocks.ReplayAll();

            var output = new EmptyStabilityStoneCoverOutput();

            // Call
            Color color = info.ForeColor(output);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void Image_Always_ReturnOutputIcon()
        {
            // Setup
            mocks.ReplayAll();

            var output = new EmptyStabilityStoneCoverOutput();

            // Call
            Image icon = info.Image(output);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralOutputIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var output = new EmptyStabilityStoneCoverOutput();

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(output, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(output, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }
    }
}