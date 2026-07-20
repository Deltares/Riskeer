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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var emptyCollection = new ForeshoreProfileCollection();
            var context = new ForeshoreProfilesContext(emptyCollection, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void ForeColor_CollectionHasElements_ReturnControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
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
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnChildrenOfCollection()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
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
        }

        [Test]
        public void ContextMenuStrip_Always_ReturnContextMenuStrip()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

                var emptyCollection = new ForeshoreProfileCollection();
                var context = new ForeshoreProfilesContext(emptyCollection, failureMechanism, assessmentSection);

                var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
                contextMenuBuilder.AddImportItem().Returns(contextMenuBuilder);
                contextMenuBuilder.AddUpdateItem().Returns(contextMenuBuilder);
                contextMenuBuilder.AddSeparator().Returns(contextMenuBuilder);
                contextMenuBuilder.AddCollapseAllItem().Returns(contextMenuBuilder);
                contextMenuBuilder.AddExpandAllItem().Returns(contextMenuBuilder);
                contextMenuBuilder.AddPropertiesItem().Returns(contextMenuBuilder);

                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeViewControl).Returns(contextMenuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);

                // Assert
                plugin.Dispose();
                Received.InOrder(() =>
                {
                    contextMenuBuilder.AddImportItem();
                    contextMenuBuilder.AddUpdateItem();
                    contextMenuBuilder.AddSeparator();
                    contextMenuBuilder.AddCollapseAllItem();
                    contextMenuBuilder.AddExpandAllItem();
                    contextMenuBuilder.AddSeparator();
                    contextMenuBuilder.AddPropertiesItem();
                    contextMenuBuilder.Build();
                });
            }
        }
    }
}