﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
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
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnDrop);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.OnNodeRenamed);
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
            mocks.ReplayAll();

            var emptyCollection = new ObservableList<ForeshoreProfile>();
            var context = new ForeshoreProfilesContext(emptyCollection, assessmentSection);

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
            mocks.ReplayAll();

            var emptyCollection = new ObservableList<ForeshoreProfile>
            {
                new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties())
            };
            var context = new ForeshoreProfilesContext(emptyCollection, assessmentSection);

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
            mocks.ReplayAll();

            var profile1 = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties());
            var profile2 = new ForeshoreProfile(new Point2D(1, 1), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties());
            var profile3 = new ForeshoreProfile(new Point2D(2, 2), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties());
            var emptyCollection = new ObservableList<ForeshoreProfile>
            {
                profile1,
                profile2,
                profile3,
            };
            var context = new ForeshoreProfilesContext(emptyCollection, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            var expectedChildren = new[]
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

                var emptyCollection = new ObservableList<ForeshoreProfile>();
                var context = new ForeshoreProfilesContext(emptyCollection, assessmentSection);

                var contextMenuBuilder = mocks.Stub<IContextMenuBuilder>();
                contextMenuBuilder.Expect(b => b.AddImportItem()).Return(contextMenuBuilder);
                contextMenuBuilder.Expect(b => b.AddSeparator()).Return(contextMenuBuilder);
                contextMenuBuilder.Expect(b => b.AddCollapseAllItem()).Return(contextMenuBuilder);
                contextMenuBuilder.Expect(b => b.AddExpandAllItem()).Return(contextMenuBuilder);
                contextMenuBuilder.Expect(b => b.Build()).Return(null);

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