﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ForeshoreProfilesContextTreeNodeInfoTest
    {
        private RiskeerPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<ICalculatableFailureMechanism>();
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
            var failureMechanism = mocks.Stub<ICalculatableFailureMechanism>();
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
            var failureMechanism = mocks.Stub<ICalculatableFailureMechanism>();
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
                var failureMechanism = mocks.Stub<ICalculatableFailureMechanism>();

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

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(contextMenuBuilder);
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