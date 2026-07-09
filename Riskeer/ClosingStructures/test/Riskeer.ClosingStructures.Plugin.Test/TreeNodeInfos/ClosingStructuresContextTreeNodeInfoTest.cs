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
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Properties;
using NSubstitute;

namespace Riskeer.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresContextTreeNodeInfoTest
    {
        private ClosingStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresContext));
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
        public void Text_Always_ReturnExpectedText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var closingStructuresContext = new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection);

            // Call
            string text = info.Text(closingStructuresContext);

            // Assert
            const string expectedText = "Kunstwerken";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void Image_Always_ReturnExpectedImage()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var closingStructuresContext = new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection);

            // Call
            Image image = info.Image(closingStructuresContext);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, image);
        }

        [Test]
        public void ForeColor_CollectionHasElementsEmpty_ReturnControlText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                new TestClosingStructure()
            }, "some path");

            // Precondition
            CollectionAssert.IsNotEmpty(failureMechanism.ClosingStructures);

            var closingStructuresContext = new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(closingStructuresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnClosingStructures()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            ClosingStructure closingStructure1 = new TestClosingStructure("structure1");
            ClosingStructure closingStructure2 = new TestClosingStructure("structure2");
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                closingStructure1,
                closingStructure2
            }, "some path");

            var closingStructuresContext = new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(closingStructuresContext);

            // Assert
            Assert.AreEqual(2, children.Length);
            Assert.AreSame(closingStructure1, children.ElementAt(0));
            Assert.AreSame(closingStructure2, children.ElementAt(1));
        }

        [Test]
        public void ForeColor_CollectionIsEmpty_ReturnGrayText()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.ClosingStructures);

            var closingStructuresContext = new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(closingStructuresContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
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

            menuBuilder.Build().Returns((System.Windows.Forms.ContextMenuStrip) null);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(null, treeViewControl).Returns(menuBuilder);

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