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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsProperties = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresContextTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

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
        }

        [Test]
        public void Text_Always_ReturnExpectedText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var context = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures,
                                                              failureMechanism,
                                                              assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                const string expectedText = "Kunstwerken";
                Assert.AreEqual(expectedText, text);
            }
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var context = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures,
                                                              failureMechanism,
                                                              assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsProperties.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ForeColor_CollectionHasElements_ReturnControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                new TestStabilityPointStructure()
            }, "path");

            // Precondition
            CollectionAssert.IsNotEmpty(failureMechanism.StabilityPointStructures);

            var context = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures,
                                                              failureMechanism,
                                                              assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnStabilityPointStructures()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            StabilityPointStructure structure1 = new TestStabilityPointStructure("id structure1");
            StabilityPointStructure structure2 = new TestStabilityPointStructure("id structure2");
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "path");

            var context = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures,
                                                              failureMechanism,
                                                              assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(context);

                // Assert
                Assert.AreEqual(2, children.Length);
                Assert.AreSame(structure1, children.ElementAt(0));
                Assert.AreSame(structure2, children.ElementAt(1));
            }
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StabilityPointStructures);

            var context = new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures,
                                                              failureMechanism,
                                                              assessmentSection);
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            {
                menuBuilder.AddImportItem().Returns(menuBuilder);
                menuBuilder.AddUpdateItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);
                menuBuilder.Build().Returns((ContextMenuStrip) null);
            }

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(null, treeViewControl).Returns(menuBuilder);

                plugin.Gui = gui;
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                info.ContextMenuStrip(null, null, treeViewControl);
            }

            // Assert
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin gui)
        {
            return gui.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresContext));
        }
    }
}