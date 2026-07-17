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
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLineCollectionTreeNodeInfoTest
    {
        private MacroStabilityInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsSurfaceLinesContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
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
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            var surfaceLinesContext = new MacroStabilityInwardsSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            string text = info.Text(surfaceLinesContext);

            // Assert
            Assert.AreEqual("Profielschematisaties", text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            var surfaceLinesContext = new MacroStabilityInwardsSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            Image image = info.Image(surfaceLinesContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void ForeColor_CollectionWithoutSurfaceLines_ReturnsGrayText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            var surfaceLinesContext = new MacroStabilityInwardsSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);

            // Call
            Color foreColor = info.ForeColor(surfaceLinesContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);
        }

        [Test]
        public void ForeColor_CollectionWithSurfaceLines_ReturnsControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Line A");
            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Line B");

            var surfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            surfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, "path");

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLinesContext = new MacroStabilityInwardsSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);
            // Call
            Color foreColor = info.ForeColor(surfaceLinesContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Line A");
            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Line B");

            var surfaceLines = new MacroStabilityInwardsSurfaceLineCollection();
            surfaceLines.AddRange(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, "path");

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var surfaceLinesContext = new MacroStabilityInwardsSurfaceLinesContext(surfaceLines, failureMechanism, assessmentSection);
            // Call
            object[] objects = info.ChildNodeObjects(surfaceLinesContext);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine1,
                surfaceLine2
            }, objects);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddImportItem().Returns(menuBuilder);
            menuBuilder.AddUpdateItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(Arg.Any<object>(), treeViewControl).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddImportItem();
                menuBuilder.AddUpdateItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }
    }
}