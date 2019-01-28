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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.Integration.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForeshoreProfilesContextTreeNodeInfoTest
    {
        private RingtoetsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ForeshoreProfilesContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            info = null;
        }

        [Test]
        public void Initialized_ExpectedValues()
        {
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
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
        public void Text_Always_ReturnText()
        {
            // Call
            string text = info.Text(null);

            // Assert
            Assert.AreEqual("Voorlandprofielen", text);
        }

        [Test]
        public void Image_Always_ReturnFolderIcon()
        {
            // Call
            Image icon = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var emptyCollection = new ForeshoreProfileCollection();
            var context = new ForeshoreProfilesContext(emptyCollection, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            mocks.ReplayAll();
        }

        [Test]
        public void ForeColor_CollectionHasElements_ReturnControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var collection = new ForeshoreProfileCollection();
            collection.AddRange(new[]
            {
                new TestForeshoreProfile()
            }, "path");

            var context = new ForeshoreProfilesContext(collection, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            mocks.ReplayAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnChildrenOfCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            ForeshoreProfile profile1 = new TestForeshoreProfile("A", "ID A");
            ForeshoreProfile profile2 = new TestForeshoreProfile("B", "ID B");
            ForeshoreProfile profile3 = new TestForeshoreProfile("C", "ID C");
            var collection = new ForeshoreProfileCollection();
            collection.AddRange(new[]
            {
                profile1,
                profile2,
                profile3
            }, "path");

            var context = new ForeshoreProfilesContext(collection, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            ForeshoreProfile[] expectedChildren =
            {
                profile1,
                profile2,
                profile3
            };
            CollectionAssert.AreEqual(expectedChildren, children);
            mocks.ReplayAll();
        }

        [Test]
        public void ContextMenuStrip_Always_ReturnContextMenuStrip()
        {
            // Setup
            var mocks = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = mocks.Stub<IFailureMechanism>();

                var emptyCollection = new ForeshoreProfileCollection();
                var context = new ForeshoreProfilesContext(emptyCollection, failureMechanism, assessmentSection);

                var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    contextMenuBuilder.Expect(b => b.AddImportItem()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddUpdateItem()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddSeparator()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddCollapseAllItem()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddExpandAllItem()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddSeparator()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.AddPropertiesItem()).Return(contextMenuBuilder);
                    contextMenuBuilder.Expect(b => b.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(contextMenuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);

                // Assert
                plugin.Dispose();
                mocks.VerifyAll();
            }
        }
    }
}