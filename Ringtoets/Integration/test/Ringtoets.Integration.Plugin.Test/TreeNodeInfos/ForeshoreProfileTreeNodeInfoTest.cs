// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Integration.Plugin;
using Riskeer.Integration.Plugin.Properties;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForeshoreProfileTreeNodeInfoTest
    {
        private RingtoetsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfile));
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

        [Test]
        public void Text_Always_ReturnForeshoreProfileName()
        {
            // Setup
            const string profileName = "Random profile name";
            ForeshoreProfile foreshoreProfile = new TestForeshoreProfile(profileName, "id");

            // Call
            string text = info.Text(foreshoreProfile);

            // Assert
            Assert.AreEqual(profileName, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.Foreshore, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                using (var p = new RingtoetsPlugin())
                {
                    p.Gui = gui;
                    TreeNodeInfo i = p.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfile));

                    // Call
                    i.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}